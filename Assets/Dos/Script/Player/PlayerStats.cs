using UnityEngine;

public class PlayerStats : Singleton<PlayerStats>
{
    [SerializeField] private float Money;
    
    public float GetMoney() => Money;
    public float SetMoney(float value) => Money = value;
}
