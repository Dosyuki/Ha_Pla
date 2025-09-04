using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : Singleton<Inventory>
{
    [SerializeField] private List<Fish> allFish;
    [SerializeField] private FishingRod currentRod;
    [SerializeField] private int maxSlots;
    public FishingRod CurrentRod => currentRod;

    [SerializeField]
    private bool _isEquipRod; // backing field

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
        if(!isMaxFish)
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
    public List<Fish> GetAllFish() => allFish;
    public bool isMaxFish => allFish.Count + 1 > maxSlots;
    public int GetMaxSlots() => maxSlots;
    public int UpgradeCost() => (int)((currentUpgradeTier * 1.5f) * 100);
}
