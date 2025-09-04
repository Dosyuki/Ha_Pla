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

    private bool selected;
    public void UpdateCardUI(Fish fish)
    {
        baseFish = fish;
        sprite.sprite = baseFish.SpriteModel;
        weight.text = baseFish.Weight.ToString() + " KG";
        value.text = baseFish.CalculateValue() + " Fishlar";
        name.text = baseFish.Name;
    }

    public void OnClick()
    {
        Debug.Log("Clicked On " + baseFish.Name);
        if(!ShopManager.Instance.GetIsOpen())
            return;
        if (!selected)
        {
            GetComponent<Outline>().enabled = true;
            selected = true;
            ShopManager.Instance.AddSelectedFish(baseFish);
        }
        else
        {
            GetComponent<Outline>().enabled = false;
            selected = false;
            ShopManager.Instance.RemoveSelectedFish(baseFish);
        }
    }
}
