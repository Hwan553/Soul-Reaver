using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Map/MapData")]
public class MapData : ScriptableObject
{
    [Header("기본 정보")]
    public string mapName;

    [Header("맵에 적용할 머티리얼들")]
    public List<Material> materials; // 지형, 바닥, 벽 등에 적용할 머티리얼 리스트

    [Header("스폰 정보")]
    public EnemyMapSet enemySetData;
}
