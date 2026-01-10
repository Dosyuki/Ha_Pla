using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Quest/Dialogue Data")]
public class DialogueData : ScriptableObject
{
    [Header("Speaker")]
    public string speakerName;
    public Sprite portrait; // รูปคนพูด (ถ้ามี)

    [Header("Lines")]
    [TextArea] public List<string> sentences;

    [Header("Action After Dialogue")]
    public QuestData questToOpen; // คุยจบแล้วให้เปิดเควสไหน (ถ้าไม่มี = ปิดเฉยๆ)
}