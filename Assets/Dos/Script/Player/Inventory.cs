using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : Singleton<Inventory>
{
    [SerializeField] private List<Fish> allFish;
    [SerializeField] private List<Bait> allBait = new List<Bait>();
    [SerializeField] private FishingRod currentRod;
    [SerializeField] public Bait currentBait;
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
        maxSlots = 5 + (currentUpgradeTier * 5);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            isEquipRod = !isEquipRod;
        }
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
        currentUpgradeTier++;
        maxSlots = 5 + (currentUpgradeTier * 5);
    }

    public void AddBait(BaseBait baseBait, int amount = 1)
    {
        Bait existing = allBait.Find(b => b.Name == baseBait.Name);

        if (existing == null)
        {
            // Create a new entry
            Bait newBait = new Bait(baseBait, amount);
            allBait.Add(newBait);
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
}
