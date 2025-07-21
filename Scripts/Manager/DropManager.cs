using System.Collections.Generic;
using UnityEngine;

public class DropManager : Singleton<DropManager>
{
    [Header("��� �ݰ�")]
    [SerializeField] private float goldDropRadius = 3.0f;
    [SerializeField] private float gachaDropRadius = 5.0f;
    [SerializeField] private float runeDropRadius = 7.0f;

    [Header("��ȭ ��� ������")]
    [SerializeField] private DropGoodsData normalDropData;
    [SerializeField] private DropGoodsData bossDropData;

    [Header("��ȭ ������")]
    [SerializeField] private GameObject goldPrefab;
    [SerializeField] private GameObject gachaPrefab;

    [Header("�� ������")]
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
        // ���� ���ο� ���� ��� ������ ���� (�Ϲ� ���� �Ǵ� ���� ��� ���̺�)
        DropGoodsData dropData = isBoss ? bossDropData : normalDropData;

        // ��� ��� ����
        DropItems(goldPrefab, dropPos, dropData.GetGoldAmount(), goldDropRadius, GoodsType.Gold, 10);

        if (isBoss)
        {
            // ������ ���: �߰� ���� ���

            // ��í ��ȭ ���
            DropItems(gachaPrefab, dropPos, dropData.GetGachaAmount(), gachaDropRadius, GoodsType.Gp, 5);

            // �� ����� Ȯ�� ����̹Ƿ� TryDropRune���� ó��
            TryDropRune(dropPos);
        }
    }

    private void DropItems(GameObject prefab, Vector3 dropOrigin, int count, float radius, GoodsType type, int baseValue)
    {
        // ��� �������� �������� ���� ��� ��� ���
        if (prefab == null)
        {
            Debug.LogWarning($"[DropManager] Prefab for {type} is null. Check assignment.");
            return;
        }

        for (int i = 0; i < count; i++)
        {
            // ������ �������� ������ ���� ��ġ �ȿ��� ���� ��ġ ���� (�ڿ������� ��������)
            Vector3 offset = dropOrigin + Random.insideUnitSphere * radius;
            offset.y = 1.0f; // y ���̴� �����ϰ� ���� (���� ���� ���)

            // ������Ʈ Ǯ���� ��� ������ ����
            GameObject drop = PoolManager.Instance.ActivateObj(prefab, offset, Quaternion.identity);

            // ��ӵ� ������Ʈ�� GoodsItem ������Ʈ�� �ִ��� Ȯ�� �� �ʱ�ȭ
            if (drop.TryGetComponent<GoodsItem>(out var item))
            {
                item.Initialize(baseValue, type); // �⺻�� �� Ÿ�� ���� (ex. ��� 10, Ÿ�� Gold)
            }
            else
            {
                Debug.LogError($"[DropManager] Missing GoodsItem component on {prefab.name}");
            }
        }
    }

    private void TryDropRune(Vector3 dropOrigin)
    {
        // �� ������ ����Ʈ�� ������� �ƹ��͵� ���� ����
        if (runePrefabs == null || runePrefabs.Count == 0) return;

        float rand = Random.value; // 0.0 ~ 1.0 ������ ���� ��

        if (rand > runeDropChance) return;
        // ������ Ȯ������ ũ�� ��� ���� �� �� ��� �� ��

        int index = Random.Range(0, runePrefabs.Count); // �� ������ ����Ʈ���� ������ ����
        GameObject rune = runePrefabs[index];

        if (rune == null)
        {
            Debug.LogWarning("[DropManager] Selected rune prefab is null.");
            return;
        }

        // ��ġ�� �������� ���� ��Ѹ��� ����
        Vector3 offset = dropOrigin + Random.insideUnitSphere * runeDropRadius;
        offset.y = 1.0f;

        // ���õ� �� �������� ������Ʈ Ǯ���� Ȱ��ȭ
        PoolManager.Instance.ActivateObj(rune, offset, Quaternion.identity);

        // ����� �α� ���
        Debug.Log($"[DropManager] �� ��ӵ�: {rune.name} (Chance: {runeDropChance * 100}%)");
    }
}
