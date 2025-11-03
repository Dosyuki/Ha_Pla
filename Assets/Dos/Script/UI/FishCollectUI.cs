using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.IO; // เพิ่มเข้ามาเพื่อจัดการไฟล์

public class FishCollectUI : MonoBehaviour
{
    public Fish fishStats;
    public TMP_Text fishName;
    public TMP_Text fishWeight;
    public Image fishSprite;

    public bool isOpen;

    [Header("UI References")]
    [Tooltip("ปุ่มสำหรับถ่ายรูป (ลากมาใส่)")]
    [SerializeField] private Button takePhotoButton; // <-- เพิ่มตัวแปรสำหรับปุ่ม

    [Header("Photo Capture Setup (ต้องตั้งค่า)")]
    [Tooltip("กล้องตัวที่สองสำหรับถ่ายรูป (ต้องตั้งค่า Culling Mask และ Output Texture)")]
    [SerializeField] private Camera photoCamera;
    
    [Tooltip("Render Texture ที่กล้อง (ด้านบน) จะบันทึกภาพลงไป")]
    [SerializeField] private RenderTexture photoRenderTexture;
    
    [Tooltip("ตำแหน่ง (Empty GameObject) ที่จะวางปลาเพื่อถ่ายรูป")]
    [SerializeField] private Transform photoShootLocation;
    
    [Tooltip("ชื่อ Layer ที่ตั้งค่าไว้สำหรับปลา (เช่น 'FishPhotoLayer')")]
    [SerializeField] private string fishPhotoLayerName = "FishPhotoLayer";
    
    private int fishLayer;

    private void Start()
    {
        // แปลงชื่อ Layer (string) เป็น Layer (int) ตอนเริ่มเกม
        fishLayer = LayerMask.NameToLayer(fishPhotoLayerName);
        if (fishLayer == -1)
        {
            Debug.LogError($"Layer '{fishPhotoLayerName}' ไม่ได้ถูกสร้าง! กรุณาสร้าง Layer นี้ใน Project Settings -> Tags and Layers");
        }
    }

    public void UpdateFish(Fish fish)
    {
        isOpen = true;
        fishStats = fish;
        gameObject.SetActive(true);
        UIManager.Instance.ChangeState(currentState.UI);
        fishName.text = fishStats.Name;
        fishWeight.text = $"{fishStats.Weight:F2} KGs";
        fishSprite.sprite = fishStats.SpriteModel; 
        Cursor.visible = true;

        // --- เพิ่มส่วนนี้ ---
        // รีเซ็ตปุ่มถ่ายรูปให้กดได้ทุกครั้งที่ปลาตัวใหม่โผล่มา
        if (takePhotoButton != null)
        {
            takePhotoButton.interactable = true;
        }
    }

    /// <summary>
    /// ฟังก์ชันใหม่: สำหรับให้ปุ่ม "ถ่ายรูป" เรียกใช้
    /// </summary>
    public void OnClick_TakePhoto()
    {
        if (fishStats == null) return; // ป้องกัน Error

        // ตรวจสอบว่าเคยถ่ายรูปปลาตัวนี้ไปแล้วหรือยัง (ถ้า photoGUID ไม่ว่าง)
        if (!string.IsNullOrEmpty(fishStats.photoGUID))
        {
            Debug.Log("คุณได้ถ่ายรูปปลาตัวนี้ไปแล้ว");
            return;
        }

        // 1. ถ่ายรูปปลา และรับ "ชื่อไฟล์" (GUID) กลับมา
        string photoGUID = TakeFishPhotoAndSave(fishStats.PrefabModel);

        // 2. บันทึกชื่อไฟล์ (GUID) ลงในข้อมูลของปลา (ที่ยังอยู่ใน UI นี้)
        if (!string.IsNullOrEmpty(photoGUID))
        {
            // เราอัปเดต fishStats ที่อยู่ใน UI นี้
            fishStats.photoGUID = photoGUID; // (ต้องมีตัวแปร 'public string photoGUID;' ในคลาส Fish.cs)
            Debug.Log($"บันทึกรูปปลาสำเร็จ: {photoGUID}.png");

            // --- เพิ่มส่วนนี้ ---
            // ปิดการใช้งานปุ่มหลังจากถ่ายสำเร็จ
            if (takePhotoButton != null)
            {
                takePhotoButton.interactable = false;
            }
        }
        else
        {
            Debug.LogWarning("ไม่สามารถบันทึกรูปปลาได้ (Photo Capture Setup อาจจะยังไม่ได้ตั้งค่า)");
        }
        SaveManager.Instance.SaveGame(GameSession.Instance.CurrentSlotId);
        PickUpFish();
    }

