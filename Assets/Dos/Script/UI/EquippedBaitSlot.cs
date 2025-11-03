using UnityEngine;
using UnityEngine.UI;


public class EquippedBaitSlot : Singleton<EquippedBaitSlot>
{
    [Header("UI References")]
    [Tooltip("ลาก 'Image' ของปุ่มนี้มาใส่")]
    [SerializeField] private Image baitImage;

    [Tooltip("ลาก Sprite ที่เป็นรูป 'ช่องว่าง' มาใส่")]
    [SerializeField] private Sprite emptySlotSprite;

    private Button button;

    protected void Awake()
    {
        // ดึงปุ่มที่ติดอยู่กับ GameObject นี้
        button = GetComponent<Button>();
        if (button != null)
        {
            // สั่งให้ปุ่มเรียกฟังก์ชัน OnClick_Unequip() เมื่อถูกคลิก
            button.onClick.AddListener(OnClick_Unequip);
        }

        UpdateEquippedBait(null);
    }


    public void UpdateEquippedBait(Bait bait)
    {
        if (baitImage == null) return;

        if (bait == null)
        {
            baitImage.color = new Color(0, 0, 0, 0);
       
        }
        else
        {
         
            baitImage.color = new Color(255, 255, 255, 1);
            baitImage.sprite = bait.GetBaseBait().Sprite;
            
          
        }
    }
    
    public void OnClick_Unequip()
    {
        // 1. ตรวจสอบว่ามีเหยื่อให้ถอดหรือไม่ (ถ้าไม่มี ก็ไม่ต้องทำอะไร)
        if (Inventory.Instance.currentBait == null)
        {
            return;
        }

        Debug.Log("ถอดเหยื่อ (Unequip Bait)");
        
        Inventory.Instance.currentBait = null;
        BaitInventoryPanel.Instance.CheckCurrentBaitSelected();
    }
}