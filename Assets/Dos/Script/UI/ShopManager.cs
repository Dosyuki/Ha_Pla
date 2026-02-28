using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShopManager : Singleton<ShopManager>
{
    [SerializeField] private LayerMask playerMask;
    [SerializeField] private float hitboxSize;
    [SerializeField] private Vector3 offset;
    
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
        colliders = Physics.OverlapBox(transform.position + offset, (transform.lossyScale / 2f) * hitboxSize, transform.rotation, playerMask);
        if (colliders.Length > 0)
        {
            shopUIGroup.alpha = 1;
            /*
            if (Input.GetKeyDown(KeyCode.F))
            {
                if (!isOpen)  // only open if not already open
                {
                    isOpen = true;
                    OpenShopUI();
                    return;
                    // DO NOT call UIManager.ChangeState here
                }
            }*/
            if (isOpen && (Input.GetKeyDown(KeyCode.Escape)))
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
        Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.position + offset, transform.rotation, transform.lossyScale * hitboxSize);
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
        valueText.text = $"{currentSelectedValue:F2} Fishlars";
    }

    public void OpenShopUI()
    {
        InventoryUI.Instance.CreateCardUI(false);
        isOpen = true;
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
    public void OnClick_ToggleSelectAll()
    {
        // 1. ดึงปลาทั้งหมดที่มีใน Inventory
        List<Fish> allFish = Inventory.Instance.GetAllFish(); //
        if (allFish == null || allFish.Count == 0) return; // ไม่มีปลาให้เลือก

        // 2. ตรวจสอบสถานะปัจจุบัน
        // (ถ้าจำนวนที่เลือก < จำนวนทั้งหมด = เราจะ "เลือกทั้งหมด")
        bool shouldSelectAll = selectedFish.Count < allFish.Count;

        // 3. ล้าง List ที่เลือกไว้ก่อน
        selectedFish.Clear();

        // 4. ถ้าเราควรกด "เลือกทั้งหมด"
        if (shouldSelectAll)
        {
            // เพิ่มปลาทั้งหมดเข้าไปใน List
            selectedFish.AddRange(allFish);
        }
        // (ถ้า shouldSelectAll เป็น false, List จะว่างเปล่า = ไม่เลือกเลย)

        // 5. คำนวณราคารวมใหม่
        CalculateAllValue();

        // 6. (สำคัญ) สั่งให้ InventoryUI "วาด Outline" ใหม่ทั้งหมด
        InventoryUI.Instance.RefreshAllCardVisuals();
    }
    public List<Fish> GetSelectedFishList()
    {
        return selectedFish;
    }
    public bool GetIsOpen() => isOpen;
    
}
