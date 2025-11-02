// ตัวอย่างไฟล์: PauseMenu.cs

using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private CanvasGroup pauseMenuCanvasGroup;
    bool isPaused = false;

    private void Start()
    {
        pauseMenuCanvasGroup.alpha = 0;
        pauseMenuCanvasGroup.interactable = false;
        pauseMenuCanvasGroup.blocksRaycasts = false;
    }

    private void Update()
    {
        // ตรวจสอบแค่ปุ่ม Esc
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // --- ตรรกะใหม่ ---

            // กรณีที่ 1: ถ้า Pause Menu "เปิดอยู่"
            if (isPaused)
            {
                // Action: ปิด Pause Menu
                isPaused = false;
                pauseMenuCanvasGroup.alpha = 0;
                pauseMenuCanvasGroup.interactable = false;
                pauseMenuCanvasGroup.blocksRaycasts = false;
                UIManager.Instance.ChangeState(currentState.None);
            }
            // กรณีที่ 2: ถ้า Pause Menu "ยังไม่เปิด"
            else
            {
                // Action: "ถาม" UIManager ก่อนว่าเปิดได้ไหม
                // (CanOpenPauseMenu จะเช็ค State == None และ เช็ค Buffer Time ให้เรา)
                if (UIManager.Instance.CanOpenPauseMenu())
                {
                    // ถ้าเปิดได้ (เราอยู่ในเกมปกติ ไม่ใช่เพิ่งปิด Shop)
                    // Action: เปิด Pause Menu
                    isPaused = true;
                    pauseMenuCanvasGroup.alpha = 1;
                    pauseMenuCanvasGroup.interactable = true;
                    pauseMenuCanvasGroup.blocksRaycasts = true;
                    UIManager.Instance.ChangeState(currentState.UI);
                }
                // ถ้าเปิดไม่ได้ (เช่น เพิ่งปิด Shop/Inventory มา)
                // ... ก็ไม่ต้องทำอะไร (ปุ่ม Esc จะถูก "ซับ" ไป)
            }
        }
    }

    public void OnClickSaveButton()
    {
        // 1. ตรวจสอบว่า GameSession อยู่หรือไม่
        if (GameSession.Instance == null)
        {
            Debug.LogError("ไม่พบ GameSession! ไม่สามารถ Save ได้");
            return;
        }

        // 2. ดึง Slot ID ปัจจุบันมาจาก GameSession
        int slotToSave = GameSession.Instance.CurrentSlotId;

        // 3. ตรวจสอบว่าเรามี Slot ที่ถูกต้อง
        if (slotToSave <= 0)
        {
            Debug.LogError($"Slot ID ไม่ถูกต้อง ({slotToSave})! ไม่สามารถ Save ได้");
            return;
        }

        // 4. สั่ง SaveManager ให้ Save ลง Slot นั้น
        Debug.Log($"PauseMenu: กำลัง Save เกมลง Slot {slotToSave}...");
        SaveManager.Instance.SaveGame(slotToSave);
        
        // (อาจจะแสดง UI ว่า "Save Complete!")
    }

    public void OnClickQuitToMenu()
    {
        // (สำคัญ) ถ้าจะกลับ Main Menu ควรทำลาย GameSession ทิ้ง
        // หรือจัดการให้ดีว่า Main Menu จะรับมือยังไงถ้าเจอ GameSession เก่า
        // วิธีที่ง่ายที่สุดคือโหลด Main Menu Scene ใหม่เลย
        SceneManager.LoadScene("MainMenu"); 
    }
}