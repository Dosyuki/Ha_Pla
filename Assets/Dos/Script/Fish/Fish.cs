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
    public float AISpeed;
    public float DashMultiplier;
    public float DashDuration;
    public float ProgressRateIncrease;
    public float ProgressRateDecrease;
    
    [SerializeField] private BaseFish baseFish;

    public Fish(BaseFish baseFish, float luckMultiplier, float weightMultiplier, Bait bait)
    {
        this.baseFish = baseFish;
        Name = baseFish.Name;
        Description = baseFish.Description;
        Rarity = baseFish.Rarity; // รับค่าตรงจาก BaseFish
        Value = baseFish.Value;
        PrefabModel = baseFish.PrefabModel;
        SpriteModel = baseFish.SpriteModel;
        AISpeed = baseFish.AISpeed;
        DashMultiplier = baseFish.DashMultiplier;
        DashDuration = baseFish.DashDuration;
        ProgressRateIncrease = baseFish.ProgressRateIncrease;
        ProgressRateDecrease = baseFish.ProgressRateDecrease;

        float[] dynamicCon = TimeSystem.Instance.GetDynamicCondition();

        float minWeight = baseFish.Weight * 0.8f * weightMultiplier * bait.WeightMultiplier * dynamicCon[1];
        float maxWeight = baseFish.Weight * 1.2f * weightMultiplier * bait.WeightMultiplier * dynamicCon[1];
        Weight = Random.Range(minWeight, maxWeight);
    }


    public float CalculateValue()
    {
        return Value * (Weight / baseFish.Weight);
    }
}