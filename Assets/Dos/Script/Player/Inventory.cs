using System;
using System.Collections.Generic;
using UnityEngine;

// Singleton `Inventory` นี้จะจัดการทั้ง "ข้อมูล" (Data) และ "การสั่งเปิด/ปิด" UI (View Logic)
public class Inventory : Singleton<Inventory>
{
    [Header("Data")]
    [SerializeField] private List<Fish> allFish;
    [SerializeField] private List<Bait> allBait = new List<Bait>();
    [SerializeField] private FishingRod currentRod;
    private Bait _currentBait;

    [SerializeField]
    public Bait currentBait
    {
        get => _currentBait;
        set
        {
            _currentBait = value;
            UpdateCurrentBait();
        }
    }
    [SerializeField] private BaitInventoryUI baitInventoryUI;
    [SerializeField] private int maxSlots;
    public FishingRod CurrentRod => currentRod;

    [SerializeField] private bool _isEquipRod; // backing field
    private int currentUpgradeTier = 1;

    [Header("State")]
    [Tooltip("สถานะว่า Inventory UI เปิดอยู่หรือไม่ (ควบคุมโดยสคริปต์นี้)")]
    private bool isInventoryOpen = false;

    public bool isEquipRod
    {
        get { return _isEquipRod; }
        set
        {
            // ตรรกะเดิม: ห้ามสลับเบ็ดถ้ากำลังขว้าง หรือ UI เปิดอยู่
            if(currentRod.getIsThrown() || UIManager.Instance.GetCurrentState() == currentState.UI)
                return;
            _isEquipRod = value;
            if (currentRod != null)
            {
                currentRod.gameObject.SetActive(value);
                //currentRod.HideSliderCanvas(!value);
            }
        }
    }

    void Start()
    {
        maxSlots = 20 + (currentUpgradeTier * 10);
        isEquipRod = true;
        isInventoryOpen = false; 
        InventoryUI.Instance.UpdateUpgradeCostText(UpgradeCost());
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Tab) && !isInventoryOpen && 
            UIManager.Instance.GetCurrentState() == currentState.None)
        {
            // (ห้ามเปิดถ้ากำลังขว้างเบ็ด)
            if (currentRod.getIsThrown())
                return; 
            
            InventoryUI.Instance.CreateCardUI(true);
            isInventoryOpen = true;
            return; // จบการทำงานในเฟรมนี้
        }


        if ((Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Tab)) && isInventoryOpen)
        {
            // ปิด Inventory (ใช้ `InventorySource.Ship` เหมือนกับ `ShipStorage`)
            InventoryUI.Instance.CloseCardUI(InventorySource.Ship);
            isInventoryOpen = false;
            return; // จบการทำงานในเฟรมนี้
        }
        

        if (isInventoryOpen && Input.GetKeyDown(KeyCode.R) && 
            PlayerStats.Instance.GetMoney() >= Inventory.Instance.UpgradeCost())
        {
            PlayerStats.Instance.SetMoney(PlayerStats.Instance.GetMoney() - Inventory.Instance.UpgradeCost());
            UpgradeTier();
            InventoryUI.Instance.UpdateText();
        }
    }

    // --- (ฟังก์ชันอื่นๆ ทั้งหมดเหมือนเดิม) ---

    public void UpdateCurrentBait()
    {
        if (EquippedBaitSlot.Instance != null)
        {
            // และสั่งให้มันอัปเดตหน้าตาตามเหยื่อปัจจุบัน (ไม่ว่าจะเป็น null หรือไม่)
            EquippedBaitSlot.Instance.UpdateEquippedBait(currentBait);
        }
    }
    
    public void AddFish(Fish fish)
    {
        if (!isMaxFish)
            allFish.Add(fish);
        if (fish != null)
        {
            // (โค้ด Encyclopedia ของคุณยังอยู่ครบ)
            EncyclopediaManager.Instance.SubmitFish(fish);
        }
    }

    public void RemoveFish(Fish fish)
    {
        allFish.Remove(fish);
    }

    public void UpgradeTier()
    {
        if(PlayerStats.Instance.GetMoney() < Inventory.Instance.UpgradeCost())
            return;
        InventoryUI.Instance.UpdateUpgradeCostText(UpgradeCost());
        currentUpgradeTier++;
        maxSlots = 20 + (currentUpgradeTier * 10);
        
        if (isInventoryOpen)
        {
            InventoryUI.Instance.UpdateText();
        }
    }

    public void AddBait(BaseBait baseBait, int amount = 1)
    {
        Bait existing = allBait.Find(b => b.Name == baseBait.Name);

        if (existing == null)
        {
            Bait newBait = new Bait(baseBait, amount);
            allBait.Add(newBait);
            currentBait = newBait;
            return;
        }
        else
        {
            existing.amount += amount;
        }
        currentBait = existing;
    }
    
    public InventoryData GetSaveData()
    {
        InventoryData data = new InventoryData();
        data.maxSlots = this.maxSlots;
        data.currentUpgradeTier = this.currentUpgradeTier;
        data.currentBaitName = (currentBait != null) ? currentBait.Name : string.Empty;

        data.allFish = new List<FishData>();
        foreach (Fish fish in allFish)
        {
            data.allFish.Add(new FishData(fish));
        }

        data.allBait = new List<BaitData>();
        foreach (Bait bait in allBait)
        {
            data.allBait.Add(new BaitData(bait));
        }
        
        return data;
    }

    public void LoadData(InventoryData data)
    {
        this.maxSlots = data.maxSlots;
        this.currentUpgradeTier = data.currentUpgradeTier;

        allFish.Clear();
        allBait.Clear();

        FishManager fishDB = FishManager.Instance;
        foreach (FishData fishData in data.allFish)
        {
            BaseFish baseFish = fishDB.GetBaseFishByName(fishData.baseFishName);
            if (baseFish != null)
            {
                BaseBait dummyBaseBait = BaitManager.Instance.GetBaseBaitByName("baitA");
                if (dummyBaseBait == null) {
                    Debug.LogError("Dummy bait 'baitA' not found for loading fish!");
                    continue;
                }
                Bait dummyBait = new Bait(dummyBaseBait, 0);
                
                Fish newFish = new Fish(baseFish, 1f, 1f, dummyBait);
                newFish.Weight = fishData.weight; 
                allFish.Add(newFish);
            }
        }

        BaitManager baitDB = BaitManager.Instance;
        foreach (BaitData baitData in data.allBait)
        {
            BaseBait baseBait = baitDB.GetBaseBaitByName(baitData.baseBaitName);
            if (baseBait != null)
            {
                Bait newBait = new Bait(baseBait, baitData.amount);
                allBait.Add(newBait);
                if (baitData.baseBaitName == data.currentBaitName)
                {
                    _currentBait = newBait; 
                }
            }
        }
        
        UpdateCurrentBait();
        Debug.Log("Inventory loaded.");
    }

    public List<Fish> GetAllFish() => allFish;
    public bool isMaxFish => allFish.Count + 1 > maxSlots;
    public int GetMaxSlots() => maxSlots;
    public int UpgradeCost() => (int)((currentUpgradeTier * 1.5f) * 100);
    public List<Bait> GetAllBait() => allBait;
}