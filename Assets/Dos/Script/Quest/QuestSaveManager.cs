using UnityEngine;
using System.IO;

[System.Serializable]
public class QuestSaveData
{
    public bool slot1_Done;
    public bool slot2_Done;
    public bool slot3_Done;
    public bool slot4_Done;
}

public class QuestSaveManager : Singleton<QuestSaveManager>
{
    public QuestSaveData data = new QuestSaveData();
    private string savePath;

    void Awake()
    {
        savePath = Path.Combine(Application.persistentDataPath, "quest_status.json");
        LoadQuestData();
    }

    public bool IsQuestCompleted(string questID)
    {
        return questID switch
        {
            "Quest1" => data.slot1_Done,
            "Quest2" => data.slot2_Done,
            "Quest3" => data.slot3_Done,
            "Quest4" => data.slot4_Done,
            _ => false
        };
    }

    public void CompleteQuest(string questID)
    {
        if (questID == "Quest1") data.slot1_Done = true;
        if (questID == "Quest2") data.slot2_Done = true;
        if (questID == "Quest3") data.slot3_Done = true;
        if (questID == "Quest4") data.slot4_Done = true;
        SaveQuestData();
    }

    public void SaveQuestData()
    {
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(savePath, json);
    }

    public void LoadQuestData()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            data = JsonUtility.FromJson<QuestSaveData>(json);
        }
    }
}