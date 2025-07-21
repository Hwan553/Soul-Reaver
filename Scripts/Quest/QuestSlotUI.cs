using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestSlotUI : MonoBehaviour
{
    public TMP_Text titleText;
    public TMP_Text descriptionText;
    public Image rewardIcon;
    public TMP_Text rewardAmountText;
    public Button rewardButton;

    private Quest currentQuest;

    public void Setup(Quest quest)
    {
        currentQuest = quest;

        titleText.text = quest.data.questTitle;
        descriptionText.text = $"진행도: {quest.currentProgress} / {quest.targetAmount}";
        rewardIcon.sprite = quest.data.rewardIcon;
        rewardAmountText.text = $"보상: {quest.data.rewardAmount}";

        // 보상 버튼 상태 설정
        rewardButton.interactable = quest.isCompleted && !quest.isRewardClaimed;

        // 버튼 이벤트 연결
        rewardButton.onClick.RemoveAllListeners();
        rewardButton.onClick.AddListener(ClaimReward);
    }

    private void ClaimReward()
    {
        if (currentQuest.isCompleted && !currentQuest.isRewardClaimed)
        {
            currentQuest.isRewardClaimed = true;

            // 예: 보상 지급 처리 (골드 증가 등)
            Debug.Log($"보상 지급: +{currentQuest.data.rewardAmount}");
            GameManager.Instance.GetGp(500);

            // 퀘스트 반복 생성
            QuestManager.Instance.CreateRepeatableQuest(currentQuest.data.questID);

            // 수정 저장
            QuestManager.Instance.ModifyQuestProgress(currentQuest);

            // 퀘스트 갱신
            QuestManager.Instance.RemoveClaimedQuests();
        }
    }

}
