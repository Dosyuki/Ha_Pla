using UnityEngine;

public class PlayerMenuInteract : MonoBehaviour
{
    public CanvasGroup mapCanvasGroup;
    void Update()
    {
        switch (UIManager.Instance.GetCurrentState())
        {  
            case currentState.None:
                if( Input.GetKeyDown(KeyCode.J))
                    QuestCardManager.Instance.OpenQuestBookPanel();
                else if (Input.GetKeyDown(KeyCode.M))
                    UIManager.Instance.ToggleCanvasGroup(mapCanvasGroup,true);
                    
                break;
            case currentState.UI:
                if ( Input.GetKeyDown(KeyCode.J))
                    QuestCardManager.Instance.CloseQuestBookPanel();
                else if (Input.GetKeyDown(KeyCode.M))
                    UIManager.Instance.ToggleCanvasGroup(mapCanvasGroup,false);
                break;
        }
    }
}
