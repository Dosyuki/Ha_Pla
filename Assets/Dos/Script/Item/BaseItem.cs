using UnityEngine;

public abstract class BaseItem :  MonoBehaviour
{
    public string Name;
    [TextAreaAttribute]
    public string Description;
    public float LuckMultiplier;
    public float WeightMultiplier;
    public GameObject Prefab;   
}
