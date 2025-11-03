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
        if (Input.GetKeyDown(KeyCode.Escape))
        {
           
            if (isPaused)
            {
                isPaused = false;
                pauseMenuCanvasGroup.alpha = 0;
                pauseMenuCanvasGroup.interactable = false;
                pauseMenuCanvasGroup.blocksRaycasts = false;
                UIManager.Instance.ChangeState(currentState.None);
            }
            else
            {
                
                if (UIManager.Instance.CanOpenPauseMenu())
                {
             
                    isPaused = true;
                    pauseMenuCanvasGroup.alpha = 1;
                    pauseMenuCanvasGroup.interactable = true;
                    pauseMenuCanvasGroup.blocksRaycasts = true;
                    UIManager.Instance.ChangeState(currentState.UI);
                }
      
            }
        }
    }

    public void OnClickSaveButton()
    {
        if (GameSession.Instance == null)
        {
            Debug.LogError("ไม่พบ GameSession! ไม่สามารถ Save ได้");
            return;
        }

        int slotToSave = GameSession.Instance.CurrentSlotId;

        if (slotToSave <= 0)
        {
            Debug.LogError($"Slot ID ไม่ถูกต้อง ({slotToSave})! ไม่สามารถ Save ได้");
            return;
        }

        Debug.Log($"PauseMenu: กำลัง Save เกมลง Slot {slotToSave}...");
        SaveManager.Instance.SaveGame(slotToSave);
        
    }

    public void OnClickQuitToMenu()
    {
        SceneManager.LoadScene("MainMenu"); 
    }
}