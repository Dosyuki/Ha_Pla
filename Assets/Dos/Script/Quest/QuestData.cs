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
    // public ItemData itemReward;

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
    public BaseFish specificFish; 
    public FishRarity requiredRarity;
    public float minWeight = 0;

    [Header("Amount")]
    public int amountRequired = 1;

    public bool IsMatch(Fish fish)
    {
        if (minWeight > 0 && fish.Weight < minWeight) return false;

        switch (type)
        {
            case RequirementType.SpecificFish:
                return fish.Name == specificFish.Name;
                
            case RequirementType.RarityOnly:
                return fish.Rarity == requiredRarity;
                
            case RequirementType.AnyFish:
                return true;
        }
        return false;
    }
}