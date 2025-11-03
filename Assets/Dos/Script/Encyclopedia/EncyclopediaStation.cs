using UnityEngine;

[RequireComponent(typeof(Collider))]
public class EncyclopediaStation : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("ลาก CanvasGroup ของหน้าต่าง Encyclopedia UI มาใส่")]
    [SerializeField] private CanvasGroup encyclopediaCanvasGroup;

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

        if (pressFPrompt != null) pressFPrompt.alpha = 0;
        

        if (encyclopediaCanvasGroup == null)
        {
            Debug.LogError("ยังไม่ได้ลาก Encyclopedia Canvas Group มาใส่ใน Inspector!");
        }
        else
        {
            // ซิงค์สถานะเริ่มต้น
            isOpen = encyclopediaCanvasGroup.alpha > 0;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            isInRange = true;
            if (pressFPrompt != null && !isOpen) 
            {
                pressFPrompt.alpha = 1;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            isInRange = false;
            if (pressFPrompt != null)
            {
                pressFPrompt.alpha = 0;
            }

            if (isOpen)
            {
                CloseEncyclopedia();
            }
        }
    }

    private void Update()
    {

        if (isInRange && Input.GetKeyDown(KeyCode.F) && !isOpen)
        {
            OpenEncyclopedia();
        }

        else if (isOpen && (Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.Escape)))
        {
            CloseEncyclopedia();
        }
    }

    private void OpenEncyclopedia()
    {
        isOpen = true;
        encyclopediaCanvasGroup.alpha = 1;
        encyclopediaCanvasGroup.interactable = true;
        encyclopediaCanvasGroup.blocksRaycasts = true;

        UIManager.Instance.ChangeState(currentState.UI);

        if (pressFPrompt != null)
        {
            pressFPrompt.alpha = 0;
        }
    }

    private void CloseEncyclopedia()
    {
        isOpen = false;
        encyclopediaCanvasGroup.alpha = 0;
        encyclopediaCanvasGroup.interactable = false;
        encyclopediaCanvasGroup.blocksRaycasts = false;

        UIManager.Instance.ChangeState(currentState.None);

        if (isInRange && pressFPrompt != null)
        {
            pressFPrompt.alpha = 1;
        }
    }
}