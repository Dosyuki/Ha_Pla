using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : Singleton<Inventory>
{
    [SerializeField] private List<Fish> allFish;
    [SerializeField] private FishingRod currentRod;
    public FishingRod CurrentRod => currentRod;

    [SerializeField]
    private bool _isEquipRod; // backing field

    private bool isEquipRod
    {
        get { return _isEquipRod; }
        set
        {
            _isEquipRod = value;
            if (currentRod != null)
            {
                currentRod.gameObject.SetActive(value);
            }
        }
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
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
        allFish.Add(fish);
    }
    public List<Fish> GetAllFish() => allFish;
}
