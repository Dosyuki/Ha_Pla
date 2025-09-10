using UnityEngine;

[System.Serializable]
public class Fish
{
    public string Name;
    public string Description;
    public float Weight;
    public float Value;
    public FishRarity Rarity;
    public GameObject PrefabModel;
    public Sprite SpriteModel;
    
    [SerializeField] private BaseFish baseFish;

    public Fish(BaseFish baseFish, float luckMultiplier, float weightMultiplier,Bait bait)
    {
        this.baseFish = baseFish;
        Name = baseFish.Name;
        Description = baseFish.Description;
        Rarity = baseFish.Rarity;
        Value = baseFish.Value;
        PrefabModel = baseFish.PrefabModel;
        SpriteModel = baseFish.SpriteModel;

        float minWeight = baseFish.Weight * 0.8f * weightMultiplier * bait.WeightMultiplier;
        float maxWeight = baseFish.Weight * 1.2f * weightMultiplier * bait.WeightMultiplier;
        Weight = Random.Range(minWeight, maxWeight);

        if (Random.value < 0.05f * luckMultiplier * bait.LuckMultiplier && Rarity < FishRarity.Legendary)
        {
            Rarity += 1; 
        }
    }

    public float CalculateValue()
    {
        return Value * (Weight / baseFish.Weight);
    }
}