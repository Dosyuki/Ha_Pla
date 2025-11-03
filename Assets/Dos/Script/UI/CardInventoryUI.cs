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

    public bool selected;
    public void UpdateCardUI(Fish fish)
    {
        baseFish = fish;
        sprite.sprite = baseFish.SpriteModel;
        weight.text = $"{baseFish.Weight:F2} KG";
        value.text = $"{baseFish.CalculateValue():F2} Fishlars";
        name.text = baseFish.Name;
    }

    public void OnClick()
    {
        Debug.Log("Clicked On " + baseFish.Name);
        if(!ShopManager.Instance.GetIsOpen())
            return;
        if (!selected)
        {
            GetComponent<Image>().sprite = selectedSprite;
            selected = true;
            ShopManager.Instance.AddSelectedFish(baseFish);
        }
        else
        {
            GetComponent<Image>().sprite = normalSprite;
            selected = false;
            ShopManager.Instance.RemoveSelectedFish(baseFish);
        }
    }
}
