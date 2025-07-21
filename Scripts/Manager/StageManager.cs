using BackEnd;
using LitJson;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StageManager : Singleton<StageManager>
{
    [Header("Player")]
    [SerializeField] private PlayerController _player;

    [Header("스테이지 데이터 리스트")]
    [SerializeField] private List<MapData> _maps;
    [SerializeField] private int _currentMapIndex;

    [Header("머티리얼을 적용할 오브젝트")]
    [SerializeField] private List<MeshRenderer> _mapRenderersToApply;

    [Header("웨이브")]
    [SerializeField] private int _stageWave;

    [Header("타이머")]
    [SerializeField] private int _startMinutes;
    [SerializeField] private float _totalTime;
    [SerializeField] private float _remainingTime;
    [SerializeField] private int _minutes;
    [SerializeField] private int _seconds;

    [Header("failure")]
    private bool _isStageFailed;
    private float _retryTimer = 0f;
    [SerializeField] private float _retryDelay = 2f;

    //플레이어 프로퍼티
    public PlayerController Player => _player;

    // 맵 프로퍼티
    public List<MapData> Maps => _maps;
    public List<MeshRenderer> MapRenderersToApply => _mapRenderersToApply;

    // 스테이지 프로퍼티
    public int StageWave => _stageWave;

    // 타이머 프로퍼티
    public float TotalTime => _totalTime;
    public float RemainingTime => _remainingTime;
    public string StageTimer => $"{_minutes:00}:{_seconds:00}";

    public int CurrentMapIndex
    {
        get { return _currentMapIndex; }
        set { _currentMapIndex = value; }
    }

    protected override void Initialized()
    {
        base.Initialized();

        Setting();
        ResourceAllLoad();
    }

    private void Start()
    {
        if (_maps == null || _maps.Count == 0)
        {
            Debug.LogError("[StageManager] 맵 데이터가 설정되지 않았습니다.");
        }

        _mapRenderersToApply.Add(GameObject.FindGameObjectWithTag(Define.Ground_Tag).gameObject.GetComponent<MeshRenderer>());
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        ResetTimer();

        ApplyCurrentMap(); // 게임 시작 시 첫 맵 적용
    }

    private void Update()
    {
        UpdateTimer();

        if (!_isStageFailed)
        {
            // 1. 시간 초과
            if (_remainingTime <= 0f)
            {
                OnStageFailed("시간 초과");
            }
            // 2. 플레이어 사망 체크 (PlayerController에서 상태 전달)
            else if (_player != null && _player.IsDeath)
            {
                OnStageFailed("플레이어 사망");
            }
        }

        // 실패 후 자동 재도전
        if (_isStageFailed)
        {
            _retryTimer += Time.deltaTime;
            if (_retryTimer >= _retryDelay)
            {
                RetryCurrentStage();
            }
        }
    }

    private void Setting()
    {
        _maps = new List<MapData>();
        _currentMapIndex = 0;

        _mapRenderersToApply = new List<MeshRenderer>();

        _stageWave = 1;
        _startMinutes = 1;
    }

    // 모든 데이터 리소스를 로드하는 함수
    private void ResourceAllLoad()
    {
        _maps = Resources.LoadAll<MapData>(Define.Map_Scriptable_Path).ToList();
    }

    // 타이머 초기화
    private void ResetTimer()
    {
        _totalTime = _startMinutes * 60.0f;
        _remainingTime = _totalTime;
    }

    // 실패 함수
    private void OnStageFailed(string reason)
    {
        _isStageFailed = true;
        _retryTimer = 0.0f;

        RemoveAllEnemy();

        // 최소 스테이지 1 보장
        _stageWave = Mathf.Max(1, _stageWave - 1);

        // 웨이브 기반으로 맵 다시 계산
        _currentMapIndex = Mathf.Clamp((_stageWave - 1) / 10, 0, Maps.Count - 1);
        ApplyCurrentMap(); // 머티리얼 재적용

        SpawnManager.Instance.StopWave();
        StageManager.Instance.SaveStageDataToBackend(); // 서버에 현재 스테이지 반영
    }

    // Retry 코루틴 함수
    private void RetryCurrentStage()
    {
        StartCoroutine(DoRetry());
    }

    // Retry 함수
    private IEnumerator DoRetry()
    {
        _isStageFailed = false;
        _retryTimer = 0.0f;

        ResetTimer();
        ApplyCurrentMap();
        _player?.ReSpawnPlayer();

        yield return new WaitForSeconds(1.5f);           // 한 프레임 대기

        SpawnManager.Instance.StartWave(_stageWave);
    }

    // 타이머 업데이트
    private void UpdateTimer()
    {
        if (_remainingTime > 0.0f)
        {
            _remainingTime -= Time.deltaTime;
            _remainingTime = Mathf.Max(_remainingTime, 0.0f); // 음수 방지

            // 시간 형식으로 변환
            _minutes = Mathf.FloorToInt(_remainingTime / 60.0f);
            _seconds = Mathf.FloorToInt(_remainingTime % 60.0f);
        }
        else
        {
            _remainingTime = 0.0f;
            _minutes = 0;
            _seconds = 0;
        }
    }

    public void NextStage()
    {
        _stageWave++; // 현재 웨이브 번호 1 증가

        SkillManager.Instance.SkillSlotUnlocked();
        // 스킬 슬롯 해제 로직 호출 (예: 특정 웨이브마다 스킬 슬롯이 열림)

        if (_stageWave % 10 == 1 && _currentMapIndex < Maps.Count - 1)
        {
            _currentMapIndex++;
            // 새로운 챕터 진입 시 맵 전환 (ex. 11, 21, 31 웨이브에서만 맵 인덱스 증가)
            // 최대 맵 개수를 초과하지 않도록 제한
        }

        ApplyCurrentMap(); // 현재 인덱스에 해당하는 맵 머티리얼 적용
        ResetTimer();      // 시간 초기화 (예: 웨이브 시작 타이머 등)

        SpawnManager.Instance.StartWave(_stageWave);
        // 적 스폰 매니저에 현재 웨이브 시작을 알림

        UpdateStageDataToBackend();
        // 백엔드 저장 처리 (현재 웨이브, 맵 등 게임 진행도 저장)
    }

    public void ApplyCurrentMap()
    {
        MapData currentMap = _maps[_currentMapIndex];
        // 현재 맵 인덱스에 해당하는 맵 데이터를 가져옴

        for (int i = 0; i < _mapRenderersToApply.Count; i++)
        {
            if (i < currentMap.materials.Count && _mapRenderersToApply[i] != null)
            {
                _mapRenderersToApply[i].material = currentMap.materials[i];
                // 맵 배경, 타일 등 각 렌더러에 머티리얼을 적용
            }
            else
            {
                Debug.LogWarning($"[StageManager] 머티리얼 적용 누락: Index {i}");
                // 예외 상황 경고 로그 (예: 렌더러나 머티리얼이 비어있을 때)
            }
        }
    }

    
    public EnemyMapSet GetCurrentEnemySet()
    {
        return _maps[_currentMapIndex].enemySetData;
        // 현재 맵 인덱스에 등록된 EnemyMapSet을 반환
        // EnemyMapSet은 일반 적과 보스 종류 등을 포함한 ScriptableObject로 반환
    }

    // Retry시 스테이지 기존 적 제거
    public void RemoveAllEnemy()
    {
        var _enemy = GameObject.FindGameObjectsWithTag(Define.Enemy_Tag);
        var _boss = GameObject.FindGameObjectsWithTag(Define.Boss_Tag);

        foreach (var enemy in _enemy)
        {
            PoolManager.Instance.DeactivateObj(enemy);
        }

        foreach (var boss in _boss)
        {
            PoolManager.Instance.DeactivateObj(boss);
        }
    }

    protected override void Clear()
    {
        base.Clear();

        _maps.Clear();
        _mapRenderersToApply.Clear();
    }

    // 뒤끝 웨이브 저장 코드

    private string stageRowInDate = string.Empty;

    [System.Serializable]
    public class StageSaveData
    {
        public int stageWave;
        public int mapIndex;
    }

    // 저장하기
    public void SaveStageDataToBackend()
    {
        StageSaveData saveData = new StageSaveData
        {
            stageWave = _stageWave,
            mapIndex = _currentMapIndex
        };

        string json = JsonUtility.ToJson(saveData);
        Param param = new Param();
        param.Add("StatData", json);
        param.Add("StatWave", _stageWave);


        var bro = Backend.GameData.Insert("USER_STAGEWAVE", param);

        if (bro.IsSuccess())
        {
            Debug.Log("스테이지 데이터 저장 성공");
            stageRowInDate = bro.GetInDate();

            // 리더보드 등록
            RankingManager.Instance.UpdateUserLeaderboardScore(_stageWave);
        }
        else
        {
            Debug.LogError("스테이지 데이터 저장 실패: " + bro);
        }

        SpawnManager.Instance.StartWave(_stageWave); // 웨이브 적 소환
    }

    // 수정하기
    public void UpdateStageDataToBackend()
    {
        if (string.IsNullOrEmpty(stageRowInDate))
        {
            Debug.LogError("먼저 SaveStageDataToBackend()를 통해 데이터 삽입이 필요합니다.");
            return;
        }

        StageSaveData saveData = new StageSaveData
        {
            stageWave = _stageWave,
            mapIndex = _currentMapIndex
        };

        string json = JsonUtility.ToJson(saveData);
        Param param = new Param();
        param.Add("StatData", json);
        param.Add("StatWave", _stageWave);

        var bro = Backend.GameData.UpdateV2("USER_STAGEWAVE", stageRowInDate, Backend.UserInDate, param);

        if (bro.IsSuccess())
        {
            Debug.Log("스테이지 데이터 수정 성공");

            // 리더보드 등록
            RankingManager.Instance.UpdateUserLeaderboardScore(_stageWave);
        }
        else
            Debug.LogError("스테이지 데이터 수정 실패: " + bro);
    }

    // 불러오기
    public void LoadStageDataFromBackend()
    {
        var bro = Backend.GameData.GetMyData("USER_STAGEWAVE", new Where());

        if (!bro.IsSuccess())
        {
            Debug.LogError("스테이지 데이터 로드 실패: " + bro);
            return;
        }

        JsonData rows = bro.FlattenRows();
        if (rows.Count <= 0)
        {
            Debug.Log("스테이지 저장 데이터 없음, 신규 저장");
            SaveStageDataToBackend();
            return;
        }

        stageRowInDate = rows[0]["inDate"].ToString();
        string json = rows[0]["StatData"].ToString();

        StageSaveData saveData = JsonUtility.FromJson<StageSaveData>(json);
        _stageWave = saveData.stageWave;
        _currentMapIndex = saveData.mapIndex; // 저장된 맵 인덱스 사용

        Debug.Log($"스테이지 데이터 불러오기 완료: 웨이브 {_stageWave}, 맵 {_currentMapIndex}");

        ApplyCurrentMap();                   // 맵 머티리얼 적용
        ResetTimer();                        // 타이머 초기화
        SpawnManager.Instance.StartWave(_stageWave); // 웨이브 적 소환
        SkillManager.Instance.SkillSlotUnlocked();

        // 리더보드 등록
        RankingManager.Instance.UpdateUserLeaderboardScore(_stageWave);
    }

}