// ไฟล์ใหม่: SaveManager.cs
using UnityEngine;
using System.IO;

public class SaveManager : Singleton<SaveManager> 
{
    private string GetSavePath(int slotId)
    {
        return Path.Combine(Application.persistentDataPath, $"SaveSlot_{slotId}.json");
    }
    public GameData PeekSaveData(int slotId)
    {
        string path = GetSavePath(slotId);
        if (!File.Exists(path))
        {
            return null; // คืนค่า null ถ้าไม่มีไฟล์ (เป็น "New Save")
        }

        try
        {
            // อ่านไฟล์ JSON
            string json = File.ReadAllText(path);
            
            // แปลงกลับเป็น GameData แต่ "ไม่" สั่ง Load
            GameData data = JsonUtility.FromJson<GameData>(json);
            return data;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error peeking save file {slotId}: {ex.Message}");
            return null; // ถ้าไฟล์เสีย ก็ให้เป็น New Save ไป
        }
    }
    public void SaveGame(int slotId)
    {
        Debug.Log($"Saving Game to Slot {slotId}...");
        GameData data = new GameData();


        GameData existingData = PeekSaveData(slotId);
        if (existingData != null)
        {
            data.saveSlotName = existingData.saveSlotName;
        }
        else
        {
            data.saveSlotName = "Save Data"; 
        }

        data.playerStats = PlayerStats.Instance.GetSaveData();
        data.inventory = Inventory.Instance.GetSaveData();
        data.world = TimeSystem.Instance.GetSaveData();
        data.characterData = CharacterCustomize.Instance.GetSaveData();
        data.encyclopedia = EncyclopediaManager.Instance.GetSaveData();
        data.lastUpdated = System.DateTime.Now.Ticks;

        // 3. แปลงเป็น JSON และเขียนไฟล์ (เหมือนเดิม)
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(GetSavePath(slotId), json);
        Debug.Log("Save Complete!");
    }

    public bool LoadGame(int slotId)
    {
        string path = GetSavePath(slotId);
        if (!File.Exists(path))
        {
            Debug.LogError($"Save file not found: {path}");
            return false;
        }

        Debug.Log($"Loading Game from Slot {slotId}...");
        // 1. อ่านไฟล์
        string json = File.ReadAllText(path);

        // 2. แปลงกลับเป็น Class
        GameData data = JsonUtility.FromJson<GameData>(json);

        // 3. แจกจ่ายข้อมูล (Apply Data)
        PlayerStats.Instance.LoadData(data.playerStats);
        Inventory.Instance.LoadData(data.inventory);
        TimeSystem.Instance.LoadData(data.world);
        CharacterCustomize.Instance.LoadData(data.characterData); // **เพิ่มส่วนตัวละคร**
        EncyclopediaManager.Instance.LoadData(data.encyclopedia);
        EncyclopediaManager.Instance.LoadData(data.encyclopedia);
        Debug.Log("Load Complete!");
        return true;
    }
    public void CreateNewSaveFile(int slotId, string slotName)
    {
        Debug.Log($"กำลังสร้างไฟล์เซฟใหม่สำหรับ Slot {slotId} ชื่อ: {slotName}");

        // 1. สร้างโครงข้อมูล GameData เปล่าๆ
        GameData data = new GameData();

        // 2. ตั้งชื่อเซฟ
        data.saveSlotName = slotName;
        
        data.playerStats = new PlayerStatsData 
        { 
            money = 50, // (ตัวอย่าง: เงินเริ่มต้น)
            luckMultiplier = 1, 
            weightMultiplier = 1 
        };
        data.inventory = new InventoryData { maxSlots = 20 + (1 * 10), currentUpgradeTier = 1 };
        data.encyclopedia = new EncyclopediaData();
        data.characterData = new CharacterCustomizeData(); 
        data.world = new WorldData { timeOfDay = 8.0f }; // (ตัวอย่าง: เริ่ม 8 โมงเช้า)

        // 4. แปลงเป็น JSON และเขียนไฟล์
        try
        {
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(GetSavePath(slotId), json);
            Debug.Log("สร้างไฟล์เซฟเริ่มต้นสำเร็จ!");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error creating initial save file: {ex.Message}");
        }
    }
}