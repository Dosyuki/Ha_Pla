using TMPro;
using UnityEngine;

public class BaitTooltip : Singleton<BaitTooltip>
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private Vector2 offset;

    private void Start()
    {
        Hide();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Update()
    {
        // ให้ tooltip ตามเมาส์
        if (canvasGroup.alpha > 0)
        {
            Vector2 mousePos = Input.mousePosition;
            transform.position = mousePos + offset; // offset นิดหน่อย
        }
    }

    public void Show(string description)
    {
        descriptionText.text = description;
        canvasGroup.alpha = 1;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    public void Hide()
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }
}