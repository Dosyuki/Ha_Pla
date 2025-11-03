using System;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] private FirstPersonController m_FirstPersonController;
    [SerializeField] private currentState m_currentState;
    private float timeEnteredNoneState = -1f; // เวลาที่เราเพิ่งเข้าสู่ State 'None'
    private const float pauseBufferTime = 0.1f; // บัฟเฟอร์ 0.1 วินาที (กันการกดชนกัน)
    private void Awake()
    {
        m_FirstPersonController  = FindObjectOfType<FirstPersonController>();
        m_currentState = currentState.None;
        ChangeState(m_currentState);
    }

    public void ChangeState(currentState newState)
    {
        if (newState == currentState.None && m_currentState == currentState.UI)
        {
            // ให้บันทึกเวลาปัจจุบันไว้
            timeEnteredNoneState = Time.unscaledTime; // (ใช้ unscaledTime เผื่อเกม Pause)
        }
        switch (newState)
        {
            case currentState.None:
                m_currentState = newState;
                m_FirstPersonController.SetCanMove(true);
                m_FirstPersonController.GetMouseLook().SetCursorLock(true);
                Inventory.Instance.CurrentRod.HideSliderCanvas(false);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = false;
                break;
            case currentState.UI:
                m_currentState = newState;
                m_FirstPersonController.SetCanMove(false);
                m_FirstPersonController.GetMouseLook().SetCursorLock(false);
                Inventory.Instance.CurrentRod.HideSliderCanvas(true);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                break;
        }
    }
    public bool CanOpenPauseMenu()
    {
        // 1. ถ้า State ไม่ใช่ None (เช่น Shop เปิดอยู่) = เปิด Pause ไม่ได้
        if (m_currentState != currentState.None)
            return false;
            
        // 2. ถ้าเรา "เพิ่ง" เข้าสู่ State None (ยังอยู่ในบัฟเฟอร์ 0.1 วิ) = เปิด Pause ไม่ได้
        if (Time.unscaledTime < timeEnteredNoneState + pauseBufferTime)
            return false;
            
        // 3. ถ้าผ่าน 2 ข้อบนมาได้ = เปิด Pause ได้
        return true;
    }
    public currentState GetCurrentState() => m_currentState;
}

public enum currentState
{
    None,
    UI
}
