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

    private bool isEquipRod
    {
        get { return _isEquipRod; }
        set
        {
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


    public List<Fish> GetAllFish() => allFish;
    public bool isMaxFish => allFish.Count + 1 > maxSlots;
    public int GetMaxSlots() => maxSlots;
    public int UpgradeCost() => (int)((currentUpgradeTier * 1.5f) * 100);
    public List<Bait> GetAllBait() => allBait;

}
