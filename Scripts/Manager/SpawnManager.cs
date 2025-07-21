using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : Singleton<SpawnManager>
{
    [Header("Kill Count")]
    private int _enemiesKilled;
    private int _bossKilled;

    [Header("Wave Settings")]
    private int _enemiesToKillForNextWave;
    private int _bossToKillForNextWave;

    [Header("Spawn Settings")]
    public Transform player;
    public float SpawnRadius;

    private Dictionary<string, Transform> _enemyParentMap;
    private Dictionary<string, Transform> _bossParentMap;
    private KillCountCardFlipButton KillCountCardFlipButton;

    private bool waveInProgress = false;

    private void Start()
    {
        KillCountCardFlipButton = FindAnyObjectByType<KillCountCardFlipButton>();

        Setting();

        CacheEnemyParents();
        CacheBossParents();
    }

    private void Setting()
    {
        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag(Define.Player_Tag);

            if (playerObject != null)
                player = playerObject.transform;
            else
                Debug.LogError("SpawnManager : Player not found in Scene!");
        }

        _enemiesKilled = 0;
        _bossKilled = 0;

        _enemiesToKillForNextWave = 0;
        _bossToKillForNextWave = 0;

        SpawnRadius = 30.0f;

    }

    public void StartWave(int wave = -1) // 기본 파라미터는 -1 (외부에서 안 넘기면 -1로 시작)
    {
        if (waveInProgress) return; // 이미 웨이브가 진행 중이면 중복 실행 방지

        waveInProgress = true;      // 현재 웨이브가 진행 중임을 표시
        _enemiesKilled = 0;         // 이번 웨이브에서 죽인 일반 몬스터 수 초기화
        _bossKilled = 0;            // 이번 웨이브에서 죽인 보스 수 초기화

        if (wave % 10 == 0)         // 10의 배수 웨이브면 보스 웨이브
        {
            _enemiesToKillForNextWave = 1; // 보스 웨이브는 1마리만 처치하면 다음으로 넘어감
            StartCoroutine(SpawnBossWave()); // 보스 스폰 코루틴 시작
        }
        else
        {
            int spawnCount = 5 + wave * 2; // 일반 웨이브: 웨이브 수에 따라 점진적으로 적 수 증가
            _enemiesToKillForNextWave = spawnCount; // 이번 웨이브에서 죽여야 하는 적 수 설정
            StartCoroutine(SpawnEnemyWave(spawnCount)); // 일반 적 스폰 코루틴 시작
        }
    }

    private IEnumerator SpawnBossWave()
    {
        int wave = StageManager.Instance.StageWave; // 현재 웨이브 번호 가져오기

        SpawnBoss(); // 보스 소환 (구현된 별도 메서드라고 가정)

        // 보스를 처치할 때까지 대기 (1마리만 처치하면 되므로 1 이상인지 확인)
        yield return new WaitUntil(() => _bossKilled >= _enemiesToKillForNextWave);

        StageManager.Instance.NextStage(); // 다음 웨이브로 넘어갈 준비 (스테이지 내부 상태 갱신)

        int nextWave = wave + 1; // 다음 웨이브 번호 계산

        // 맵 인덱스 갱신: 10웨이브마다 다음 맵으로 전환되며 최대값은 Maps.Count - 1로 제한
        StageManager.Instance.CurrentMapIndex = Mathf.Min((nextWave - 1) / 10, StageManager.Instance.Maps.Count - 1);
        StageManager.Instance.ApplyCurrentMap(); // 맵 전환 로직 수행

        yield return StartCoroutine(SpawnAndWaitForEnemies()); // 일반 몬스터 추가 스폰? 맵 연출용일 수도 있음

        waveInProgress = false; // 현재 웨이브 종료 처리
        StartWave(nextWave + 1); // 다음 웨이브 시작
    }

    private IEnumerator SpawnEnemyWave(int spawnCount)
    {
        int wave = StageManager.Instance.StageWave; // 현재 웨이브 번호 가져오기

        yield return StartCoroutine(SpawnAndWaitForEnemies()); // 적 스폰 및 처치 완료까지 대기

        StageManager.Instance.NextStage(); // 다음 웨이브로 내부 상태 업데이트

        waveInProgress = false; // 웨이브 종료 상태로 설정
        StartWave(wave + 1); // 다음 웨이브 시작
    }

    private IEnumerator SpawnAndWaitForEnemies()
    {
        int wave = StageManager.Instance.StageWave;

        int spawnCount = 3 + wave * 2;
        _enemiesKilled = 0;
        _enemiesToKillForNextWave = spawnCount;

        yield return new WaitForSeconds(1.5f);

        for (int i = 0; i < spawnCount; i++)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitUntil(() => _enemiesKilled >= _enemiesToKillForNextWave);
    }

    private void SpawnEnemy()
    {
        int wave = StageManager.Instance.StageWave;

        var enemySet = StageManager.Instance.GetCurrentEnemySet();
        var enemyList = enemySet?.enemy;

        if (enemyList == null || enemyList.Count == 0)
        {
            Debug.LogWarning("SpawnManager : No enemies defined for current map.");
            return;
        }

        GameObject prefab = enemyList[Random.Range(0, enemyList.Count)];
        Vector3 spawnPos = GetRandomSpawnPosition();
        GameObject enemy = PoolManager.Instance.ActivateObj(prefab, spawnPos, Quaternion.identity);

        if (enemy.TryGetComponent(out EnemyController enemyBase))
        {
            enemyBase.SetSpawnManager(this);
            enemyBase.InitializeStats(wave);
        }

        string enemyType = GetEnemyType(prefab.name); // 이름에 따라 종류 추출

        if (_enemyParentMap.TryGetValue(enemyType, out var parent))
        {
            enemy.transform.SetParent(parent);
        }
    }

    private void SpawnBoss()
    {
        int wave = StageManager.Instance.StageWave;

        var enemySet = StageManager.Instance.GetCurrentEnemySet();
        var bossList = enemySet?.boss;

        if (bossList == null || bossList.Count == 0)
        {
            Debug.LogWarning("SpawnManager : No bosses defined for current map.");
            return;
        }

        GameObject bossPrefab = bossList[Random.Range(0, bossList.Count)];
        Vector3 spawnPos = GetRandomSpawnPosition();
        GameObject boss = PoolManager.Instance.ActivateObj(bossPrefab, spawnPos, Quaternion.identity);

        if (boss.TryGetComponent(out BossController bossBase))
        {
            bossBase.SetSpawnManager(this);
            bossBase.InitializeStats(wave);

            Debug.Log("<color=yellow>Spawned Boss : " + boss.name + " successfully.</color>");
        }
        else
        {
            Debug.LogError("SpawnManager : Failed to get BossController on'" + boss.name + "'", boss);
        }

        string bossType = GetBossType(bossPrefab.name);

        if (_bossParentMap.TryGetValue(bossType, out var parent))
        {
            boss.transform.SetParent(parent);
        }
        else
        {
            Debug.LogWarning($"SpawnManager : No parent found for Boss type '{bossType}'");
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        Vector2 offset = Random.insideUnitCircle * SpawnRadius;
        return new Vector3(player.position.x + offset.x, 0.15f, player.position.z + offset.y);
    }

    private void CacheEnemyParents()
    {
        _enemyParentMap = new Dictionary<string, Transform>();

        string[] types = { "Human", "Elf", "Devil", "Skeleton" };

        foreach (string type in types)
        {
            var parentObj = GameObject.Find($"{type} Enemy");

            if (parentObj != null)
            {
                _enemyParentMap[type] = parentObj.transform;
            }
            else
            {
                Debug.LogWarning($"SpawnManager : '{type} Enemy' parent object not found in scene.");
            }
        }
    }

    private void CacheBossParents()
    {
        _bossParentMap = new Dictionary<string, Transform>();

        string[] types = { "Human", "Elf", "Devil", "Skeleton" };

        foreach (string type in types)
        {
            var parentObj = GameObject.Find($"{type} Boss");

            if (parentObj != null)
            {
                _bossParentMap[type] = parentObj.transform;
            }
            else
            {
                Debug.LogWarning($"SpawnManager : '{type} Boss' parent object not found in scene.");
            }
        }
    }

    private string GetEnemyType(string prefabName)
    {
        if (prefabName.Contains("Human")) return "Human";
        if (prefabName.Contains("Elf")) return "Elf";
        if (prefabName.Contains("Devil")) return "Devil";
        if (prefabName.Contains("Skeleton")) return "Skeleton";

        return "Unknown";
    }

    private string GetBossType(string prefabName)
    {
        if (prefabName.Contains("Human")) return "Human";
        if (prefabName.Contains("Elf")) return "Elf";
        if (prefabName.Contains("Devil")) return "Devil";
        if (prefabName.Contains("Skeleton")) return "Skeleton";

        return "Unknown";
    }

    public void StopWave()
    {
        // 진행 중인 모든 스폰 코루틴 정지
        StopAllCoroutines();

        // 플래그 초기화
        waveInProgress = false;

        // 필요하면 킬 카운트도 리셋
        _enemiesKilled = 0;
        _bossKilled = 0;
    }

    public void OnEnemyKilled()
    {
       _enemiesKilled++;
        KillCountCardFlipButton.OnMonsterKilled();
        QuestManager.Instance.OnMonsterKilled();
        Debug.Log($"Enemy killed! {_enemiesKilled}/{_enemiesToKillForNextWave}");
    }

    public void OnBossKilled()
    {
        _bossKilled++;
        Debug.Log($"Boss killed! {_bossKilled}/{_bossToKillForNextWave}");
    }
}