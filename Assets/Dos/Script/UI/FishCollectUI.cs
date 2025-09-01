using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FishCollectUI : MonoBehaviour
{
    public Fish fishStats;
    public TMP_Text fishName;
    public TMP_Text fishWeight;
    public Image fishSprite;
    public void UpdateFish(Fish fish)
    {
        fishStats = fish;
        gameObject.SetActive(true);
        
        fishName.text = fishStats.Name;
        fishWeight.text = fishStats.Weight.ToString();
        fishSprite.sprite = fishStats.SpriteModel;
        Cursor.visible = true;
    }

    public void PickUpFish()
    {
        Inventory.Instance.AddFish(fishStats);
        Inventory.Instance.CurrentRod.currentFish = null;
        fishStats = null;
        gameObject.SetActive(false);
        Cursor.visible = false;
    }

    public void DropFish()
    {
        gameObject.SetActive(false);
        Inventory.Instance.CurrentRod.currentFish = null;
        fishStats = null;
        Cursor.visible = false;
        
    }
}
