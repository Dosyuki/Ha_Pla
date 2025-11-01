// ตัวอย่างไฟล์: PauseMenu.cs

using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    bool isPaused = false;
    private void Update()
    {
        if (UIManager.Instance.GetCurrentState() != currentState.UI
            && Input.GetKeyDown(KeyCode.Escape))
        {
            isPaused = true;
            pauseMenu.SetActive(true);
            UIManager.Instance.ChangeState(currentState.UI);
        }
        else if (UIManager.Instance.GetCurrentState() == currentState.UI
                 && Input.GetKeyDown(KeyCode.Escape))
        {
            isPaused = false;
            pauseMenu.SetActive(false);
            UIManager.Instance.ChangeState(currentState.None);
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