// แก้ไขไฟล์: Dos/Script/Player/PlayerStats.cs
using UnityEngine;

public class PlayerStats : Singleton<PlayerStats>
{
    [SerializeField] private float Money;
    [SerializeField] private float LuckMultiplier = 1.0f;
    [SerializeField] private float WeightMultiplier = 1.0f;
    
    public void SetMoney(float value) => Money = value;
    public void AddMoney(float value) => Money += value;
    public void SetLuckMultiplier(float value) => LuckMultiplier = value;
    public void AddLuck(float value) => LuckMultiplier += value;
    public void RemoveLuck(float value) => LuckMultiplier -= value;
    public void  SetWeightMultiplier(float value) => WeightMultiplier = value;
    public float AddWeightMultiplier(float value) => WeightMultiplier += value;
    public float RemoveWeightMultiplier(float value) => WeightMultiplier -= value;

    public float GetMoney() => Money;
    public void RemoveMoney(float value) => Money -= value;
    public float GetLuck() => LuckMultiplier;
    public float GetWeightMultiplier() => WeightMultiplier;
    // --- ส่วนที่เพิ่มสำหรับ Save/Load ---

    public PlayerStatsData GetSaveData()
    {
        return new PlayerStatsData
        {
            money = this.Money,
            luckMultiplier = this.LuckMultiplier, 
            weightMultiplier = this.WeightMultiplier
        };
    }

    public void LoadData(PlayerStatsData data)
    {
        this.Money = data.money;
        this.LuckMultiplier = data.luckMultiplier;
        this.WeightMultiplier = data.weightMultiplier;
        // อัปเดต UI (ถ้ามี)
        if (InventoryUI.Instance != null)
            InventoryUI.Instance.UpdateText(); //
    }
}