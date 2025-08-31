using UnityEngine;

[System.Serializable]
public class Fish
{
    public string Name;
    public string Description;
    public float Weight;
    public FishRarity Rarity;
    public GameObject PrefabModel;
    public Sprite SpriteModel;

    // Constructor builds from BaseFish with multipliers
    public Fish(BaseFish baseFish, float luckMultiplier, float weightMultiplier)
    {
        Name = baseFish.Name;
        Description = baseFish.Description;
        Rarity = baseFish.Rarity;
        PrefabModel = baseFish.PrefabModel;
        SpriteModel = baseFish.SpriteModel;

        // Example randomization:
        float minWeight = baseFish.Weight * 0.8f * weightMultiplier;
        float maxWeight = baseFish.Weight * 1.2f * weightMultiplier;
        Weight = Random.Range(minWeight, maxWeight);

        // Luck could shift rarities or weight slightly
        if (Random.value < 0.05f * luckMultiplier && Rarity < FishRarity.Legendary)
        {
            Rarity += 1; // upgrade rarity by one tier
        }
    }
}