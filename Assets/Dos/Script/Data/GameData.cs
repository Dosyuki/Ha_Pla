// ไฟล์ใหม่: GameData.cs
using System.Collections.Generic;

[System.Serializable]
public class GameData
{
    public string saveSlotName;
    public long lastUpdated;
    public PlayerStatsData playerStats;
    public InventoryData inventory;
    public WorldData world;
    public CharacterCustomizeData characterData; // เพิ่มส่วนของ Customization
    public EncyclopediaData encyclopedia;
}

// ไฟล์ใหม่: PlayerStatsData.cs
[System.Serializable]
public class PlayerStatsData
{
    public float money;
    public float luckMultiplier; 
    public float weightMultiplier;
    // คุณสามารถเพิ่ม Vector3 playerPosition ที่นี่ได้ถ้าต้องการ Save ตำแหน่ง
}

// ----------------------------------------

// ไฟล์ใหม่: CharacterCustomizeData.cs
[System.Serializable]
public class CharacterCustomizeData
{
    public string hairMaterialName;
    public string skinMaterialName;
    public string clothMaterialName;
}

// ----------------------------------------

// ไฟล์ใหม่: InventoryData.cs
[System.Serializable]
public class InventoryData
{
    public List<FishData> allFish;
    public List<BaitData> allBait;
    public string currentBaitName;
    public int maxSlots;
    public int currentUpgradeTier;
}

// ----------------------------------------

// ไฟล์ใหม่: WorldData.cs
[System.Serializable]
public class WorldData
{
    public float timeOfDay;
}

// ----------------------------------------

// ไฟล์ใหม่: FishData.cs
[System.Serializable]
public class FishData
{
    public string baseFishName; // ID อ้างอิง
    public float weight;       // ข้อมูลที่เปลี่ยน
    public string photoGUID;
    public FishData(Fish fish)
    {
        baseFishName = fish.Name; // อ้างอิงจากคลาส Fish
        weight = fish.Weight;
    }
}

// ----------------------------------------

// ไฟล์ใหม่: BaitData.cs
[System.Serializable]
public class BaitData
{
    public string baseBaitName; // ID อ้างอิง
    public int amount;         // ข้อมูลที่เปลี่ยน

    public BaitData(Bait bait)
    {
        baseBaitName = bait.Name; // อ้างอิงจากคลาส Bait
        amount = bait.amount;
    }
}