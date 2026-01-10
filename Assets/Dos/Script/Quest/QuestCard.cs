using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestCard : MonoBehaviour
{
    public Image questImage;
    public TMP_Text questDesc;
    public QuestData questData;
    public void UpdateCardInfo(QuestData quest)
    {
        questData = quest;
        var req = questData.requirements[0];
        questDesc.text = $"{questData.questTitle}";
        questImage.sprite = req.specificFish.SpriteModel;
    }

    public void OpenQuestPanels()
    {
        QuestUIManager.Instance.OpenQuestWindow(questData);
    }
}
