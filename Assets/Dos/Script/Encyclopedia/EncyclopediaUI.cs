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
        EncyclopediaManager.OnDatabaseUpdated += RefreshAllCards;
        
        RefreshAllCards();
    }
    private void OnDisable()
    {
        EncyclopediaManager.OnDatabaseUpdated -= RefreshAllCards;
    }

    private void RefreshAllCards()
    {
        // (Log 1 ของคุณบอกว่าฟังก์ชันนี้เริ่มทำงาน)
        
        // 4. ตรวจสอบว่า Manager พร้อมหรือยัง
        if (EncyclopediaManager.Instance == null) return;
        
        // (Log 2 ของคุณบอกว่า GetDatabase ทำงาน)
        Dictionary<string, EncyclopediaEntry> database = EncyclopediaManager.Instance.GetDatabase();

        if (database == null) return; // (ป้องกัน Error ถ้ายังไม่ Initialize)

        foreach (EncyclopediaCardUI card in allCardsInScene)
        {
            string id = card.fishID;
            
            // (Log 3-4 ของคุณบอกว่า TryGetValue ทำงาน)
            if (database.TryGetValue(id, out EncyclopediaEntry entry))
            {
                // 5. ส่งข้อมูล (ที่อัปเดตแล้ว) ไปให้ Card
                card.UpdateDisplay(entry);
            }
            else
            {
                // (ไม่ควรเกิด ถ้าคุณตั้งค่า Inspector ถูก)
            }
        }
    }
}