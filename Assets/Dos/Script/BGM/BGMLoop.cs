using UnityEngine;

// (โค้ดนี้อ้างอิงถึง Singleton.cs ที่คุณมีอยู่)
public class BGMLoop : Singleton<BGMLoop> 
{
    public AudioSource audioSource;
    public AudioClip audioClip;

    // --- 1. เพิ่มฟังก์ชัน Awake() นี้เข้าไป ---
    protected void Awake()
    {
        // 1. "ปลุก" Instance property (การเรียก .Instance จะไปกระตุ้น 'get' accessor ใน Singleton.cs)
        // ตัวแรกที่ตื่นขึ้นมา จะถูกกำหนดให้เป็น _instance (ถ้า base class ของคุณตั้งค่าไว้)
        // หรือถูก "ค้นหา" (ถ้าใช้ FindObjectsOfType)
        
        // (แก้ไข: อ้างอิงจาก Singleton.cs ที่คุณใช้ 
        // ซึ่งเป็นแบบ "Find" เราต้องเรียกใช้ Instance เพื่อบังคับให้มันค้นหา)
        var mainInstance = Instance; 

        // 2. ตรวจสอบ
        // ถ้า Instance ที่ 'get' คืนมา "ไม่ใช่" GameObject นี้
        // แปลว่าเราเป็น "ตัวซ้ำ" ที่ถูกสร้างขึ้นมาใหม่ตอนโหลด Scene
        if (mainInstance != this)
        {
            // หยุดเล่นเพลง (ถ้าเผลอเล่น) และทำลายตัวเองทิ้ง
            if (audioSource != null)
            {
                audioSource.Stop();
            }
            Destroy(this.gameObject);
            return; // จบการทำงาน
        }

        // 3. ถ้าเราคือ "ตัวจริง" (Instance == this)
        // สั่งให้เรา "รอดชีวิต" ข้าม Scene
        DontDestroyOnLoad(this.gameObject);
        
        // (ถ้า base class ของคุณ ไม่ได้ตั้งค่า _instance ใน Awake 
        // คุณอาจจะต้องเพิ่ม base.Awake() แต่จากไฟล์ที่คุณให้มา ไม่จำเป็น)
    }

    // --- 2. โค้ด Start() ของคุณ (เหมือนเดิม) ---
    void Start()
    {
        // เช็คอีกครั้ง (เผื่อไว้) แต่ตอนนี้ Awake() ควรจะจัดการตัวซ้ำไปแล้ว
        if(!audioSource.isPlaying)
            audioSource.Play();
    }
}