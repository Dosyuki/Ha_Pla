using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopCardUI : MonoBehaviour
{
    [SerializeField] private BaseBait baseBait;
    
    [SerializeField] private Image image;
    [SerializeField] private TMP_Text NameText;
    [SerializeField] private TMP_Text ValueText;
    

    public void UpdateUI(BaseBait bait)
    {
        baseBait = bait;
        image.sprite = baseBait.Sprite;
        NameText.text = baseBait.Name;
        ValueText.text = ((int)baseBait.Value).ToString();
    }

    public void Clicked()
    {
        PlayerStats.Instance.RemoveMoney(baseBait.Value);
        Inventory.Instance.AddBait(baseBait,1);
        Debug.Log(baseBait.Name + " " + baseBait.Value);
    }
}
