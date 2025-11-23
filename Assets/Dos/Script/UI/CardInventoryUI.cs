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
    [SerializeField] public Sprite normalSprite;
    [SerializeField] public Sprite selectedSprite;
    [SerializeField] public Image bgImage;

    public bool selected;
    public void UpdateCardUI(Fish fish)
    {
        baseFish = fish;
        sprite.sprite = baseFish.SpriteModel;
        weight.text = $"{baseFish.Weight:F2} KG";
        value.text = $"{baseFish.CalculateValue():F2} Fishlars";
        name.text = baseFish.Name;
        if (bgImage != null)
        {
            bgImage.sprite = CardInventoryManager.Instance.getSprite(baseFish.Rarity, selected);
        }
    }

    public void OnClick()
    {
        Debug.Log("Clicked On " + baseFish.Name);
        if(!ShopManager.Instance.GetIsOpen())
            return;
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
    public void SetSelectionVisual(bool isSelected)
    {
        if (bgImage != null)
        {
            bgImage.sprite = CardInventoryManager.Instance.getSprite(baseFish.Rarity, isSelected);
        }
        selected = isSelected;
    }
}
