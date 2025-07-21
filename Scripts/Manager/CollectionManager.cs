using BackEnd;
using LitJson;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CollectionManager : Singleton<CollectionManager>
{
    public ItemBase[] Items;                       // 모든 아이템 정보 배열 (외부에서 세팅)
    public GameObject IconPrefab;                  // 아이템 아이콘 UI 프리팹
    public Transform ContentParent;                // 아이콘들을 넣을 스크롤뷰 컨텐츠 부모 객체
    public Button CloseButton;                     // 아이템 정보창 닫기 버튼

    [Header("Info Panel")]
    public GameObject ItemInfoPanel;                // 아이템 상세 정보창 패널
    public TMP_Text NameText;                       // 아이템 이름 텍스트
    public TMP_Text GradeText;                      // 아이템 등급 텍스트
    public TMP_Text ExplanationText;                // 아이템 설명 텍스트

    [Header("Reward")]
    public Button[] RewardButtons;                  // 보상 수령 버튼들 (인스펙터에 4개 설정)
    public TMP_Text[] RewardButtonTexts;            // 보상 버튼 내 텍스트들
    private bool[] _rewardClaimed;                   // 각 보상 단계 수령 여부 체크 배열

    // 뒤에 배열 선언하면됨
    //[Header("Reward Data")]
    private int[] _rewardGoldAmounts = {1000, 1500, 2000, 2500, 3000};                 // 각 보상 단계별 지급 골드량

    private int _unlockedCount = 0;                  // 해금된(획득한) 아이템 개수 카운트
    private int[] _rewardMilestones = { 5, 10, 15, 20, 25 };  // 보상 받을 아이템 개수 기준
    private List<GameObject> _iconObjects = new List<GameObject>(); // 생성된 아이콘 오브젝트 리스트

    // 뒤끝
    private const string COLLECTION_TABLE = "USER_COLLECTION";
    private string collectionRowInDate = string.Empty;

    // 등급별 정렬 우선순위 (숫자가 낮을수록 우선)
    private Dictionary<Grade, int> _gradeOrder = new Dictionary<Grade, int>()
    {
        { Grade.Legendary, 0 },
        { Grade.Rare, 1 },
        { Grade.Uncommon, 2 },
        { Grade.Common, 3 }
    };

    // 등급별 배경색 지정
    private Dictionary<Grade, Color> _gradeColors = new Dictionary<Grade, Color>()
    {
        { Grade.Legendary, new Color32(237, 171, 95, 255) },
        { Grade.Rare, new Color32(190, 119, 231, 255) },
        { Grade.Uncommon, new Color32(87, 152, 241, 255) },
        { Grade.Common, new Color32(114, 143, 115, 255) }
    };

    void Start()
    {
        LoadCollectionDataFromBackend(); // 내부에서 UI 초기화까지 하도록 처리됨
    }

    private void Update()
    {
        //Debug.Log(_unlockedCount);
    }

    // 아이템 상세 정보 UI에 내용 채우고 패널 활성화
    private void ShowItemInfo(ItemBase item)
    {
        SoundManager.Instance.PlaySFX("Button");

        NameText.text = item.Name;
        GradeText.text = item.grade.ToString();
        ExplanationText.text = item.Explanation;
        ItemInfoPanel.SetActive(true);
    }

    private void InitializeUI()
    {
        // 아이콘 정렬 및 생성
        var sortedItems = Items
            .OrderBy(item => _gradeOrder.ContainsKey(item.grade) ? _gradeOrder[item.grade] : int.MaxValue)
            .ToList();

        foreach (var item in sortedItems)
        {
            GameObject iconObj = Instantiate(IconPrefab, ContentParent);
            Image iconImage = iconObj.transform.Find("Icon").GetComponent<Image>();
            iconImage.sprite = item.ItemImage;

            Image background = iconObj.transform.Find("Background").GetComponent<Image>();
            Color bgColor = _gradeColors.ContainsKey(item.grade) ? _gradeColors[item.grade] : Color.white;

            UpdateIconLockState(iconObj, item.IsUnlocked, bgColor);

            Button btn = iconObj.GetComponent<Button>();
            btn.onClick.AddListener(() => ShowItemInfo(item));

            _iconObjects.Add(iconObj);
        }

        // 보상 버튼 설정
        for (int i = 0; i < RewardButtons.Length; i++)
        {
            int index = i;
            RewardButtons[i].onClick.AddListener(() => OnRewardButtonClicked(index));
            RewardButtons[i].gameObject.SetActive(false);
        }

        CloseButton.onClick.AddListener(OnCloseButtonClick);
        ItemInfoPanel.SetActive(false);

        CheckRewardAvailability();
    }


    // 외부에서 아이템 해금 시 호출하는 함수
    public void UnlockItem(ItemBase item)
    {
        if (!item.IsUnlocked)
        {
            item.IsUnlocked = true;
            _unlockedCount++;

            // 기존 아이콘 전부 삭제
            foreach (var icon in _iconObjects)
            {
                Destroy(icon);
            }
            _iconObjects.Clear();

            // UI 다시 초기화 (아이콘 새로 생성 및 정렬)
            InitializeUI();

            CheckRewardAvailability();
        }

        if (string.IsNullOrEmpty(collectionRowInDate))
            SaveCollectionDataToBackend();
        else
            UpdateCollectionDataToBackend();
    }

    // 아이콘의 잠금 상태에 따라 이미지 색상 변경
    private void UpdateIconLockState(GameObject iconObj, bool unlocked, Color bgColor)
    {
        var iconImage = iconObj.transform.Find("Icon").GetComponent<Image>();
        var background = iconObj.transform.Find("Background").GetComponent<Image>();

        if (unlocked)
        {
            iconImage.color = Color.white;           // 선명한 아이콘
            background.color = bgColor;               // 등급에 맞는 배경색
        }
        else
        {
            iconImage.color = new Color(1, 1, 1, 0.3f);   // 반투명 아이콘 (잠금 느낌)
            background.color = new Color(bgColor.r, bgColor.g, bgColor.b, 0.3f);  // 반투명 배경
        }
    }

    // 보상 받을 수 있는지 체크하고 보상 버튼 활성화/비활성화 처리
    private void CheckRewardAvailability()
    {
        for (int i = 0; i < _rewardMilestones.Length; i++)
        {
            // 아직 보상 미수령 && 해금 아이템 수가 기준 이상이면 버튼 활성화
            if (!_rewardClaimed[i] && _unlockedCount >= _rewardMilestones[i])
            {
                RewardButtons[i].gameObject.SetActive(true);
                RewardButtons[i].interactable = true;
                RewardButtonTexts[i].text = $"보상 받기! ({_rewardMilestones[i]}개)";
            }
            // 이미 보상 수령한 경우 버튼 활성화 + 비활성화 처리
            else if (_rewardClaimed[i])
            {
                RewardButtons[i].gameObject.SetActive(true);
                RewardButtons[i].interactable = false;
                RewardButtonTexts[i].text = "수령 완료";
            }
            // 아직 조건 안 됐으면 버튼 숨기기
            else
            {
                RewardButtons[i].gameObject.SetActive(false);
            }
        }
    }

    // 보상 버튼 클릭 시 호출, 보상 지급 처리
    private void OnRewardButtonClicked(int index)
    {
        if (_rewardClaimed[index]) return;  // 이미 받은 보상은 무시

        SoundManager.Instance.PlaySFX("Button");

        Debug.Log($"{_rewardMilestones[index]}개 보상 지급!");

        // 골드 보상 지급 처리 (예시)
        if (_rewardGoldAmounts != null && index < _rewardGoldAmounts.Length)
        {
            int gold = _rewardGoldAmounts[index];
            Debug.Log($"골드 +{gold}");

            GameManager.Instance.GetGp(gold); // 실제 골드 지급 함수 호출
        }

        // 보상 수령 완료 처리
        _rewardClaimed[index] = true;
        RewardButtons[index].interactable = false;
        RewardButtonTexts[index].text = "수령 완료";

        // 변경된 보상 상태 서버에 업데이트
        if (!string.IsNullOrEmpty(collectionRowInDate))
            UpdateCollectionDataToBackend();
    }

    private void OnCloseButtonClick()
    {
        SoundManager.Instance.PlaySFX("Button");

        ItemInfoPanel.SetActive(false);
    }

    // 뒤끝 서버에 저장할 데이터
    [System.Serializable]
    public class CollectionSaveData
    {
        public List<string> unlockedItemIDs = new List<string>();  // 해금된 아이템 ID 목록
        public List<bool> rewardClaimed = new List<bool>();         // 보상 수령 여부
    }

    #region 저장

    public void SaveCollectionDataToBackend()
    {
        CollectionSaveData data = new CollectionSaveData();

        foreach (var item in Items)
        {
            if (item.IsUnlocked)
                data.unlockedItemIDs.Add(item.ID);
        }

        foreach (var claimed in _rewardClaimed)
        {
            data.rewardClaimed.Add(claimed);
        }

        string json = JsonUtility.ToJson(data);
        Param param = new Param();
        param.Add("StatData", json);

        var bro = Backend.GameData.Insert(COLLECTION_TABLE, param);

        if (bro.IsSuccess())
        {
            Debug.Log("도감 데이터 저장 성공");
            collectionRowInDate = bro.GetInDate();
        }
        else
        {
            Debug.LogError("도감 데이터 저장 실패: " + bro);
        }
    }


    #endregion

    #region 수정하기
    public void UpdateCollectionDataToBackend()
    {
        if (string.IsNullOrEmpty(collectionRowInDate))
        {
            SaveCollectionDataToBackend();
            Debug.LogError("도감 데이터가 먼저 삽입되어야 수정이 가능합니다. SaveCollectionDataToBackend()를 먼저 호출하세요.");
            return;
        }

        CollectionSaveData data = new CollectionSaveData();

        foreach (var item in Items)
        {
            if (item.IsUnlocked)
                data.unlockedItemIDs.Add(item.ID);
        }

        foreach (var claimed in _rewardClaimed)
        {
            data.rewardClaimed.Add(claimed);
        }

        string json = JsonUtility.ToJson(data);
        Param param = new Param();
        param.Add("StatData", json);

        var bro = Backend.GameData.UpdateV2(COLLECTION_TABLE, collectionRowInDate, Backend.UserInDate, param);

        if (bro.IsSuccess())
        {
            Debug.Log("도감 데이터 수정 성공");
        }
        else
        {
            Debug.LogError("도감 데이터 수정 실패: " + bro);
        }
    }

    #endregion

    #region 불러오기

    public void LoadCollectionDataFromBackend()
    {
        foreach (var item in Items)
            item.IsUnlocked = false;

        var bro = Backend.GameData.GetMyData(COLLECTION_TABLE, new Where());

        if (!bro.IsSuccess())
        {
            Debug.LogError("도감 데이터 조회 실패: " + bro);
            InitializeUI();
            return;
        }

        JsonData rows = bro.FlattenRows();
        if (rows.Count <= 0)
        {
            Debug.Log("도감 데이터 없음 (최초 접속)");
            _rewardClaimed = new bool[_rewardMilestones.Length];
            InitializeUI();
            return;
        }

        collectionRowInDate = rows[0]["inDate"].ToString();
        string json = rows[0]["StatData"].ToString();

        CollectionSaveData data = JsonUtility.FromJson<CollectionSaveData>(json);

        foreach (var item in Items)
            item.IsUnlocked = data.unlockedItemIDs.Contains(item.ID);

        _unlockedCount = data.unlockedItemIDs.Count;

        _rewardClaimed = new bool[_rewardMilestones.Length];
        for (int i = 0; i < _rewardClaimed.Length && i < data.rewardClaimed.Count; i++)
            _rewardClaimed[i] = data.rewardClaimed[i];

        InitializeUI(); // ← 이 타이밍이 중요!
    }

    #endregion
}
