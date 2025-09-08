using System;
using System.Collections;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopCardUI : MonoBehaviour
{
    [SerializeField] private BaseBait baseBait;
    
    [SerializeField] private Image image;
    [SerializeField] private TMP_Text NameText;
    [SerializeField] private TMP_Text ValueText;
    [SerializeField] private TMP_Text BuyAmountText;

    private bool isRepeatClicking =  false;
    private int comboBuy = 0;
    private float Timer = 1f;
    private float curTime = 0;
    

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
        InventoryUI.Instance.UpdateText();
        Inventory.Instance.AddBait(baseBait,1);
        Debug.Log(baseBait.Name + " " + baseBait.Value);
        isRepeatClicking = true;
        comboBuy++;
        curTime = 0;
    }

    private void Update()
    {
        if(!isRepeatClicking)
            return;
        curTime += Time.deltaTime;
        if (curTime <= Timer)
        {
            BuyAmountText.text = comboBuy.ToString();
        }
        else
        {
            BuyAmountText.text = string.Empty;
            isRepeatClicking = false;
            comboBuy = 0;
        }
    }
}
