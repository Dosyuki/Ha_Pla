using UnityEngine;

public class PlayerQuestInteract : MonoBehaviour
{
    void Update()
    {
        switch (UIManager.Instance.GetCurrentState())
        {
            case currentState.None:
                if( Input.GetKeyDown(KeyCode.R))
                    QuestCardManager.Instance.OpenQuestBookPanel();
                break;
            case currentState.UI:
                if ( Input.GetKeyDown(KeyCode.R))
                    QuestCardManager.Instance.CloseQuestBookPanel();
                break;
        }
    }
}
