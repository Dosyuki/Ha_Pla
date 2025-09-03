using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InventoryUI : Singleton<InventoryUI>
{
    [SerializeField] private List<Fish> allFish = new List<Fish>();
    [SerializeField] private GameObject fishCardPrefab;
    [SerializeField] private Transform fishCardHolder;
    [SerializeField] private TMP_Text maxslotText;

    
    private CanvasGroup canvasGroup;
    private bool isOpen = false;
    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Update()
    {
        
    }

    public void CreateCardUI()
    {
        if(isOpen)
            return;
        int index = 0;
        allFish = Inventory.Instance.GetAllFish();
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;    
        maxslotText.text = $"{allFish.Count} / {Inventory.Instance.GetMaxSlots()}";
        isOpen = true;
        foreach (Fish fish in allFish)
        {
            CardInventoryUI cardUI = Instantiate(fishCardPrefab, fishCardHolder).GetComponent<CardInventoryUI>();
            cardUI.UpdateCardUI(fish);
            index++;
        }
    }

    public void CloseCardUI()
    {
        isOpen = false;
        
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;

        foreach (Transform child in fishCardHolder.transform)
        {
            Destroy(child.gameObject);
        }
    }
}
