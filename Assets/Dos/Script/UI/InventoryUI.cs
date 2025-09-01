using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private List<Fish> allFish = new List<Fish>();
    [SerializeField] private GameObject fishCardPrefab;
    [SerializeField] private List<Transform> fishCardHolder = new List<Transform>();

    private void Start()
    {
        CreateCardUI();
    }

    private void Update()
    {
        
    }

    private void CreateCardUI()
    {
        int index = 0;
        allFish = Inventory.Instance.GetAllFish();
        foreach (Fish fish in allFish)
        {
            CardInventoryUI cardUI = Instantiate(fishCardPrefab, fishCardHolder[index % 3]).GetComponent<CardInventoryUI>();
            cardUI.UpdateCardUI(fish);
            index++;
        }
    }
}
