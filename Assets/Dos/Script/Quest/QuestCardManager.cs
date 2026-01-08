using System;
using System.Collections.Generic;
using UnityEngine;

public class QuestCardManager : Singleton<QuestCardManager>
{
    [Serializable]
    public struct QuestPage
    {
        public string title;
        public List<QuestData> QuestDatas;
    }
    public List<QuestPage> questPage;
    public List<QuestCard> questCards;
    public int CurrentPage = 0;
    public CanvasGroup QuestBookPanel;
    public bool isOpen;
    public void OpenQuestBookPanel()
    {
        UIManager.Instance.ChangeState(currentState.UI);
        UIManager.Instance.ToggleCanvasGroup(QuestBookPanel,true);
        List<QuestData> questDatas = questPage[CurrentPage].QuestDatas;
        UpdateQuestCards();
        isOpen = true;
    }

    public void UpdateQuestCards()
    {
        for (int i = 0; i < questCards.Count; i++)
        {
            questCards[i].UpdateCardInfo(questPage[CurrentPage].QuestDatas[i]);
        }
    }
    public void CloseQuestBookPanel()
    {
        UIManager.Instance.ToggleCanvasGroup(QuestBookPanel, false);
        UIManager.Instance.ChangeState(currentState.None);
        isOpen = false;
    }
    public void NextPage()
    {
        if (CurrentPage < questPage.Count - 1)
        {
            CurrentPage++;
            UpdateQuestCards();
        }
    }
    
    public void PrevPage()
    {
        if (CurrentPage > 0)
        {
            CurrentPage--;
            UpdateQuestCards();
        }
    }
}
