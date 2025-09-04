using System;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] private FirstPersonController m_FirstPersonController;
    [SerializeField] private currentState m_currentState;
    private void Awake()
    {
        m_FirstPersonController  = FindObjectOfType<FirstPersonController>();
        m_currentState = currentState.None;
        ChangeState(m_currentState);
    }

    public void ChangeState(currentState newState)
    {
        switch (newState)
        {
            case currentState.None:
                m_currentState = newState;
                m_FirstPersonController.SetCanMove(true);
                m_FirstPersonController.GetMouseLook().SetCursorLock(true);
                Inventory.Instance.CurrentRod.HideSliderCanvas(false);
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
    public currentState GetCurrentState() => m_currentState;
}

public enum currentState
{
    None,
    UI
}
