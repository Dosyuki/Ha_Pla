using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShopManager : Singleton<ShopManager>
{
    [SerializeField] private LayerMask playerMask;
    [SerializeField] private float hitboxSize;
    
    private Collider[] colliders;
    private CanvasGroup shopUIGroup;
    
    [SerializeField] private List<Fish> selectedFish = new List<Fish>();
    [SerializeField] private bool isOpen = false;

    [SerializeField] CanvasGroup shopUICanvasGroup;
    [SerializeField] private TMP_Text valueText;
    private float currentSelectedValue;
    
    [SerializeField] private List<BaseBait> baits = new List<BaseBait>();
    private void Start()
    {
        shopUIGroup = transform.GetComponentInChildren<CanvasGroup>();
    }


    private void Update()
    {
        colliders = Physics.OverlapBox(transform.position, (transform.lossyScale / 2f) * hitboxSize, transform.rotation, playerMask);
        if (colliders.Length > 0)
        {
            shopUIGroup.alpha = 1;
            if (Input.GetKeyDown(KeyCode.F))
            {
                if (!isOpen)  // only open if not already open
                {
                    isOpen = true;
                    InventoryUI.Instance.CreateCardUI(false);
                    OpenShopUI();
                    return;
                    // DO NOT call UIManager.ChangeState here
                }
            }
            if (isOpen && (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.F)))
            {
                isOpen = false;
                InventoryUI.Instance.CloseCardUI(InventorySource.Shop);
                CloseShopUI();
                selectedFish.Clear();
            }
        }
        else if (colliders.Length == 0)
            shopUIGroup.alpha = 0;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale * hitboxSize);
        Gizmos.matrix = rotationMatrix;
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
        Gizmos.matrix = Matrix4x4.identity;
    }
    public void AddSelectedFish(Fish fish)
    {
        selectedFish.Add(fish);
        CalculateAllValue();
    }

    public void RemoveSelectedFish(Fish fish)
    {
        selectedFish.Remove(fish);
        CalculateAllValue();
    }

    public void CalculateAllValue()
    {
        float _value = 0;
        if(selectedFish.Count > 0)
            foreach (Fish fish in selectedFish)
                _value +=  fish.CalculateValue();
        currentSelectedValue = _value;
        valueText.text = $"{currentSelectedValue} Fishllars";
    }

    public void OpenShopUI()
    {
        shopUICanvasGroup.interactable = true;
        shopUICanvasGroup.alpha = 1;
        shopUICanvasGroup.blocksRaycasts = true;
        shopUICanvasGroup.GetComponent<ShopUI>().OpenShop(baits);
    }

    public void CloseShopUI()
    {
        shopUICanvasGroup.interactable = false;
        shopUICanvasGroup.alpha = 0;
        shopUICanvasGroup.blocksRaycasts = false;
        shopUICanvasGroup.GetComponent<ShopUI>().CloseShop();
        
    }

    public void ConfirmSell()
    {
        PlayerStats.Instance.AddMoney(currentSelectedValue);
        foreach (Fish fish in selectedFish)
        {
            Inventory.Instance.RemoveFish(fish);
        }
        InventoryUI.Instance.CloseCardUI(InventorySource.Shop);
        InventoryUI.Instance.CreateCardUI(false);
        selectedFish.Clear();
        CalculateAllValue();
    }
    public bool GetIsOpen() => isOpen;
    
}
