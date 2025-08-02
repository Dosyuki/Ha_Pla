using UnityEngine;

public  class BaseItem :  MonoBehaviour
{
    public string Name;
    [TextAreaAttribute]
    public string Description;
    public float LuckMultiplier;
    public float WeightMultiplier;
    public GameObject Prefab;   
}
