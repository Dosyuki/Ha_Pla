using UnityEngine;
[CreateAssetMenu(fileName = "New Item", menuName = "Bait")]
public class BaseBait : ScriptableObject
{
    public string Name;
    public Sprite Sprite;
    [TextAreaAttribute]
    public string Description;

    public float Value;
    public float LuckMultiplier;
    public float WeightMultiplier;
    
}
