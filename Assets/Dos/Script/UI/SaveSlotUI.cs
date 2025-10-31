// ไฟล์ใหม่: SaveSlotUI.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro; // ต้องใช้ TextMeshPro
using UnityEngine.SceneManagement; // ต้องใช้สำหรับเปลี่ยน Scene

public class SaveSlotUI : MonoBehaviour
{
    [Header("Slot Config")]
    [SerializeField] private int slotId; // ตั้งค่าใน Inspector (1, 2, 3, หรือ 4)
    [SerializeField] private string sceneToLoad = "TestScene"; // ตั้งชื่อ Scene เกมหลักของคุณ

    [Header("UI References")]
    [SerializeField] private Button slotButton; // ตัวปุ่มหลัก
    [SerializeField] private GameObject dataDisplayGroup; // Object ที่รวบรวม UI ที่มีข้อมูล
    [SerializeField] private TMP_Text moneyText; // UI Text ที่จะโชว์เงิน
    [SerializeField] private TMP_Text newSaveText; // UI Text ที่จะโชว์คำว่า "New Save"

    private GameData slotData; // เก็บข้อมูลที่ Peek มา

    // ทำงานทุกครั้งที่ Object นี้ถูกเปิด (เช่น เปิดหน้า Load Game)
    private void OnEnable()
    {
        // 1. แอบดูข้อมูลใน Slot นี้
        slotData = SaveManager.Instance.PeekSaveData(slotId);

        // 2. อัปเดต UI
        UpdateSlotDisplay();
    }

    private void Start()
    {
        // 3. ตั้งค่าให้ปุ่มนี้เรียกฟังก์ชัน OnClick_Slot() เมื่อถูกกด
        slotButton.onClick.AddListener(OnClick_Slot);
    }

    private void UpdateSlotDisplay()
    {
        if (slotData == null)
        {
            // --- กรณีเป็น "New Save" ---
            newSaveText.gameObject.SetActive(true);
            newSaveText.text = $"New Game {slotId}";
            dataDisplayGroup.SetActive(false);
        }
        else
        {
            // --- กรณีมี "Save Data" ---
            newSaveText.gameObject.SetActive(false);
            dataDisplayGroup.SetActive(true);

            // ดึงข้อมูลเงินมาแสดง
            // (เราใช้ .playerStats.money จาก GameData ที่เราออกแบบไว้)
            moneyText.text = $"${slotData.playerStats.money:F2}"; 
            
            // (คุณสามารถเพิ่มข้อมูลอื่นได้ตามต้องการ เช่น เวลาเล่น, ชื่อตัวละคร ฯลฯ)
        }
    }

    /// <summary>
    /// ฟังก์ชันนี้จะทำงานเมื่อผู้เล่น "กด" ปุ่ม Slot นี้
    /// </summary>
    public void OnClick_Slot()
    {
        
        GameSession.Instance.SetCurrentSlot(slotId);
        
        if (slotData == null)
        {
            // ถ้าไม่มีข้อมูล = เริ่มเกมใหม่
            Debug.Log($"Starting New Game in Slot {slotId}...");
            MainMenu.slotToLoad = 0; // 0 = New Game [อ้างอิงจาก MainMenu.cs]
        }
        else
        {
            // ถ้ามีข้อมูล = โหลดเกม
            Debug.Log($"Loading Game from Slot {slotId}...");
            MainMenu.slotToLoad = slotId; // [อ้างอิงจาก MainMenu.cs]
        }

        // โหลด Scene เกมหลัก
        SceneManager.LoadScene(sceneToLoad);
    }
}