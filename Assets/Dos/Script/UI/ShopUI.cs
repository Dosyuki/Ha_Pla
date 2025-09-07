using UnityEngine;
using System.Collections.Generic;

public class ShopUI : MonoBehaviour
{
    [SerializeField] private List<BaseBait> baits = new();
    [SerializeField] private GameObject shopCardPrefab;
    [SerializeField] private Transform shopCardHolder;

    private bool isOpen = false;

    public void OpenShop(List<BaseBait> baits)
    {
        if (isOpen) return;
        isOpen = true;

        this.baits = new List<BaseBait>(baits);
        foreach (Transform child in shopCardHolder)
            Destroy(child.gameObject);

        foreach (BaseBait bait in baits)
        {
            ShopCardUI card = Instantiate(shopCardPrefab, shopCardHolder).GetComponent<ShopCardUI>();
            card.UpdateUI(bait);
        }
    }

    public void CloseShop()
    {
        baits.Clear();
        isOpen = false;
        foreach (Transform child in shopCardHolder)
            Destroy(child.gameObject);
    }
}