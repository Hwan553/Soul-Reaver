using UnityEngine;

[CreateAssetMenu(fileName = "QuestData", menuName = "Quests/Quest")]
public class QuestDataSO : ScriptableObject
{
    public string questID;                      // 고유 ID
    public string questTitle;                   // 퀘스트 이름
    public string questDescription;             // 퀘스트 내용
    public Sprite rewardIcon;                   // 보상 아이콘
    public int rewardAmount;                    // 보상 수치
    public QuestType questType;                 // 퀘스트 타입
}

public enum QuestType
{
    KillMonster,
    DoGacha,
    ClearStage,
}
