using BackEnd;
using LitJson;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuestManager : Singleton<QuestManager>
{
    [Header("퀘스트 템플릿 리스트")]
    public List<QuestDataSO> questTemplates;

    // 실시간 퀘스트 리스트
    public List<Quest> activeQuests = new List<Quest>();
    private Dictionary<string, int> questLevels = new Dictionary<string, int>();
    private string _questInDate = string.Empty;

    private const string QuestTableName = "USER_QUEST"; // 뒤끝 테이블명

    [System.Serializable]
    public class QuestSaveData
    {
        public string questID;
        public int currentProgress;
        public int targetAmount;
        public bool isCompleted;
        public bool isRewardClaimed;
    }

    // 퀘스트 생성
    public void CreateRepeatableQuest(string questID)
    {
        var data = questTemplates.Find(q => q.questID == questID);
        if (data == null)
        {
            Debug.LogWarning($"[{questID}] 퀘스트 템플릿을 찾을 수 없습니다.");
            return;
        }

        if (!questLevels.ContainsKey(questID))
            questLevels[questID] = 1;
        else
            questLevels[questID] += 1;

        Quest newQuest = new Quest { data = data };
        newQuest.Initialize(questLevels[questID]);
        activeQuests.Add(newQuest);

        FindAnyObjectByType<QuestUIManager>()?.RefreshUI(activeQuests);
    }

    #region 뒤끝

    // 전체 저장 (Insert or Update)
    public void SaveAllQuests()
    {
        List<QuestSaveData> saveList = activeQuests.Select(q => new QuestSaveData
        {
            questID = q.data.questID,
            currentProgress = q.currentProgress,
            targetAmount = q.targetAmount,
            isCompleted = q.isCompleted,
            isRewardClaimed = q.isRewardClaimed
        }).ToList();

        string json = JsonMapper.ToJson(saveList);
        Param param = new Param { { "Quests", json } };

        if (string.IsNullOrEmpty(_questInDate))
        {
            var bro = Backend.GameData.Insert(QuestTableName, param);
            if (bro.IsSuccess())
            {
                _questInDate = bro.GetInDate();
                Debug.Log("전체 퀘스트 첫 저장 완료");
            }
            else
            {
                Debug.LogError("전체 퀘스트 Insert 실패: " + bro);
            }
        }
        else
        {
            var bro = Backend.GameData.Update(QuestTableName, new Where(), param);
            if (bro.IsSuccess())
                Debug.Log("전체 퀘스트 업데이트 완료");
            else
                Debug.LogError("전체 퀘스트 Update 실패: " + bro);
        }
    }

    // 특정 퀘스트만 수정해서 저장
    public void ModifyQuestProgress(Quest modifiedQuest)
    {
        Backend.GameData.GetMyData(QuestTableName, new Where(), bro =>
        {
            if (!bro.IsSuccess())
            {
                Debug.LogError("퀘스트 수정 저장 실패 - 데이터 조회 실패: " + bro);
                return;
            }

            var rows = bro.FlattenRows();
            if (rows.Count == 0)
            {
                Debug.LogWarning("저장된 퀘스트 데이터가 없음. 수정 불가.");
                return;
            }

            var row = rows[0];
            _questInDate = row["inDate"].ToString();

            string json = row["Quests"].ToString();
            List<QuestSaveData> saveList = JsonMapper.ToObject<List<QuestSaveData>>(json);

            // 해당 퀘스트 찾기
            var existing = saveList.Find(q => q.questID == modifiedQuest.data.questID);
            if (existing != null)
            {
                existing.currentProgress = modifiedQuest.currentProgress;
                existing.targetAmount = modifiedQuest.targetAmount;
                existing.isCompleted = modifiedQuest.isCompleted;
                existing.isRewardClaimed = modifiedQuest.isRewardClaimed;
            }
            else
            {
                saveList.Add(new QuestSaveData
                {
                    questID = modifiedQuest.data.questID,
                    currentProgress = modifiedQuest.currentProgress,
                    targetAmount = modifiedQuest.targetAmount,
                    isCompleted = modifiedQuest.isCompleted,
                    isRewardClaimed = modifiedQuest.isRewardClaimed
                });
            }

            // 수정된 전체 리스트 다시 저장
            string updatedJson = JsonMapper.ToJson(saveList);
            Param param = new Param { { "Quests", updatedJson } };

            var updateBro = Backend.GameData.Update(QuestTableName, new Where(), param);
            if (updateBro.IsSuccess())
                Debug.Log($"퀘스트 [{modifiedQuest.data.questID}] 수정 저장 완료");
            else
                Debug.LogError("퀘스트 수정 저장 실패: " + updateBro);
        });
    }

    // 전체 불러오기
    public void LoadAllQuests()
    {
        Backend.GameData.GetMyData(QuestTableName, new Where(), bro =>
        {
            if (bro.IsSuccess())
            {
                var rows = bro.FlattenRows();
                if (rows.Count == 0)
                {
                    Debug.Log("처음 접속한 유저, 퀘스트 데이터 없음 → 기본 퀘스트 저장");

                    // [1] 기본 퀘스트 추가
                    CreateRepeatableQuest("Monster_001"); // 템플릿에 있는 questID로

                    // [2] 바로 저장
                    SaveAllQuests();
                    return;
                }

                var row = rows[0];
                _questInDate = row["inDate"].ToString();

                string json = row["Quests"].ToString();
                List<QuestSaveData> loaded = JsonMapper.ToObject<List<QuestSaveData>>(json);

                activeQuests.Clear();

                foreach (var saveData in loaded)
                {
                    QuestDataSO dataSO = questTemplates.Find(q => q.questID == saveData.questID);
                    if (dataSO == null) continue;

                    Quest q = new Quest
                    {
                        data = dataSO,
                        currentProgress = saveData.currentProgress,
                        targetAmount = saveData.targetAmount,
                        isCompleted = saveData.isCompleted,
                        isRewardClaimed = saveData.isRewardClaimed
                    };

                    activeQuests.Add(q);

                    if (!questLevels.ContainsKey(saveData.questID))
                        questLevels[saveData.questID] = 1;

                    if (dataSO.questType == QuestType.KillMonster)
                        questLevels[saveData.questID] = Mathf.Max(1, saveData.targetAmount / 5);
                    else if (dataSO.questType == QuestType.ClearStage)
                        questLevels[saveData.questID] = Mathf.Max(1, saveData.targetAmount / 3);
                }

                FindAnyObjectByType<QuestUIManager>()?.RefreshUI(activeQuests);
            }
            else
            {
                Debug.LogWarning("퀘스트 불러오기 실패: " + bro);
            }
        });
    }

    #endregion

    // 퀘스트 제거
    public void RemoveClaimedQuests()
    {
        activeQuests.RemoveAll(q => q.isCompleted && q.isRewardClaimed);
        SaveAllQuests(); // 저장 갱신
        FindAnyObjectByType<QuestUIManager>()?.RefreshUI(activeQuests);
    }

    // 몬스터 킬 
    public void OnMonsterKilled()
    {
        bool questUpdated = false;

        foreach (var quest in activeQuests)
        {
            if (quest.data.questType == QuestType.KillMonster &&
                !quest.isCompleted)
            {
                quest.currentProgress++;

                // 완료 체크
                if (quest.currentProgress >= quest.targetAmount)
                {
                    quest.currentProgress = quest.targetAmount;
                    quest.isCompleted = true;
                    Debug.Log($"[퀘스트 완료] {quest.data.questTitle}");
                }

                // 퀘스트 수정 저장
                ModifyQuestProgress(quest);
                questUpdated = true;
            }
        }

        if (questUpdated)
        {
            FindAnyObjectByType<QuestUIManager>()?.RefreshUI(activeQuests);
        }
    }

}
