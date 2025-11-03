using UnityEngine;
using System.Collections.Generic;

public class EncyclopediaUI : MonoBehaviour
{


    private EncyclopediaCardUI[] allCardsInScene;

    private void Awake()
    {
        
        allCardsInScene = GetComponentsInChildren<EncyclopediaCardUI>(true); // true = ค้นหาตัวที่ปิดอยู่ด้วย
    }

    private void OnEnable()
    {
        if (EncyclopediaManager.Instance == null)
        {
            Debug.LogError("EncyclopediaManager ไม่พร้อมใช้งาน!");
            return;
        }
        
        RefreshAllCards();
    }

    private void RefreshAllCards()
    {
        Dictionary<string, EncyclopediaEntry> database = EncyclopediaManager.Instance.GetDatabase();

        foreach (EncyclopediaCardUI card in allCardsInScene)
        {
            string id = card.fishID;

            if (database.TryGetValue(id, out EncyclopediaEntry entry))
            {
                card.UpdateDisplay(entry);
            }
            else
            {
                Debug.LogWarning($"ไม่พบข้อมูลสำหรับ Card ที่มี ID: {id}");
            }
        }
    }
}