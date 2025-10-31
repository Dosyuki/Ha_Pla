// ไฟล์ใหม่: GameSession.cs
using UnityEngine;

// ใช้ Singleton.cs ที่คุณมี
// เราจะปรับแต่ง Singleton นี้เล็กน้อยให้เป็น DontDestroyOnLoad
public class GameSession : Singleton<GameSession>
{
    // เราจะเก็บ Slot ID ที่กำลังเล่นอยู่ไว้ที่นี่
    public int CurrentSlotId { get; private set; } = 0; // เริ่มต้นเป็น 0 (ยังไม่เลือก)

    protected void Awake()
    {
        // และสั่งให้ตัวเอง "ไม่ถูกทำลาย" เมื่อโหลด Scene ใหม่
        DontDestroyOnLoad(this.gameObject);
    }

    /// <summary>
    /// ฟังก์ชันนี้จะถูกเรียกโดยปุ่มใน Main Menu
    /// </summary>
    public void SetCurrentSlot(int slotId)
    {
        CurrentSlotId = slotId;
        Debug.Log($"GameSession: ตั้งค่า Slot ปัจจุบันเป็น {CurrentSlotId}");
    }
}