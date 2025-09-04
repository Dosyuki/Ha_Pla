using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : Singleton<InventoryUI>
{
    [SerializeField] private List<Fish> allFish = new List<Fish>();
    [SerializeField] private GameObject fishCardPrefab;
    [SerializeField] private Transform fishCardHolder;
    [SerializeField] private TMP_Text maxslotText;

    
    private CanvasGroup canvasGroup;
    private bool isOpen = false;
    private InventorySource inventorySource;
    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Update()
    {
        
    }

    public void CreateCardUI(bool openFromShip)
    {
        if(isOpen)
            return;
        inventorySource = openFromShip ? InventorySource.Ship : InventorySource.Shop;
        isOpen = true;
        UIManager.Instance.ChangeState(currentState.UI);
        int index = 0;
        allFish = Inventory.Instance.GetAllFish();
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;    
        canvasGroup.blocksRaycasts = true;
        UpdateText();
        foreach (Fish fish in allFish)
        {
            CardInventoryUI cardUI = Instantiate(fishCardPrefab, fishCardHolder).GetComponent<CardInventoryUI>();
            cardUI.UpdateCardUI(fish);
            if(openFromShip)
                cardUI.GetComponent<Button>().enabled = false;
            else
                cardUI.GetComponent<Button>().enabled = true;
            index++;
        }
    }

    public void CloseCardUI(InventorySource caller = InventorySource.None)
    {
        if(caller == InventorySource.None || caller != inventorySource)
            return;
        Debug.Log("closeInventoryUI" );
        isOpen = false;
        UIManager.Instance.ChangeState(currentState.None);
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        foreach (Transform child in fishCardHolder.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void UpdateText()
    {
        maxslotText.text = $"{allFish.Count} / {Inventory.Instance.GetMaxSlots()}";
    }
}
public enum InventorySource
{
    None,
    Shop,
    Ship
}