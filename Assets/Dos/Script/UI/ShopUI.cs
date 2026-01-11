using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class ShopUI : MonoBehaviour
{
    [SerializeField] private GameObject shopCardPrefab;
    [SerializeField] private Transform shopCardHolder;
    
    // ปุ่ม Buy ของร้าน (ปุ่มใหญ่)
    [SerializeField] private Button confirmBuyButton;
    [SerializeField] private TMP_Text totalBillText; // โชว์ราคารวมทั้งหมด (Optional)

    private List<ShopCardUI> _activeCards = new List<ShopCardUI>();
    private List<BaseBait> _activeBaits = new List<BaseBait>();
    private bool isOpen = false;

    private void Start()
    {
        if(confirmBuyButton != null)
            confirmBuyButton.onClick.AddListener(OnConfirmBuyClicked);
    }
    
    private void Update()
    {
        // (Optional) อัปเดตราคา Realtime
        if (isOpen && totalBillText != null)
        {
            float total = 0;
            foreach (var card in _activeCards) total += card.GetTotalCost();
            totalBillText.text = $"Total: {total:N0} $";
        }
    }

    public void OpenShop(List<BaseBait> baits)
    {
        _activeBaits = new List<BaseBait>(baits);
        if (isOpen) return;
        isOpen = true;
        gameObject.SetActive(true); // เปิดหน้าต่าง

        // เคลียร์ของเก่า
        CreateCard(baits);
    }

    private void CreateCard(List<BaseBait> baits)
    {
        foreach (Transform child in shopCardHolder) Destroy(child.gameObject);
        _activeCards.Clear();

        // สร้างการ์ดใหม่
        foreach (BaseBait bait in baits)
        {
            GameObject obj = Instantiate(shopCardPrefab, shopCardHolder);
            ShopCardUI card = obj.GetComponent<ShopCardUI>();
            card.Setup(bait);
            _activeCards.Add(card); // เก็บลง List เพื่อไว้เช็คตอนกด Buy
        }
    }

    private void OnConfirmBuyClicked()
    {
        float grandTotal = 0;
        
        // 1. คำนวณราคารวมทั้งหมด
        foreach (var card in _activeCards)
        {
            grandTotal += card.GetTotalCost();
        }

        if (grandTotal <= 0)
        {
            Debug.Log("ยังไม่ได้เลือกสินค้าเลย!");
            return;
        }

        // 2. เช็คเงินผู้เล่น
        if (PlayerStats.Instance.GetMoney() >= grandTotal)
        {
            // --- ผ่าน: ทำการซื้อขาย ---
            
            // 2.1 หักเงินทีเดียว
            PlayerStats.Instance.RemoveMoney(grandTotal);
            InventoryUI.Instance.UpdateText();

            // 2.2 วนลูปเอาของเข้าตัว
            foreach (var card in _activeCards)
            {
                int amount = card.GetAmount();
                if (amount > 0)
                {
                    Inventory.Instance.AddBait(card.GetBait(), amount);
                }
            }

            Debug.Log($"ซื้อสำเร็จ! รวม {grandTotal}$");
            CreateCard(_activeBaits);

        }
        else
        {
            Debug.Log("เงินไม่พอจ่ายบิลนี้!");
            // UIAlert.Instance.ShowWarning("Not enough money!");
        }
        
        
    }

    public void CloseShop()
    {
        isOpen = false;
        gameObject.SetActive(false);
    }
}