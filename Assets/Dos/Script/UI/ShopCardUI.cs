using System;
using System.Collections;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopCardUI : MonoBehaviour , IPointerEnterHandler, IPointerExitHandler
{
    [Header("Data")]
    private BaseBait _baseBait;
    private int _currentAmount = 0; // เริ่มต้นที่ 0 (ถ้าจะซื้อต้องกด +)

    [Header("UI Components")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text pricePerUnitText; // ราคาต่อชิ้น
    [SerializeField] private TMP_Text amountText;       // จำนวนที่เลือก

    [Header("Buttons")]
    [SerializeField] private Button plusButton;
    [SerializeField] private Button minusButton;
    

    public void Setup(BaseBait bait)
    {
        _baseBait = bait;
        _currentAmount = 0; // รีเซ็ตเป็น 0

        iconImage.sprite = bait.Sprite;
        nameText.text = bait.Name;
        pricePerUnitText.text = $"{bait.Value} $/pc";

        // ผูกปุ่ม
        plusButton.onClick.RemoveAllListeners();
        minusButton.onClick.RemoveAllListeners();
        
        plusButton.onClick.AddListener(() => ChangeAmount(1));
        minusButton.onClick.AddListener(() => ChangeAmount(-1));

        UpdateDisplay();
    }

    private void ChangeAmount(int change)
    {
        _currentAmount += change;
        if (_currentAmount < 0) _currentAmount = 0; // ห้ามติดลบ

        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        amountText.text = _currentAmount.ToString();
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        BaitTooltip.Instance.Show(_baseBait.Description);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        BaitTooltip.Instance.Hide();
    }
    // --- ฟังก์ชันสำหรับให้ ShopUI (ตัวแม่) ดึงข้อมูล ---
    public BaseBait GetBait() => _baseBait;
    public int GetAmount() => _currentAmount;
    public float GetTotalCost() => _baseBait.Value * _currentAmount;
}
