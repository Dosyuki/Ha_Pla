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
    private bool hasClaimedZoneAReward = false;
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
// --- *** ฟังก์ชันใหม่ *** ---
    /// <summary>
    /// ตรวจสอบว่าปลาใน Zone A ครบหรือยัง และให้รางวัลถ้ายังไม่เคยรับ
    /// </summary>
    public void CheckForZoneACompletion()
    {
        // 1. ถ้ายังไม่พร้อม หรือ รับรางวัลไปแล้ว -> ไม่ต้องทำอะไร
        if (!isInitialized || hasClaimedZoneAReward)
        {
            return;
        }

        // 2. ตรวจสอบปลา "ทุกตัว" ในฐานข้อมูล
        foreach (EncyclopediaEntry entry in entryDatabase.Values)
        {
            // ถ้าเจอตัวไหนที่ยังไม่ถูกค้นพบ (isDiscovered == false)
            if (!entry.isDiscovered)
            {
                // ยังไม่ครบ -> ออกจากฟังก์ชัน
                return; 
            }
        }

        // 3. ถ้าโค้ดวิ่งมาถึงตรงนี้ได้ = "ยินดีด้วย คุณเก็บครบแล้ว!"
        Debug.Log("สารานุกรม Zone A ครบแล้ว! กำลังมอบรางวัล...");
        
        // 4. ตั้งธงว่า "รับรางวัลแล้ว" (สำคัญมาก!)
        hasClaimedZoneAReward = true;
        
        // 5. เรียกฟังก์ชันให้รางวัล
        GiveZoneAReward();
    }

    // --- *** ฟังก์ชันใหม่ *** ---
    private void GiveZoneAReward()
    {
        // --- นี่คือส่วนที่คุณใส่รางวัลได้เลย ---

        // ตัวอย่าง: ให้เงิน 1000 Fishlars
        if (PlayerStats.Instance != null)
        {
            PlayerStats.Instance.AddWeightMultiplier(0.2f);
            Debug.Log("REWARD: ได้รับ 1000 Fishlars!");
        }

        // ตัวอย่าง: ให้เหยื่อพิเศษ 5 ชิ้น (เช่น baitD)
        if (BaitManager.Instance != null && Inventory.Instance != null)
        {
            // (เราต้องมีฟังก์ชัน GetBaseBaitByName ใน BaitManager ก่อน)
            BaseBait specialBait = BaitManager.Instance.GetBaseBaitByName("baitD");
            if (specialBait != null)
            {
                Inventory.Instance.AddBait(specialBait, 5); //
                Debug.Log("REWARD: ได้รับ BaitD 5 ชิ้น!");
            }
        }

        // (คุณสามารถเรียก UI Popup แสดงความยินดีได้ที่นี่)
    }
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
        
        if (entryDatabase.TryGetValue(fish.Name, out EncyclopediaEntry entry))
        {
            bool wasAlreadyDiscovered = entry.isDiscovered; // เก็บสถานะ "ก่อน" อัปเดต
            
            if (!entry.isDiscovered || fish.Weight > entry.bestWeight)
            {
                Debug.Log($"Encyclopedia: อัปเดตสถิติ {fish.Name}! น้ำหนักใหม่: {fish.Weight:F2}kg");
                
                entry.isDiscovered = true;
                entry.bestWeight = fish.Weight;
                
                if (!string.IsNullOrEmpty(fish.photoGUID))
                {
                    DeleteOldPhoto(entry.bestPhotoGUID);
                    entry.bestPhotoGUID = fish.photoGUID;
                }
            }
            
            // --- *** เพิ่มส่วนนี้ *** ---
            // ถ้าปลาตัวนี้ "เพิ่งจะถูกค้นพบ" (ก่อนหน้านี้เป็น false)
            // ให้เราตรวจสอบว่าครบหรือยัง
            if (!wasAlreadyDiscovered && entry.isDiscovered)
            {
                CheckForZoneACompletion();
            }
            // --- *** จบส่วนที่เพิ่ม *** ---
        }
    }

    // --- ฟังก์ชัน Save/Load ---

    public EncyclopediaData GetSaveData()
    {
        if (!isInitialized) InitializeDatabase();

        EncyclopediaData data = new EncyclopediaData();
        data.entries = entryDatabase.Values.ToList();
        data.hasClaimedZoneAReward = this.hasClaimedZoneAReward; // <-- *** เพิ่มบรรทัดนี้ ***
        return data;
    }

    public void LoadData(EncyclopediaData data)
    {
        if (data == null) 
        {
            Debug.Log("Encyclopedia: ไม่พบข้อมูล Save (New Game).");
            this.hasClaimedZoneAReward = false; // <-- *** เพิ่มบรรทัดนี้ ***
            return;
        }

        pendingLoadData = data;
        this.hasClaimedZoneAReward = data.hasClaimedZoneAReward; // <-- *** เพิ่มบรรทัดนี้ ***
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