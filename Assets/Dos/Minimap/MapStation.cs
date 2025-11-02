using UnityEngine;

// ตรวจสอบให้แน่ใจว่า GameObject นี้มี Collider ที่เป็น Trigger
[RequireComponent(typeof(Collider))]
public class MapStation : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("ลาก CanvasGroup ของหน้าต่าง Minimap UI มาใส่")]
    [SerializeField] private CanvasGroup minimapCanvasGroup; // (เปลี่ยนจาก Encyclopedia)

    [Tooltip("ลาก CanvasGroup ของ UI 'Press F' มาใส่ (Optional)")]
    [SerializeField] private CanvasGroup pressFPrompt;

    [Header("Config")]
    [Tooltip("Tag ของ Player Object")]
    [SerializeField] private string playerTag = "Player";

    private bool isInRange = false; // ผู้เล่นอยู่ในระยะหรือไม่
    private bool isOpen = false; // หน้าต่าง UI เปิดอยู่หรือไม่

    private void Start()
    {
        // ตรวจสอบ Collider ว่าเป็น Trigger
        Collider col = GetComponent<Collider>();
        if (!col.isTrigger)
        {
            Debug.LogWarning($"Collider บน {gameObject.name} ไม่ได้ตั้งค่าเป็น IsTrigger!");
            col.isTrigger = true;
        }

        // ซ่อน UI ตอนเริ่ม
        if (pressFPrompt != null) pressFPrompt.alpha = 0;
        
        if (minimapCanvasGroup == null)
        {
            Debug.LogError("ยังไม่ได้ลาก Minimap Canvas Group มาใส่ใน Inspector!");
        }
        else
        {
            // ซิงค์สถานะเริ่มต้น
            isOpen = minimapCanvasGroup.alpha > 0;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // เมื่อผู้เล่นเดินเข้ามาในระยะ
        if (other.CompareTag(playerTag))
        {
            isInRange = true;
            // แสดงปุ่ม "Press F" (ถ้ามี)
            if (pressFPrompt != null && !isOpen)
            {
                pressFPrompt.alpha = 1;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // เมื่อผู้เล่นเดินออกจากระยะ
        if (other.CompareTag(playerTag))
        {
            isInRange = false;
            // ซ่อนปุ่ม "Press F"
            if (pressFPrompt != null)
            {
                pressFPrompt.alpha = 0;
            }

            // ถ้าผู้เล่นเดินหนีในขณะที่ UI เปิดอยู่ ให้ปิด UI อัตโนมัติ
            if (isOpen)
            {
                CloseMap(); // (เปลี่ยนจาก CloseEncyclopedia)
            }
        }
    }

    private void Update()
    {
        // 1. ตรวจสอบการ "เปิด" (ต้องอยู่ในระยะ, กด F, และ UI ยังไม่เปิด)
        if (isInRange && Input.GetKeyDown(KeyCode.F) && !isOpen)
        {
            OpenMap(); // (เปลี่ยนจาก OpenEncyclopedia)
        }
        // 2. ตรวจสอบการ "ปิด" (UI ต้องเปิดอยู่, และกด F หรือ ESC)
        else if (isOpen && (Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.Escape)))
        {
            CloseMap(); // (เปลี่ยนจาก CloseEncyclopedia)
        }
    }

    private void OpenMap() // (เปลี่ยนจาก OpenEncyclopedia)
    {
        isOpen = true;
        minimapCanvasGroup.alpha = 1;
        minimapCanvasGroup.interactable = true;
        minimapCanvasGroup.blocksRaycasts = true;

        // เปลี่ยน State ของเกม (อ้างอิงจาก UIManager ของคุณ)
        UIManager.Instance.ChangeState(currentState.UI);

        // ซ่อนปุ่ม "Press F"
        if (pressFPrompt != null)
        {
            pressFPrompt.alpha = 0;
        }
    }

    private void CloseMap() // (เปลี่ยนจาก CloseEncyclopedia)
    {
        isOpen = false;
        minimapCanvasGroup.alpha = 0;
        minimapCanvasGroup.interactable = false;
        minimapCanvasGroup.blocksRaycasts = false;

        // เปลี่ยน State ของเกมกลับ (อ้างอิงจาก UIManager ของคุณ)
        UIManager.Instance.ChangeState(currentState.None);

        // ถ้าผู้เล่น "ยัง" อยู่ในระยะ ให้แสดงปุ่ม "Press F" กลับมา
        if (isInRange && pressFPrompt != null)
        {
            pressFPrompt.alpha = 1;
        }
    }
}