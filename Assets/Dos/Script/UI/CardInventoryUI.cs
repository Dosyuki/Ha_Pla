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

    public void UpdateCardUI(Fish fish)
    {
        baseFish = fish;
        sprite.sprite = baseFish.SpriteModel;
        weight.text = baseFish.Weight.ToString() + " KG";
        value.text = baseFish.CalculateValue() + " Fishlar";
        name.text = baseFish.Name;
    }
}
