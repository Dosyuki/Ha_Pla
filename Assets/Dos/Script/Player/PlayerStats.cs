// แก้ไขไฟล์: Dos/Script/Player/PlayerStats.cs
using UnityEngine;

public class PlayerStats : Singleton<PlayerStats>
{
    [SerializeField] private float Money;
    
    public float GetMoney() => Money;
    public void SetMoney(float value) => Money = value;
    public void AddMoney(float value) => Money += value;
    public void RemoveMoney(float value) => Money -= value;

    // --- ส่วนที่เพิ่มสำหรับ Save/Load ---

    public PlayerStatsData GetSaveData()
    {
        return new PlayerStatsData
        {
            money = this.Money
        };
    }

    public void LoadData(PlayerStatsData data)
    {
        this.Money = data.money;
        
        // อัปเดต UI (ถ้ามี)
        if (InventoryUI.Instance != null)
            InventoryUI.Instance.UpdateText(); //
    }
}