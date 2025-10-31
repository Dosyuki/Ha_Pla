using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : Singleton<Inventory>
{
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

    public bool isEquipRod
    {
        get { return _isEquipRod; }
        set
        {
            if(currentRod.getIsThrown() || UIManager.Instance.GetCurrentState() == currentState.UI)
                return;
            _isEquipRod = value;
            if (currentRod != null)
            {
                currentRod.gameObject.SetActive(value);
                currentRod.HideSliderCanvas(!value);
            }
        }
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        maxSlots = 20 + (currentUpgradeTier * 10);
        isEquipRod = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            isEquipRod = !isEquipRod;
        }
    }

    public void UpdateCurrentBait()
    {
        if(currentBait != null)
            baitInventoryUI.UpdateCardUI(currentBait,currentBait.amount);
    }
    
    public void AddFish(Fish fish)
    {
        if (!isMaxFish)
            allFish.Add(fish);
    }

    public void RemoveFish(Fish fish)
    {
        allFish.Remove(fish);
    }

    public void UpgradeTier()
    {
        if(PlayerStats.Instance.GetMoney() < Inventory.Instance.UpgradeCost())
            return;
        currentUpgradeTier++;
        maxSlots = 20 + (currentUpgradeTier * 10);
    }

    public void AddBait(BaseBait baseBait, int amount = 1)
    {
        Bait existing = allBait.Find(b => b.Name == baseBait.Name);

        if (existing == null)
        {
            // Create a new entry
            Bait newBait = new Bait(baseBait, amount);
            allBait.Add(newBait);
            currentBait = newBait;
            return;
        }
        else
        {
            // Add to existing
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

        // แปลง List<Fish> (Runtime) เป็น List<FishData> (Save)
        data.allFish = new List<FishData>();
        foreach (Fish fish in allFish)
        {
            data.allFish.Add(new FishData(fish));
        }

        // แปลง List<Bait> (Runtime) เป็น List<BaitData> (Save)
        data.allBait = new List<BaitData>();
        foreach (Bait bait in allBait)
        {
            data.allBait.Add(new BaitData(bait));
        }
        
        return data;
    }

    public void LoadData(InventoryData data)
    {
        // 1. โหลดข้อมูลพื้นฐาน
        this.maxSlots = data.maxSlots;
        this.currentUpgradeTier = data.currentUpgradeTier;

        // 2. ล้างข้อมูลเก่า
        allFish.Clear();
        allBait.Clear();

        // 3. สร้าง List<Fish> คืนจาก List<FishData>
        FishManager fishDB = FishManager.Instance;
        foreach (FishData fishData in data.allFish)
        {
            BaseFish baseFish = fishDB.GetBaseFishByName(fishData.baseFishName);
            if (baseFish != null)
            {
                // สร้าง Fish Instance ใหม่
                // เราต้องหา Dummy Bait มาใส่ใน constructor ชั่วคราว
                // หรือปรับ Constructor ของ Fish ให้ยืดหยุ่นกว่านี้
                
                // (สมมติว่าคุณมีเหยื่อ "baitA" เป็นเหยื่อพื้นฐาน)
                BaseBait dummyBaseBait = BaitManager.Instance.GetBaseBaitByName("baitA");
                if (dummyBaseBait == null) {
                    Debug.LogError("Dummy bait 'baitA' not found for loading fish!");
                    continue;
                }
                
                Bait dummyBait = new Bait(dummyBaseBait, 0); //
                
                //
                Fish newFish = new Fish(baseFish, 1f, 1f, dummyBait); // สร้างด้วยค่า dummy
                newFish.Weight = fishData.weight; // **Override น้ำหนักที่ Save ไว้**
                allFish.Add(newFish);
            }
        }

        // 4. สร้าง List<Bait> คืนจาก List<BaitData>
        BaitManager baitDB = BaitManager.Instance;
        foreach (BaitData baitData in data.allBait)
        {
            BaseBait baseBait = baitDB.GetBaseBaitByName(baitData.baseBaitName);
            if (baseBait != null)
            {
                Bait newBait = new Bait(baseBait, baitData.amount); //
                allBait.Add(newBait);

                // ตั้งค่าเหยื่อที่สวมใส่
                if (baitData.baseBaitName == data.currentBaitName)
                {
                    // อย่าใช้ property "currentBait" ตรงๆ ตอน Load เพราะมันอาจจะเรียก UpdateCurrentBait() เร็วไป
                    _currentBait = newBait; 
                }
            }
        }
        
        // อัปเดต UI ตอนสุดท้าย
        UpdateCurrentBait(); //
        Debug.Log("Inventory loaded.");
    }

    public List<Fish> GetAllFish() => allFish;
    public bool isMaxFish => allFish.Count + 1 > maxSlots;
    public int GetMaxSlots() => maxSlots;
    public int UpgradeCost() => (int)((currentUpgradeTier * 1.5f) * 100);
    public List<Bait> GetAllBait() => allBait;

}
