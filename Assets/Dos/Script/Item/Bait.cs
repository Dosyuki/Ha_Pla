using System;
using UnityEngine;
[Serializable]
public class Bait
{
    [SerializeField] private BaseBait bait;
    [SerializeField] public int amount;
    
    public string Name;
    [TextAreaAttribute]
    public string Description;
    public float LuckMultiplier;
    public float WeightMultiplier;

    public Bait(BaseBait bait, int amount = 0)
    {
        this.bait = bait;
        Name = bait.Name;
        Description = bait.Description;
        WeightMultiplier = bait.WeightMultiplier;
        LuckMultiplier = bait.LuckMultiplier;
        this.amount = amount;
    }
}
