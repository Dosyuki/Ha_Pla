using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewQuest", menuName = "Quest/Quest Data")]
public class QuestData : ScriptableObject
{
    [Header("Quest Info")]
    public string questTitle;
    [TextArea] public string description;
    
    [Header("Requirements")]
    public List<QuestRequirement> requirements;

    [Header("Rewards")]
    public float moneyReward;
    // public ItemData itemReward; // ใส่เพิ่มได้ถ้ามีระบบ Item

    // ฟังก์ชันเช็คว่าปลาตัวนี้ "เข้าข่าย" ที่จะโชว์ใน Filter ไหม
    public bool IsFishCompatible(Fish fish)
    {
        foreach (var req in requirements)
        {
            if (req.IsMatch(fish)) return true;
        }
        return false;
    }
}

[System.Serializable]
public class QuestRequirement
{
    public enum RequirementType { SpecificFish, RarityOnly, AnyFish }
    public RequirementType type;

    [Header("Conditions")]
    public BaseFish specificFish; // เจาะจงพันธุ์
    public FishRarity requiredRarity; // เจาะจงระดับ
    public float minWeight = 0; // ** เพิ่มเงื่อนไขน้ำหนัก (0 = ไม่สน) **

    [Header("Amount")]
    public int amountRequired = 1;

    // Logic ตรวจสอบเงื่อนไข
    public bool IsMatch(Fish fish)
    {
        // 1. เช็ค Weight ก่อนเลย (ถ้าตั้งไว้ > 0)
        if (minWeight > 0 && fish.Weight < minWeight) return false;

        // 2. เช็คตามประเภท
        switch (type)
        {
            case RequirementType.SpecificFish:
                // เช็คชื่อ หรือ BaseFish Reference
                return fish.Name == specificFish.Name;
                
            case RequirementType.RarityOnly:
                return fish.Rarity == requiredRarity;
                
            case RequirementType.AnyFish:
                return true;
        }
        return false;
    }
}