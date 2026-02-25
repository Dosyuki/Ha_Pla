using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BaitQuickSelectSlot : MonoBehaviour
{
    [SerializeField] private Image baitIcon;
    [SerializeField] private TMP_Text amountText;

    // ฟังก์ชันตั้งค่าข้อมูลตอนที่เสก (Instantiate) ออกมา
    public void Setup(Bait bait, bool isSelected)
    {
        if (baitIcon != null) baitIcon.sprite = bait.GetBaseBait().Sprite;
        if (amountText != null) amountText.text = bait.amount.ToString();
        
        SetSelected(isSelected);
    }

    // ฟังก์ชันเปิด/ปิดกรอบ Outline
    public void SetSelected(bool isSelected)
    {
        GetComponent<Outline>().enabled = isSelected;
    }
}