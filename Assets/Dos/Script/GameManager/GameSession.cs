// ไฟล์: GameSession.cs (ตรวจสอบว่ามีโค้ดนี้)
using UnityEngine;

public class GameSession : Singleton<GameSession>
{
    public int CurrentSlotId { get; private set; } = 0;

    // --- *** นี่คือส่วนที่สำคัญที่สุด *** ---
    protected void Awake()
    {
        
        // และสั่งให้ตัวเอง "ไม่ถูกทำลาย" เมื่อโหลด Scene ใหม่
        DontDestroyOnLoad(this.gameObject);
    }
    // --- *** จบส่วนสำคัญ *** ---

    /// <summary>
    /// ฟังก์ชันนี้จะถูกเรียกโดยปุ่มใน Main Menu (ทั้ง Load และ New)
    /// </summary>
    public void SetCurrentSlot(int slotId)
    {
        CurrentSlotId = slotId;
        Debug.Log($"GameSession: ตั้งค่า Slot ปัจจุบันเป็น {CurrentSlotId}");
    }
}