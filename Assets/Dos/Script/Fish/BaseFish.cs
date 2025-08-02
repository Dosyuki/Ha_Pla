using UnityEngine;

[CreateAssetMenu(fileName = "BaseFish", menuName = "Scriptable Objects/Fish/newFish")]
public class BaseFish : ScriptableObject
{
    public string Name;
    [TextAreaAttribute]
    public string Description;
    public float Weight;
    public FishRarity Rarity = FishRarity.Basic;
    public float DropChance;
    public GameObject PrefabModel;
    public Sprite SpriteModel;
}

public enum FishRarity
{
    Basic,
    Rare,
    Epic,
    Legendary
}