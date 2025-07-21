using System.Collections.Generic;
using UnityEngine;

public class DropManager : Singleton<DropManager>
{
    [Header("드롭 반경")]
    [SerializeField] private float goldDropRadius = 3.0f;
    [SerializeField] private float gachaDropRadius = 5.0f;
    [SerializeField] private float runeDropRadius = 7.0f;

    [Header("재화 드롭 데이터")]
    [SerializeField] private DropGoodsData normalDropData;
    [SerializeField] private DropGoodsData bossDropData;

    [Header("재화 프리팹")]
    [SerializeField] private GameObject goldPrefab;
    [SerializeField] private GameObject gachaPrefab;

    [Header("룬 프리팹")]
    [SerializeField] private List<GameObject> runePrefabs;
    [Range(0f, 1f), SerializeField] private float runeDropChance = 0.65f;

    protected override void Initialized()
    {
        if (normalDropData == null)
            normalDropData = Resources.Load<DropGoodsData>(Define.normalDropData);

        if (bossDropData == null)
            bossDropData = Resources.Load<DropGoodsData>(Define.BossDropData);

        if (goldPrefab == null)
            goldPrefab = Resources.Load<GameObject>(Define.GoldPrefab);

        if (gachaPrefab == null)
            gachaPrefab = Resources.Load<GameObject>(Define.DiamondPrefab);

        if (runePrefabs == null || runePrefabs.Count == 0)
            runePrefabs = new List<GameObject>(Resources.LoadAll<GameObject>(Define.RunePrefab_Path));

        PoolManager.Instance.PreloadDropItems(goldPrefab, 50);
        PoolManager.Instance.PreloadDropItems(gachaPrefab, 20);
    }

    public void DropFromEnemy(Vector3 dropPos, bool isBoss)
    {
        // 보스 여부에 따라 드롭 데이터 선택 (일반 몬스터 또는 보스 드롭 테이블)
        DropGoodsData dropData = isBoss ? bossDropData : normalDropData;

        // 골드 드롭 수행
        DropItems(goldPrefab, dropPos, dropData.GetGoldAmount(), goldDropRadius, GoodsType.Gold, 10);

        if (isBoss)
        {
            // 보스일 경우: 추가 보상 드롭

            // 가챠 재화 드롭
            DropItems(gachaPrefab, dropPos, dropData.GetGachaAmount(), gachaDropRadius, GoodsType.Gp, 5);

            // 룬 드롭은 확률 기반이므로 TryDropRune으로 처리
            TryDropRune(dropPos);
        }
    }

    private void DropItems(GameObject prefab, Vector3 dropOrigin, int count, float radius, GoodsType type, int baseValue)
    {
        // 드롭 프리팹이 지정되지 않은 경우 경고 출력
        if (prefab == null)
        {
            Debug.LogWarning($"[DropManager] Prefab for {type} is null. Check assignment.");
            return;
        }

        for (int i = 0; i < count; i++)
        {
            // 원점을 기준으로 랜덤한 구형 위치 안에서 생성 위치 결정 (자연스럽게 퍼지도록)
            Vector3 offset = dropOrigin + Random.insideUnitSphere * radius;
            offset.y = 1.0f; // y 높이는 일정하게 설정 (지면 위로 드롭)

            // 오브젝트 풀에서 드롭 아이템 생성
            GameObject drop = PoolManager.Instance.ActivateObj(prefab, offset, Quaternion.identity);

            // 드롭된 오브젝트에 GoodsItem 컴포넌트가 있는지 확인 후 초기화
            if (drop.TryGetComponent<GoodsItem>(out var item))
            {
                item.Initialize(baseValue, type); // 기본값 및 타입 설정 (ex. 골드 10, 타입 Gold)
            }
            else
            {
                Debug.LogError($"[DropManager] Missing GoodsItem component on {prefab.name}");
            }
        }
    }

    private void TryDropRune(Vector3 dropOrigin)
    {
        // 룬 프리팹 리스트가 비었으면 아무것도 하지 않음
        if (runePrefabs == null || runePrefabs.Count == 0) return;

        float rand = Random.value; // 0.0 ~ 1.0 사이의 랜덤 값

        if (rand > runeDropChance) return;
        // 정해진 확률보다 크면 드롭 실패 → 룬 드롭 안 함

        int index = Random.Range(0, runePrefabs.Count); // 룬 프리팹 리스트에서 무작위 선택
        GameObject rune = runePrefabs[index];

        if (rune == null)
        {
            Debug.LogWarning("[DropManager] Selected rune prefab is null.");
            return;
        }

        // 위치를 랜덤으로 조금 흩뿌리게 설정
        Vector3 offset = dropOrigin + Random.insideUnitSphere * runeDropRadius;
        offset.y = 1.0f;

        // 선택된 룬 프리팹을 오브젝트 풀에서 활성화
        PoolManager.Instance.ActivateObj(rune, offset, Quaternion.identity);

        // 디버그 로그 출력
        Debug.Log($"[DropManager] 룬 드롭됨: {rune.name} (Chance: {runeDropChance * 100}%)");
    }
}
