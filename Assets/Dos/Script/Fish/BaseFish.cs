using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BaseFish", menuName = "Scriptable Objects/Fish/newFish")]
public class BaseFish : ScriptableObject
{
    public string Name;
    [TextAreaAttribute]
    public string Description;
    public float Weight;
    public float Value;
    public FishRarity Rarity = FishRarity.Basic;
    public float DropChance;
    public GameObject PrefabModel;
    public Sprite SpriteModel;
    
    public float AISpeed;
    public float DashMultiplier;
    public float DashDuration;
    public float ProgressRateIncrease;
    public float ProgressRateDecrease;
    [System.Serializable]
    public struct movementPattern
    {
        public float timeLeft;
        public float speedTime;
    }
    public List<movementPattern> movementPatterns;
}

public enum FishRarity
{
    Basic,
    Rare,
    Epic,
    Legendary
}