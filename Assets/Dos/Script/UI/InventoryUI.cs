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
    [SerializeField] private TMP_Text moneyText;
    [SerializeField] private TMP_Text upgradeCostText;

    [SerializeField] public CanvasGroup fishGroup;
    [SerializeField] public CanvasGroup characterGroup;
    
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

    public void UpdateUpgradeCostText(float cost)
    {
        upgradeCostText.text = $"{cost:F2} to Upgrade";
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
        fishGroup.alpha = 1;
        fishGroup.interactable = true;
        fishGroup.blocksRaycasts = true;
        if (openFromShip)
        {
            characterGroup.alpha = 1;
            characterGroup.interactable = true;
            characterGroup.blocksRaycasts = true;
        }
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
        BaitInventoryPanel.Instance.CloseBaitUI();
        BaitTooltip.Instance.Hide();
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        characterGroup.alpha = 0;
        characterGroup.interactable = false;
        characterGroup.blocksRaycasts = false;

        foreach (Transform child in fishCardHolder.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void UpdateText()
    {
        maxslotText.text = $"{allFish.Count} / {Inventory.Instance.GetMaxSlots()}";
        moneyText.text = $"{PlayerStats.Instance.GetMoney():F2} Fishlars";
    }

    public void RefreshAllCardVisuals()
    {
        // 1. ถาม ShopManager ว่าตอนนี้กำลังเลือกปลาตัวไหนอยู่บ้าง
        List<Fish> currentSelection = ShopManager.Instance.GetSelectedFishList();

        // 2. วนลูป Card ทุกใบที่อยู่ใน fishCardHolder
        foreach (Transform child in fishCardHolder.transform)
        {
            CardInventoryUI card = child.GetComponent<CardInventoryUI>();
            if (card == null) continue;

            // 3. ตรวจสอบว่าปลาของ Card ใบนี้ อยู่ใน List ที่เลือกหรือไม่
            if (currentSelection.Contains(card.baseFish))
            {
                // ถ้าใช่: เปิด Outline และตั้งค่า selected = true
                card.GetComponent<Image>().sprite = card.selectedSprite;
                card.selected = true;
            }
            else
            {
                // ถ้าไม่ใช่: ปิด Outline และตั้งค่า selected = false
                card.GetComponent<Image>().sprite = card.normalSprite;
                card.selected = false;
            }
        }
    }
}
public enum InventorySource
{
    None,
    Shop,
    Ship
}