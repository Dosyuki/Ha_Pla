using System.Collections.Generic;
using UnityEngine;

public class BaitQuickSelectUI : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private CanvasGroup quickSelectCanvasGroup;
    
    [Header("Instantiate Settings")]
    [SerializeField] private Transform slotContainer;      // โฟลเดอร์/ตำแหน่งที่จะเสก Prefab ไปใส่
    [SerializeField] private GameObject baitSlotPrefab;    // Prefab ของช่องเหยื่อที่มีสคริปต์ BaitQuickSelectSlot

    private bool isOpen = false;
    private int currentIndex = 0;
    
    private List<Bait> availableBaits = new List<Bait>();
    private List<BaitQuickSelectSlot> spawnedSlots = new List<BaitQuickSelectSlot>();

    void Start()
    {
        quickSelectCanvasGroup.alpha = 0;
        quickSelectCanvasGroup.interactable = false;
        quickSelectCanvasGroup.blocksRaycasts = false;
    }

    void Update()
    {
        // 1. เช็คว่าถ้ากำลังโยนเบ็ดอยู่ ห้ามเปิด
        if (Inventory.Instance.CurrentRod != null && Inventory.Instance.CurrentRod.getIsThrown())
        {
            if (isOpen) CloseWindow();
            return;
        }

        // 2. กดปุ่ม X เพื่อ Toggle เปิด/ปิด
        if (Input.GetKeyDown(KeyCode.X))
        {
            if (!isOpen && UIManager.Instance.GetCurrentState() == currentState.UI)
                return;

            if (isOpen) CloseWindow();
            else OpenWindow();
        }

        // 3. ควบคุมการ Scroll
        if (isOpen)
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll > 0f) CycleBait(-1);      // เลื่อนขึ้น
            else if (scroll < 0f) CycleBait(1);  // เลื่อนลง
            
            // คลิกซ้ายเพื่อปิดและยืนยัน
            if (Input.GetMouseButtonDown(0))
            {
                CloseWindow();
            }
        }
    }

    private void OpenWindow()
    {
        availableBaits = Inventory.Instance.GetAllBait();

        if (availableBaits == null || availableBaits.Count == 0)
        {
            Debug.Log("No baits in inventory!"); 
            return;
        }

        isOpen = true;
        //UIManager.Instance.ChangeState(currentState.UI); 

        // หา Index เหยื่อปัจจุบัน
        currentIndex = availableBaits.IndexOf(Inventory.Instance.currentBait);
        if (currentIndex == -1) currentIndex = 0;

        // เสก UI ออกมาโชว์
        RefreshSlots();

        quickSelectCanvasGroup.alpha = 1;
    }

    private void CloseWindow()
    {
        isOpen = false;
        quickSelectCanvasGroup.alpha = 0;
        //UIManager.Instance.ChangeState(currentState.None);
    }

    // ฟังก์ชันเคลียร์ของเก่า และเสกของใหม่
    private void RefreshSlots()
    {
        // ลบของเก่าทิ้งให้หมดก่อน
        foreach (var slot in spawnedSlots)
        {
            Destroy(slot.gameObject);
        }
        spawnedSlots.Clear();

        // วนลูปเสก Prefab ตามจำนวนชนิดเหยื่อที่มี
        for (int i = 0; i < availableBaits.Count; i++)
        {
            GameObject obj = Instantiate(baitSlotPrefab, slotContainer);
            BaitQuickSelectSlot slotUI = obj.GetComponent<BaitQuickSelectSlot>();
            
            // เช็คว่าช่องนี้คือช่องที่เลือกอยู่หรือเปล่า
            bool isSelected = (i == currentIndex);
            slotUI.Setup(availableBaits[i], isSelected);
            
            spawnedSlots.Add(slotUI);
        }
    }

    // ฟังก์ชันคำนวณการเลื่อน Index
    private void CycleBait(int direction)
    {
        if (availableBaits.Count == 0) return;

        currentIndex += direction;

        if (currentIndex >= availableBaits.Count) currentIndex = 0;
        else if (currentIndex < 0) currentIndex = availableBaits.Count - 1;

        // อัปเดตข้อมูลเหยื่อ
        Inventory.Instance.currentBait = availableBaits[currentIndex];
        
        // อัปเดตการแสดงผล Outline เท่านั้น (ไม่ต้องเสกใหม่)
        UpdateSelectionVisuals();
    }

    // ฟังก์ชันเปิด/ปิด Outline ตาม Index
    private void UpdateSelectionVisuals()
    {
        for (int i = 0; i < spawnedSlots.Count; i++)
        {
            // ถ้า i ตรงกับ currentIndex ให้ส่งค่า true (เปิด Outline)
            spawnedSlots[i].SetSelected(i == currentIndex);
        }
    }
}