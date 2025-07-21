using UnityEngine;

[CreateAssetMenu(fileName = "NewBuffData", menuName = "Buff/BuffData")]
public class BuffData : ScriptableObject
{
    public string buffName;
    public string description;
    public Sprite icon;
    public BuffType type;
    public float value;
    public float duration;
}

public enum BuffType
{
    AttackUp,
    MoveSpeed,
    CritChance,
    DefenseUp,
    // 더 버프 추가 가능
}
