// ไฟล์ใหม่: SaveSlotUI.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro; // ต้องใช้ TextMeshPro
using UnityEngine.SceneManagement; // ต้องใช้สำหรับเปลี่ยน Scene

public class SaveSlotUI : MonoBehaviour
{
    [Header("Slot Config")]
    [SerializeField] private int slotId; 
    [SerializeField] private string sceneToLoad = "TestScene"; 

    [Header("UI References")]
    [SerializeField] private Button slotButton; 
    [SerializeField] private GameObject dataDisplayGroup;
    [SerializeField] private TMP_Text moneyText; 
    [SerializeField] private TMP_Text newSaveText;
    [SerializeField] private NameInputUI nameInputUI;
    private GameData slotData; // เก็บข้อมูลที่ Peek มา

    private void OnEnable()
    {
        slotData = SaveManager.Instance.PeekSaveData(slotId);

        UpdateSlotDisplay();
    }

    private void Start()
    {
        slotButton.onClick.AddListener(OnClick_Slot);
    }

    private void UpdateSlotDisplay()
    {
        if (slotData == null)
        {
            newSaveText.gameObject.SetActive(true);
            newSaveText.text = $"New Game {slotId}";
            dataDisplayGroup.SetActive(false);
        }
        else
        {
            newSaveText.text = $"Slot {slotId}";
            dataDisplayGroup.SetActive(true);

           
            moneyText.text = $"${slotData.playerStats.money:F2}"; 
            newSaveText.text = slotData.saveSlotName;
        }
    }

   
    public void OnClick_Slot()
    {
        
        GameSession.Instance.SetCurrentSlot(slotId);
        
        if (slotData == null)
        {
            if (nameInputUI == null)
            {
                Debug.LogError("ยังไม่ได้ตั้งค่า NameInputUI ใน Inspector ของ SaveSlotUI!");
                return;
            }
            Debug.Log($"Starting New Game in Slot {slotId}...");
            nameInputUI.Show(slotId);
        }
        else
        {
            Debug.Log($"Loading Game from Slot {slotId}...");
            
            // (เฉพาะตอน "Load" เท่านั้น ที่เราจะตั้งค่า GameSession ที่นี่)
            GameSession.Instance.SetCurrentSlot(slotId);
            MainMenu.slotToLoad = slotId; //
            
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}