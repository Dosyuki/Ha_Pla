using UnityEngine;

public class PlayerStats : Singleton<PlayerStats>
{
    [SerializeField] private float Money;
    
    public float GetMoney() => Money;
    public void SetMoney(float value) => Money = value;
    public void AddMoney(float value) => Money += value;
    public void RemoveMoney(float value) => Money -= value;
}
