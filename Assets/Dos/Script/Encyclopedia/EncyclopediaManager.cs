// ไฟล์ใหม่: EncyclopediaManager.cs
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

// ใช้ Singleton.cs ที่คุณมี
public class EncyclopediaManager : Singleton<EncyclopediaManager>
{
    // เราใช้ Dictionary เพื่อค้นหาข้อมูลปลาได้ทันทีจาก "ชื่อ"
    private Dictionary<string, EncyclopediaEntry> entryDatabase;

    // เราต้องรอให้ FishManager พร้อมก่อน
    private bool isInitialized = false;

    // ฟังก์ชันนี้จะทำงานหลังจาก Awake() ของทุกสคริปต์
    private void Start()
    {
        InitializeDatabase();
    }

    private void InitializeDatabase()
    {
        if (isInitialized) return;

        entryDatabase = new Dictionary<string, EncyclopediaEntry>();
        
        // 1. ดึง "ปลาทั้งหมด" ใน ZoneA มาจาก FishManager
        // (เราต้องไปเพิ่มฟังก์ชัน GetZoneAFish() ใน FishManager ก่อน)
        List<BaseFish> allZoneAFish = FishManager.Instance.fishPrefabsRedZone;

        if (allZoneAFish == null)
        {
            Debug.LogError("Encyclopedia: ไม่สามารถดึงรายชื่อปลาจาก FishManager!");
            return;
        }

        // 2. สร้าง "ช่องว่าง" ในสารานุกรมสำหรับปลาทุกตัว
        foreach (BaseFish fish in allZoneAFish)
        {
            if (!entryDatabase.ContainsKey(fish.Name))
            {
                entryDatabase.Add(fish.Name, new EncyclopediaEntry(fish.Name));
            }
        }
        
        isInitialized = true;
        Debug.Log($"Encyclopedia: เริ่มต้นฐานข้อมูลสำเร็จ มีปลา {entryDatabase.Count} ชนิด");

        // 3. (สำคัญ) ถ้ามีการ Load Game เกิดขึ้นก่อน Start()
        // เราต้องเรียก LoadData อีกครั้งเพื่อให้ข้อมูลที่โหลดมาถูกนำไปใช้
        TryReapplyLoadedData();
    }
    
    private EncyclopediaData pendingLoadData; // ตัวแปรพักข้อมูลที่ Load มา

    /// <summary>
    /// ฟังก์ชันหลัก! ใช้สำหรับส่งปลาที่จับได้ใหม่มาตรวจสอบ
    /// </summary>
    public void SubmitFish(Fish fish)
    {
        if (!isInitialized) 
        {
            Debug.LogWarning("Encyclopedia ยังไม่พร้อม SubmitFish");
            return;
        }
        
        // 1. ตรวจสอบว่ามีปลาชนิดนี้ในฐานข้อมูลหรือไม่
        if (entryDatabase.TryGetValue(fish.Name, out EncyclopediaEntry entry))
        {
            // 2. ตรวจสอบว่าค้นพบครั้งแรก หรือ น้ำหนักมากกว่าเดิม
            if (!entry.isDiscovered || fish.Weight > entry.bestWeight)
            {
                Debug.Log($"Encyclopedia: อัปเดตสถิติ {fish.Name}! น้ำหนักใหม่: {fish.Weight:F2}kg");
                
                // 3. อัปเดตข้อมูล
                entry.isDiscovered = true;
                entry.bestWeight = fish.Weight;
                
                // 4. (สำคัญ) อัปเดตรูปภาพ ถ้าปลาตัวนี้มีการถ่ายรูปมา
                if (!string.IsNullOrEmpty(fish.photoGUID))
                {
                    // (เราจะลบรูปเก่าทิ้ง เพื่อประหยัดพื้นที่)
                    DeleteOldPhoto(entry.bestPhotoGUID);
                    entry.bestPhotoGUID = fish.photoGUID;
                }
                else if (entry.isDiscovered && string.IsNullOrEmpty(entry.bestPhotoGUID))
                {
                    // เคสพิเศษ: จับได้, หนักกว่าเดิม, แต่ *ไม่ได้ถ่ายรูป*
                    // เราจะไม่ลบรูปเก่า (ถ้ามี)
                    // แต่ถ้ายังไม่เคยมีรูปเลย ก็ปล่อยว่างไว้
                }
            }
        }
    }

    // --- ฟังก์ชัน Save/Load ---

    public EncyclopediaData GetSaveData()
    {
        if (!isInitialized) InitializeDatabase();

        EncyclopediaData data = new EncyclopediaData();
        // แปลง Dictionary กลับเป็น List เพื่อให้ JsonUtility เซฟได้
        data.entries = entryDatabase.Values.ToList();
        return data;
    }

    public void LoadData(EncyclopediaData data)
    {
        if (data == null) 
        {
            Debug.Log("Encyclopedia: ไม่พบข้อมูล Save (New Game).");
            return; // ไม่ต้องทำอะไร (เริ่มเกมใหม่)
        }

        pendingLoadData = data; // เก็บข้อมูลไว้ก่อน
        TryReapplyLoadedData();
    }
    
    private void TryReapplyLoadedData()
    {
        // ฟังก์ชันนี้จะทำงานเมื่อ 1. Load เสร็จ และ 2. Database พร้อม
        if (pendingLoadData == null || !isInitialized)
            return; 

        Debug.Log("Encyclopedia: กำลังโหลดข้อมูล Save...");
        // นำข้อมูลที่เซฟไว้มาทับลงใน Dictionary
        foreach (EncyclopediaEntry entry in pendingLoadData.entries)
        {
            if (entryDatabase.ContainsKey(entry.baseFishName))
            {
                entryDatabase[entry.baseFishName] = entry;
            }
        }
        
        pendingLoadData = null; // เคลียร์ข้อมูลที่พักไว้
    }

    // --- ฟังก์ชัน Utility ---
    public Dictionary<string, EncyclopediaEntry> GetDatabase()
    {
        if (!isInitialized) InitializeDatabase();
        return entryDatabase;
    }

    private void DeleteOldPhoto(string guid)
    {
        if (string.IsNullOrEmpty(guid)) return;

        // ค้นหา Slot ปัจจุบัน (จำเป็นมาก!)
        int currentSlot = GameSession.Instance.CurrentSlotId;
        if (currentSlot <= 0) return;

        string folderPath = Path.Combine(Application.persistentDataPath, $"SlotPhotos_{currentSlot}");
        string filePath = Path.Combine(folderPath, $"{guid}.png");

        try
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                Debug.Log($"Encyclopedia: ลบรูปเก่า {guid}.png สำเร็จ");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"Encyclopedia: ลบรูปเก่าไม่สำเร็จ: {ex.Message}");
        }
    }
}