using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq; // ใช้สำหรับการคำนวณ List ง่ายๆ

public class QuestUIManager : Singleton<QuestUIManager>
{
    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text speakerNameText;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private Button nextButton;
    
    [Header("Quest UI")]
    [SerializeField] private GameObject questPanel;
    [SerializeField] private TMP_Text questTitle;
    [SerializeField] private TMP_Text questDesc;
    [SerializeField] private Transform slotsParent;
    [SerializeField] private GameObject fishSlotPrefab; // Prefab เดียวกับ Inventory
    [SerializeField] private Button submitButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private TMP_Text requirementStatusText; // Text บอกว่าขาดอะไรบ้าง

    // Runtime Variables
    private Queue<string> sentencesQueue = new Queue<string>();
    private QuestData currentActiveQuest;
    private List<Fish> selectedFish = new List<Fish>();
    private List<GameObject> currentSlotObjs = new List<GameObject>();

    private void Start()
    {
        dialoguePanel.SetActive(false);
        questPanel.SetActive(false);
        nextButton.onClick.AddListener(DisplayNextSentence);
        submitButton.onClick.AddListener(SubmitQuest);
        closeButton.onClick.AddListener(CloseQuestWindow);
    }

    // --- DIALOGUE SYSTEM ---
    public void StartDialogue(DialogueData data)
    {
        currentActiveQuest = data.questToOpen;
        
        dialoguePanel.SetActive(true);
        questPanel.SetActive(false);
        
        speakerNameText.text = data.speakerName;
        sentencesQueue.Clear();

        foreach (string sentence in data.sentences)
        {
            sentencesQueue.Enqueue(sentence);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (sentencesQueue.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence = sentencesQueue.Dequeue();
        dialogueText.text = sentence;
    }

    private void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        UIManager.Instance.ChangeState(currentState.None);
        if (currentActiveQuest != null)
        {
            OpenQuestWindow(currentActiveQuest);
            UIManager.Instance.ChangeState(currentState.UI);
        }
    }

    // --- QUEST SYSTEM ---
    public void OpenQuestWindow(QuestData quest)
    {
        currentActiveQuest = quest;
        questPanel.SetActive(true);
        questTitle.text = quest.questTitle;
        questDesc.text = quest.description;
        
        selectedFish.Clear();
        RefreshFilteredInventory();
        UpdateRequirementText();
    }

    private void RefreshFilteredInventory()
    {
        foreach (var obj in currentSlotObjs) Destroy(obj);
        currentSlotObjs.Clear();

        List<Fish> allFish = Inventory.Instance.GetAllFish();

        foreach (Fish fish in allFish)
        {
            if (currentActiveQuest.IsFishCompatible(fish))
            {
                CreateSlot(fish);
            }
        }
    }

    private void CreateSlot(Fish fish)
    {
        GameObject newSlot = Instantiate(fishSlotPrefab, slotsParent);
        
       
        CardInventoryUI ui = newSlot.GetComponent<CardInventoryUI>();
        if(ui != null) ui.UpdateCardUI(fish);

        Button btn = newSlot.GetComponent<Button>(); 
        Image bg = newSlot.GetComponent<Image>(); 
        
        btn.onClick.AddListener(() => 
        {
            ToggleSelectFish(fish, bg);
        });
        
        currentSlotObjs.Add(newSlot);
    }

    private void ToggleSelectFish(Fish fish, Image bgImage)
    {
        if (selectedFish.Contains(fish))
        {
            selectedFish.Remove(fish);
            bgImage.color = Color.white;
        }
        else
        {
            selectedFish.Add(fish);
            bgImage.color = Color.green;
        }
        
        UpdateRequirementText();
        CheckSubmitButton();
    }

    private void CheckSubmitButton()
    {
     
        submitButton.interactable = ValidateSelection();
    }

    private bool ValidateSelection()
    {
        List<QuestRequirement> tempReqs = new List<QuestRequirement>();
        foreach(var r in currentActiveQuest.requirements)
        {
            QuestRequirement temp = new QuestRequirement();
            temp.type = r.type;
            temp.specificFish = r.specificFish;
            temp.requiredRarity = r.requiredRarity;
            temp.minWeight = r.minWeight;
            temp.amountRequired = r.amountRequired;
            tempReqs.Add(temp);
        }

        foreach (Fish fish in selectedFish)
        {
            bool matched = false;
            foreach (var req in tempReqs)
            {
                if (req.amountRequired > 0 && req.IsMatch(fish))
                {
                    req.amountRequired--;
                    matched = true;
                    break;
                }
            }
        }

        foreach (var req in tempReqs)
        {
            if (req.amountRequired > 0) return false;
        }

        return true;
    }

    private void UpdateRequirementText()
    {
        string status = "Requirements:\n";
        foreach(var req in currentActiveQuest.requirements)
        {
            string w = req.minWeight > 0 ? $" (> {req.minWeight:F1}kg)" : "";
            string name = req.type == QuestRequirement.RequirementType.SpecificFish ? req.specificFish.Name : req.requiredRarity.ToString();
            status += $"- {name}{w} : {req.amountRequired} Amounts\n";
        }
        requirementStatusText.text = status;
    }

    private void SubmitQuest()
    {
        if (!ValidateSelection()) return;

        foreach (Fish f in selectedFish)
        {
            Inventory.Instance.RemoveFish(f); 
        }

        Debug.Log("Quest Completed! Reward: " + currentActiveQuest.moneyReward);
        currentActiveQuest.QuestDone();
        // PlayerStats.Instance.AddMoney(currentActiveQuest.moneyReward);

        questPanel.SetActive(false);
        UIManager.Instance.ChangeState(currentState.None);
    }
    
    public void CloseQuestWindow()
    {
         questPanel.SetActive(false);
         if(!QuestCardManager.Instance.isOpen)
             UIManager.Instance.ChangeState(currentState.None);
    }
}