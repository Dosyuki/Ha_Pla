using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class NPCInteraction : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private DialogueData lockedDialogue; 
    [SerializeField] private UnityEvent onQuestDone;

    private bool isInRange = false;

    private void Update()
    {
        if (!isInRange)
            return;
        if (Input.GetKeyDown(KeyCode.F))
        {
            Interact();
        }
        
    }

    public void Interact()
    {
        if (lockedDialogue.questToOpen != null)
        {
            if(!lockedDialogue.questToOpen.isQuestDone())
                QuestUIManager.Instance.StartDialogue(lockedDialogue);
            else if (lockedDialogue.questToOpen.isQuestDone())
            {
                onQuestDone.Invoke();
                Debug.Log("Quest done, Try to Do  Something");
            }
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