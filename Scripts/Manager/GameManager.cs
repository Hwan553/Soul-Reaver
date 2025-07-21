using BackEnd;
using LitJson;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    private Vector3 _moveDirection;

    private event Action<Vector3> OnMoveDirectionChanged;

    private string gameRowInDate = string.Empty; // 수정용으로 inDate 저장

    [SerializeField] private int _goldNum;
    [SerializeField] private int _gpNum;

    public GameObject Target { get; set; }

    public Vector3 MoveDirection
    {
        get { return _moveDirection; }
        set
        {
            _moveDirection = value;
            OnMoveDirectionChanged?.Invoke(value);
        }
    }

    public int GoldNum
    {
        get { return _goldNum; }
        set { _goldNum = value; }
    }

    public int GpNum
    {
        get { return _gpNum; }
        set { _gpNum = value; }
    }

    protected override void Initialized()
    {
        base.Initialized();

        Setting();
    }

    private void Awake()
    {
        // 강제로 초기화 (ItemDatabase가 씬에 존재하는지 반드시 확인!)
        var db = FindAnyObjectByType<ItemDatabase>();

        if (db == null)
        {
            Debug.LogError("씬에 ItemDatabase가 존재하지 않습니다!");
        }
    }

    private IEnumerator Start()
    {
        yield return null; // 한 프레임 쉬고, 다른 싱글톤 초기화 기다림

        SoundManager.Instance.PlayBGM("Night Market");

        Backend.GameData.GetMyData("USER_DATA", new Where(), callback => {
            if (callback.IsSuccess() && callback.FlattenRows().Count == 0)
            {
                StageManager.Instance.SaveStageDataToBackend();
                StatUpgradeManager.Instance.SaveStatsToBackend();
                InventoryManager.Instance.SaveInventoryToBackend();
                DrawingPanelManager.Instance.SaveGachaDataToBackend();
                QuestManager.Instance.LoadAllQuests();
                SaveGameDataToBackend();
                Setting();

            }
            else
            {
                StageManager.Instance.LoadStageDataFromBackend();
                StatUpgradeManager.Instance.LoadStatsFromBackend();
                InventoryManager.Instance.LoadInventoryFromBackend(); // 이 시점에 Instance는 null이면 안됨
                DrawingPanelManager.Instance.LoadGachaDataFromBackend();
                QuestManager.Instance.LoadAllQuests();

                SkillManager.Instance.LoadSkillDataFromBackend();
                LoadGameDataFromBackend();
            }
        });
    }

    private void Setting()
    {
        _goldNum = 0;
        _gpNum = 0;
    }

    public void LoseMoney(int money)
    {
        _goldNum -= money;
        UpdateGameDataToBackend();
    }

    public void LoseGp(int gp)
    {
        _gpNum -= gp;
        UpdateGameDataToBackend();
    }

    public void GetMoney(int money)
    {
        _goldNum += money;
        UpdateGameDataToBackend();

    }

    public void GetGp(int gp)
    {
        _gpNum += gp;
        UpdateGameDataToBackend();
    }

    // 저장
    public void SaveGameDataToBackend()
    {
        GameSaveData data = new GameSaveData
        {
            gold = _goldNum,
            gp = _gpNum
        };

        string json = JsonUtility.ToJson(data);
        Param param = new Param();
        param.Add("StatData", json);

        BackendReturnObject bro = Backend.GameData.Insert("USER_MONEY", param); // 테이블명은 USER_GAME

        if (bro.IsSuccess())
        {
            Debug.Log("게임 데이터 저장 성공");
            gameRowInDate = bro.GetInDate(); // 수정용 inDate 저장
        }
        else
        {
            Debug.LogError("게임 데이터 저장 실패: " + bro);
        }
    }

    // 수정
    public void UpdateGameDataToBackend()
    {
        if (string.IsNullOrEmpty(gameRowInDate))
        {
            Debug.LogError("먼저 SaveGameDataToBackend()를 실행해 초기 데이터를 저장해야 합니다.");
            return;
        }

        GameSaveData data = new GameSaveData
        {
            gold = _goldNum,
            gp = _gpNum
        };

        string json = JsonUtility.ToJson(data);
        Param param = new Param();
        param.Add("StatData", json);

        BackendReturnObject bro = Backend.GameData.UpdateV2("USER_MONEY", gameRowInDate, Backend.UserInDate, param);

        if (bro.IsSuccess())
            Debug.Log("게임 데이터 수정 성공");
        else
            Debug.LogError("게임 데이터 수정 실패: " + bro);
    }

    // 불러오기
    public void LoadGameDataFromBackend()
    {
        BackendReturnObject bro = Backend.GameData.GetMyData("USER_MONEY", new Where());

        if (!bro.IsSuccess())
        {
            Debug.LogError("게임 데이터 로드 실패: " + bro);
            return;
        }

        JsonData rows = bro.FlattenRows();
        if (rows.Count <= 0)
        {
            Debug.Log("게임 데이터 없음");
            SaveGameDataToBackend();
            return;
        }

        gameRowInDate = rows[0]["inDate"].ToString(); // 수정용 inDate 저장
        string json = rows[0]["StatData"].ToString();

        GameSaveData data = JsonUtility.FromJson<GameSaveData>(json);

        _goldNum = data.gold;
        _gpNum = data.gp;
    }
}

// 뒤끝 저장 및 불러오기

[Serializable]
public class GameSaveData
{
    public int gold;
    public int gp;
}