// แก้ไขไฟล์: EncyclopediaUI.cs (เวอร์ชันใหม่)
using UnityEngine;
using System.Collections.Generic;

public class EncyclopediaUI : MonoBehaviour
{
    // เราไม่ต้องการ cardPrefab หรือ cardHolder อีกต่อไป
    // เพราะเราจะวาง Card เองใน Scene

    private EncyclopediaCardUI[] allCardsInScene;

    private void Awake()
    {
        // 1. ค้นหา Card ทั้งหมดที่ "เป็นลูก" ของ Panel นี้
        // (วิธีนี้ทำให้คุณจัด Layout ยังไงก็ได้ ขอแค่มันอยู่ข้างใน)
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
        // 2. ดึงฐานข้อมูล "ข้อมูลจริง" จาก Manager
        Dictionary<string, EncyclopediaEntry> database = EncyclopediaManager.Instance.GetDatabase();

        // 3. วนลูป Card "ทุกใบที่อยู่ใน Scene"
        foreach (EncyclopediaCardUI card in allCardsInScene)
        {
            // 4. ดูว่า Card ใบนี้ "ประกาศ" ว่าตัวเองเป็นปลาอะไร (จาก fishID ที่เราจะตั้งใน Inspector)
            string id = card.fishID;

            // 5. ค้นหา "ข้อมูล" ของปลาตัวนั้นในฐานข้อมูล
            if (database.TryGetValue(id, out EncyclopediaEntry entry))
            {
                // 6. ถ้าเจอ -> ส่งข้อมูลไปให้ Card อัปเดตตัวเอง
                card.UpdateDisplay(entry);
            }
            else
            {
                // 7. ถ้าไม่เจอ (เช่น พิมพ์ชื่อผิดใน Inspector)
                Debug.LogWarning($"ไม่พบข้อมูลสำหรับ Card ที่มี ID: {id}");
                // (อาจจะซ่อน Card ใบนี้ไปเลย)
                // card.gameObject.SetActive(false);
            }
        }
    }
}