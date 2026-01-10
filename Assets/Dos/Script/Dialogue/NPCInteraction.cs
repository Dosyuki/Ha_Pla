using System;
using Unity.VisualScripting;
using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private DialogueData dialogueData; // ลาก Dialogue SO มาใส่

    private bool isInRange = false;

    private void Update()
    {
        if (!isInRange)
            return;
        if (Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
        
    }

    // ฟังก์ชันนี้เรียกเมื่อ Player เดินไปกด E หรือคลิกใส่ NPC
    public void Interact()
    {
        if (dialogueData != null)
        {
            QuestUIManager.Instance.StartDialogue(dialogueData);
            UIManager.Instance.ChangeState(currentState.UI);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        isInRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        isInRange = false;
    }
}