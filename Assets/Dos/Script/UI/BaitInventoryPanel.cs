using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BaitInventoryPanel : Singleton<BaitInventoryPanel>
{
    [SerializeField] private GameObject baitCardPrefab;
    [SerializeField] private Transform baitCardHolder;

    private CanvasGroup canvasGroup;
    private bool isOpen = false;

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        CloseBaitUI();
    }

    /// <summary>
    /// เปิด UI การ์ดเหยื่อ
    /// </summary>
    public void OpenBaitUI()
    {
        if (isOpen) return;

        isOpen = true;
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        InventoryUI.Instance.fishGroup.alpha = 0;
        InventoryUI.Instance.fishGroup.interactable = false;
        InventoryUI.Instance.fishGroup.blocksRaycasts = false;


        // โหลดเหยื่อทั้งหมดจาก Inventory
        List<Bait> allBait = Inventory.Instance.GetAllBait();

        foreach (Bait bait in allBait)
        {
            BaitInventoryUI card = Instantiate(baitCardPrefab, baitCardHolder).GetComponent<BaitInventoryUI>();
            card.UpdateCardUI(bait, bait.amount);
        }
        CheckCurrentBaitSelected();
    }

    /// <summary>
    /// ปิด UI การ์ดเหยื่อ
    /// </summary>
    public void CloseBaitUI()
    {
        if (!isOpen) return;

        isOpen = false;
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        foreach (Transform child in baitCardHolder)
        {
            Destroy(child.gameObject);
        }
    }

    public void CheckCurrentBaitSelected()
    {
        foreach (Transform child in baitCardHolder)
        {
            BaitInventoryUI card = child.GetComponent<BaitInventoryUI>();
            if (card.baseBait == Inventory.Instance.currentBait)
                card.GetComponent<Image>().sprite = card.selectedSprite;
            else
                card.GetComponent<Image>().sprite = card.normalSprite;
        }
    }
}