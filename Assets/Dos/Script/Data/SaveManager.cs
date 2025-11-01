// ไฟล์ใหม่: SaveManager.cs
using UnityEngine;
using System.IO;

// ใช้ Singleton.cs ที่คุณมี
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

        // 1. รวบรวมข้อมูล (Gather Data)
        data.playerStats = PlayerStats.Instance.GetSaveData();
        data.inventory = Inventory.Instance.GetSaveData();
        data.world = TimeSystem.Instance.GetSaveData();
        data.characterData = CharacterCustomize.Instance.GetSaveData(); // **เพิ่มส่วนตัวละคร**
        data.encyclopedia = EncyclopediaManager.Instance.GetSaveData();
        data.lastUpdated = System.DateTime.Now.Ticks;

        // 2. แปลงเป็น JSON
        string json = JsonUtility.ToJson(data, true);

        // 3. เขียนไฟล์
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
        Debug.Log("Load Complete!");
        return true;
    }
}