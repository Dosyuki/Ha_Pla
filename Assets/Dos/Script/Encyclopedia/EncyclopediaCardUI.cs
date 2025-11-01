// แก้ไขไฟล์: EncyclopediaCardUI.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class EncyclopediaCardUI : MonoBehaviour
{
    [Header("Card Identity")]
    [Tooltip("ID ของปลาที่ Card นี้จะแสดงผล (ต้องตรงกับ BaseFish.Name)")]
    public string fishID; // <-- *** เพิ่มบรรทัดนี้ ***

    [Header("UI References")]
    [SerializeField] private Image fishPhoto; 
    [SerializeField] private TMP_Text fishNameText;
    [SerializeField] private TMP_Text fishWeightText;
    [SerializeField] private GameObject undiscoveredOverlay; 

    // ฟังก์ชัน UpdateDisplay และ LoadFishPhoto ไม่ต้องแก้ไข
    // (เหมือนเดิมจากโค้ดก่อนหน้า)
    public void UpdateDisplay(EncyclopediaEntry entry)
    {
        if (entry == null) return;

        if (!entry.isDiscovered)
        {
            // --- ยังไม่ค้นพบ ---
            undiscoveredOverlay.SetActive(true);
            fishNameText.text = "???"; // (เราอาจจะเปลี่ยนเป็น fishID ก็ได้ถ้าอยากโชว์ชื่อลางๆ)
            fishWeightText.text = "--- Kg";
            fishPhoto.sprite = null; 
        }
        else
        {
            // --- ค้นพบแล้ว ---
            undiscoveredOverlay.SetActive(false);
            fishNameText.text = entry.baseFishName; // (นี่คือชื่อจาก Database)
            fishWeightText.text = $"{entry.bestWeight:F2} Kg";
            
            LoadFishPhoto(entry.bestPhotoGUID);
        }
    }

    private void LoadFishPhoto(string guid)
    {
        if (string.IsNullOrEmpty(guid))
        {
            fishPhoto.sprite = null; 
            return;
        }
        
        int currentSlot = GameSession.Instance.CurrentSlotId;
        if (currentSlot <= 0) return; 

        string folderPath = Path.Combine(Application.persistentDataPath, $"SlotPhotos_{currentSlot}");
        string filePath = Path.Combine(folderPath, $"{guid}.png");

        if (!File.Exists(filePath))
        {
            fishPhoto.sprite = null;
            return;
        }

        try
        {
            byte[] fileData = File.ReadAllBytes(filePath);
            Texture2D tex = new Texture2D(2, 2);
            if (tex.LoadImage(fileData))
            {
                Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
                fishPhoto.sprite = sprite;
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Encyclopedia: โหลดรูปไม่สำเร็จ: {ex.Message}");
            fishPhoto.sprite = null;
        }
    }
}