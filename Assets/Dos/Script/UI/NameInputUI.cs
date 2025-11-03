// แก้ไขไฟล์: NameInputUI.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro; 
using UnityEngine.SceneManagement;

public class NameInputUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;

    [Header("Config")]
    [SerializeField] private string sceneToLoad = "TestScene"; 
    
    private int targetSlotId; // Slot ที่เรากำลังจะสร้าง

    private void Start()
    {
        confirmButton.onClick.AddListener(OnConfirmClick);
        cancelButton.onClick.AddListener(OnCancelClick);

        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();
        
        Hide();
    }
    
    public void Show(int slotId)
    {
        targetSlotId = slotId; 
        nameInputField.text = ""; 

        // --- *** เพิ่มส่วนนี้ *** ---
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        // --- *** จบส่วนที่เพิ่ม *** ---

        nameInputField.Select();
    }

    // --- *** เพิ่มฟังก์ชันนี้ *** ---
    public void Hide()
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }
    
    /// <summary>
    /// ทำงานเมื่อกดยืนยันชื่อ
    /// </summary>
    private void OnConfirmClick()
    {
        string newName = nameInputField.text;

        if (string.IsNullOrWhiteSpace(newName))
        {
            Debug.Log("ชื่อต้องไม่ว่างเปล่า!");
            return;
        }

        // 1. สร้างไฟล์เซฟ (ใช้ targetSlotId ที่รับมา)
        SaveManager.Instance.CreateNewSaveFile(targetSlotId, newName);

        // 2. ตั้งค่า GameSession (ใช้ targetSlotId ที่รับมา)
        GameSession.Instance.SetCurrentSlot(targetSlotId);
        
        // 3. (สำคัญ) บอก GameManager ว่า "ไม่ต้องโหลด"
        MainMenu.slotToLoad = 0; //

        // 4. โหลด Scene เกม
        SceneManager.LoadScene(sceneToLoad);
    }

    /// <summary>
    /// ทำงานเมื่อกดยกเลิก
    /// </summary>
    private void OnCancelClick()
    {
        // --- *** แก้ไขส่วนนี้ *** ---
        // (เราไม่ควรโหลด Scene ใหม่ แค่ซ่อนหน้าต่างนี้)
        Hide();
    }
}