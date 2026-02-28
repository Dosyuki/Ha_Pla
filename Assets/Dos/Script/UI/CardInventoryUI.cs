using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardInventoryUI : MonoBehaviour
{
    [SerializeField] public Fish baseFish;
    [SerializeField] private Image sprite;
    [SerializeField] private TMP_Text weight;
    [SerializeField] private TMP_Text value;
    [SerializeField] private TMP_Text name;
    [SerializeField] private TMP_Text description;
    [SerializeField] public Sprite normalSprite;
    [SerializeField] public Sprite selectedSprite;
    [SerializeField] public Image bgImage;
    [SerializeField] public GameObject InfoBox;

    public bool selected;
    private bool showInfo;
    public void UpdateCardUI(Fish fish)
    {
        baseFish = fish;
        sprite.sprite = baseFish.SpriteModel;
        weight.text = $"{baseFish.Weight:F2} KG";
        value.text = $"{baseFish.CalculateValue():F2} Fishlars";
        if(description != null)
            description.text = $"{baseFish.Description}";
        name.text = baseFish.Name;
        if (bgImage != null)
        {
            //bgImage.sprite = CardInventoryManager.Instance.getSprite(baseFish.Rarity, selected);
        }
    }

    public void OnClick()
    {
        Debug.Log("Clicked On " + baseFish.Name);
        if (ShopManager.Instance.GetIsOpen())
        {
            if (!selected)
            {
                ShopManager.Instance.AddSelectedFish(baseFish);
                SetSelectionVisual(true); 
            }
            else
            {
                ShopManager.Instance.RemoveSelectedFish(baseFish);
                SetSelectionVisual(false);
            }
        }
        else if (InventoryUI.Instance.isOpen && InfoBox != null)
        {
            showInfo = !showInfo;
            InfoBox.SetActive(showInfo);
            InventoryUI.Instance.SelectFish(baseFish);
        }
    }
    public void SetSelectionVisual(bool isSelected)
    {
        if (bgImage != null)
        {
            //bgImage.sprite = CardInventoryManager.Instance.getSprite(baseFish.Rarity, isSelected);
        }
        selected = isSelected;
    }
}
