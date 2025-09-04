using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FishCollectUI : MonoBehaviour
{
    public Fish fishStats;
    public TMP_Text fishName;
    public TMP_Text fishWeight;
    public Image fishSprite;

    public bool isOpen;
    public void UpdateFish(Fish fish)
    {
        isOpen = true;
        fishStats = fish;
        gameObject.SetActive(true);
        UIManager.Instance.ChangeState(currentState.UI);
        fishName.text = fishStats.Name;
        fishWeight.text = fishStats.Weight.ToString();
        fishSprite.sprite = fishStats.SpriteModel;
        Cursor.visible = true;
    }

    public void PickUpFish()
    {
        if (Inventory.Instance.isMaxFish)
        {
            Debug.LogWarning("Inventory max");
            return;
        }
        isOpen = false;
        Inventory.Instance.AddFish(fishStats);
        Inventory.Instance.CurrentRod.currentFish = null;
        fishStats = null;
        gameObject.SetActive(false);
        UIManager.Instance.ChangeState(currentState.None);
    }

    public void DropFish()
    {
        isOpen = false;
        gameObject.SetActive(false);
        Inventory.Instance.CurrentRod.currentFish = null;
        fishStats = null;
        UIManager.Instance.ChangeState(currentState.None);
    }
}
