// เปิดไฟล์: Dos/Script/Fish/Fish.cs
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
    
    public string photoGUID;

    [SerializeField] private BaseFish baseFish;

    public Fish(BaseFish baseFish, float luckMultiplier, float weightMultiplier, Bait bait)
    {
        // ... (โค้ด Constructor ของคุณเหมือนเดิม) ...
        this.baseFish = baseFish;
        Name = baseFish.Name;
        Description = baseFish.Description;
        Rarity = baseFish.Rarity;
        Value = baseFish.Value;
        PrefabModel = baseFish.PrefabModel;
        SpriteModel = baseFish.SpriteModel;
        AISpeed = baseFish.AISpeed;
        DashMultiplier = baseFish.DashMultiplier;
        DashDuration = baseFish.DashDuration;
        ProgressRateIncrease = baseFish.ProgressRateIncrease;
        ProgressRateDecrease = baseFish.ProgressRateDecrease;

        float[] dynamicCon = TimeSystem.Instance.GetDynamicCondition();
        PlayerStats py  = PlayerStats.Instance;

        float minWeight = baseFish.Weight * 0.8f *
                          (1 + weightMultiplier + bait.WeightMultiplier + dynamicCon[1] + py.GetWeightMultiplier());
        float maxWeight = baseFish.Weight * 1.5f *
                          (1 + weightMultiplier + bait.WeightMultiplier + dynamicCon[1] + py.GetWeightMultiplier());
        Weight = Random.Range(minWeight, maxWeight);
    }


    public float CalculateValue()
    {
        return Value * (Weight / baseFish.Weight);
    }
    public BaseFish GetBaseFish() => baseFish;
}