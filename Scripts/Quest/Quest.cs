[System.Serializable]
public class Quest
{
    public QuestDataSO data;             // 퀘스트의 템플릿 데이터 (이름, 타입, 보상 등)
    public int targetAmount;             // 이번 퀘스트에서 목표로 해야 할 수치 (예: 몬스터 10마리)
    public int currentProgress;          // 현재 달성한 수치 (예: 현재 6마리 잡음)
    public bool isCompleted;             // 목표 달성 여부
    public bool isRewardClaimed;         // 보상 수령 여부

    // 퀘스트 생성 시 호출되는 초기화 함수
    public void Initialize(int level = 1)
    {
        // 퀘스트 타입에 따라 목표 수치를 다르게 설정
        switch (data.questType)
        {
            case QuestType.KillMonster:
                targetAmount = 5 * level;       // 레벨 1일 땐 5마리, 2일 땐 10마리...
                break;

            case QuestType.ClearStage:
                targetAmount = 3 * level;       // 예: 3웨이브, 6웨이브, 9웨이브...
                break;

            default:
                targetAmount = 1;               // 그 외 퀘스트는 기본 1로 설정
                break;
        }

        // 초기 진행도와 완료 상태 설정
        currentProgress = 0;
        isCompleted = false;
        isRewardClaimed = false;
    }

    // 퀘스트 진행도를 올리는 함수
    public void AddProgress(int amount)
    {
        if (isCompleted) return; // 이미 완료된 퀘스트는 더 이상 진행 불가

        currentProgress += amount; // 수치를 더함

        // 목표를 넘기면 완료 처리
        if (currentProgress >= targetAmount)
        {
            currentProgress = targetAmount;
            isCompleted = true;
        }
    }
}
