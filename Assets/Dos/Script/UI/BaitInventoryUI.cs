using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BaitInventoryUI : MonoBehaviour , IPointerEnterHandler , IPointerExitHandler
{
    [SerializeField] public Bait baseBait;
    [SerializeField] private Image sprite;
    [SerializeField] private TMP_Text amountText;
    [SerializeField] private TMP_Text name;

    private bool selected;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(Click);
    }

    public void UpdateCardUI(Bait bait,int amount)
    {
        baseBait = bait;
        sprite.sprite = bait.GetBaseBait().Sprite;
        name.text = baseBait.Name;
        amountText.text = amount.ToString();
    }

    public void Click()
    {
        Inventory.Instance.currentBait = baseBait;
        BaitInventoryPanel.Instance.CheckCurrentBaitSelected();
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        BaitTooltip.Instance.Show(baseBait.Description);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        BaitTooltip.Instance.Hide();
    }
}
