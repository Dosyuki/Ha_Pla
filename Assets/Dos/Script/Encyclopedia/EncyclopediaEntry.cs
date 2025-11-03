[System.Serializable]
public class EncyclopediaEntry
{
    public string baseFishName; // ID ของปลา (เช่น "BirdterflyFish")
    public bool isDiscovered; // ค้นพบหรือยัง (จับได้ครั้งแรก)
    public float bestWeight; // น้ำหนักสูงสุดที่เคยจับได้
    public string bestPhotoGUID; // GUID ของรูปภาพที่หนักที่สุด
    
    // Constructor สำหรับสร้าง "ช่องว่าง"
    public EncyclopediaEntry(string fishName)
    {
        baseFishName = fishName;
        isDiscovered = false;
        bestWeight = 0f;
        bestPhotoGUID = string.Empty;
    }
}