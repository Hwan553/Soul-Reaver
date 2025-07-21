using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Enemy Map Set Data")]

public class EnemyMapSet : ScriptableObject
{
    public List<GameObject> enemy;
    public List<GameObject> boss;
}