    /// <summary>
    /// ฟังก์ชันสำหรับปุ่ม "เก็บปลา" (Pick Up)
    /// </summary>
    public void PickUpFish()
    {
        if (Inventory.Instance.isMaxFish)
        {
            Debug.LogWarning("Inventory max");
            return;
        }

        // --- โค้ดถ่ายรูปถูกย้ายออกไปแล้ว ---
        
        isOpen = false;
        // เพิ่มปลา (fishStats ที่อาจจะมี หรือไม่มี photoGUID ก็ได้) ลงใน Inventory
        Inventory.Instance.AddFish(fishStats); 
        Inventory.Instance.CurrentRod.currentFish = null;
        fishStats = null;
        gameObject.SetActive(false);
        UIManager.Instance.ChangeState(currentState.None);
        SaveManager.Instance.SaveGame(GameSession.Instance.CurrentSlotId);

    }

    public void DropFish()
    {
        isOpen = false;
        gameObject.SetActive(false);
        Inventory.Instance.CurrentRod.currentFish = null;
        fishStats = null;
        UIManager.Instance.ChangeState(currentState.None);
        SaveManager.Instance.SaveGame(GameSession.Instance.CurrentSlotId);

    }

    // -------------------------------------------------------------------
    // ฟังก์ชันสำหรับถ่ายรูป (เหมือนเดิม แต่ตอนนี้เป็น private)
    // -------------------------------------------------------------------

    private string TakeFishPhotoAndSave(GameObject fishPrefab)
    {
        // 1. ตรวจสอบว่าระบบพร้อมหรือไม่
        if (fishPrefab == null || photoCamera == null || photoRenderTexture == null || photoShootLocation == null || fishLayer == -1)
        {
            Debug.LogError("Photo Capture Setup ไม่สมบูรณ์! ไม่สามารถถ่ายรูปได้");
            return null;
        }

        // 2. ตรวจสอบ Slot ปัจจุบัน
        if (GameSession.Instance == null || GameSession.Instance.CurrentSlotId <= 0)
        {
            Debug.LogError("ไม่พบ Slot ID ปัจจุบัน! ไม่สามารถเซฟรูปได้");
            return null;
        }
        int currentSlot = GameSession.Instance.CurrentSlotId;


        // 3. สร้างปลาจำลองใน "สตูดิโอ"
        GameObject fishInstance = Instantiate(fishPrefab, photoShootLocation.position, photoShootLocation.rotation);
        fishInstance.transform.localScale =
            fishPrefab.transform.localScale * (fishStats.Weight / fishStats.GetBaseFish().Weight);
        // 4. ตั้งค่า Layer ให้ปลา (และลูกๆ ของมัน) เพื่อให้กล้องมองเห็น
        SetLayerRecursively(fishInstance, fishLayer);

        // 5. สั่งให้กล้องถ่าย (Render) 1 เฟรม
        photoCamera.Render();

        // 6. แปลงผลลัพธ์จาก RenderTexture เป็น Texture2D
        Texture2D tex2D = new Texture2D(photoRenderTexture.width, photoRenderTexture.height, TextureFormat.RGBA32, false);
        RenderTexture.active = photoRenderTexture; 
        tex2D.ReadPixels(new Rect(0, 0, tex2D.width, tex2D.height), 0, 0); 
        tex2D.Apply();
        RenderTexture.active = null; 

        // 7. แปลง Texture2D เป็นไฟล์ PNG (byte array)
        byte[] pngData = tex2D.EncodeToPNG();
        Destroy(tex2D); 

        // 8. ทำลายปลาจำลองทิ้ง
        Destroy(fishInstance);

        // 9. สร้างชื่อไฟล์และที่อยู่
        string guid = System.Guid.NewGuid().ToString(); 
        string folderPath = Path.Combine(Application.persistentDataPath, $"SlotPhotos_{currentSlot}");
        string filePath = Path.Combine(folderPath, $"{guid}.png");

        try
        {
            // 10. สร้างโฟลเดอร์ (ถ้ายังไม่มี)
            Directory.CreateDirectory(folderPath);
            
            // 11. เขียนไฟล์ PNG ลงในเครื่อง
            File.WriteAllBytes(filePath, pngData);
            
            // 12. คืนค่าชื่อไฟล์ (GUID) กลับไป
            return guid;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"เกิดข้อผิดพลาดในการเขียนไฟล์รูปปลา: {ex.Message}");
            return null;
        }
    }

    private void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }
}