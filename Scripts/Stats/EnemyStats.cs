using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Enemy Stats Data")]

public class EnemyStats : ScriptableObject
{
    public float Health;
    public float MoveSpeed;
    public float AttackRange;
    public float AttackDamage;
    public float AttackCooldown;

    // 웨이브 수에 따라 강화된 EnemyStats 인스턴스를 반환
    public EnemyStats Clone()
    {
        EnemyStats clone = CreateInstance<EnemyStats>();

        clone.Health = this.Health;
        clone.MoveSpeed = this.MoveSpeed;
        clone.AttackRange = this.AttackRange;
        clone.AttackDamage = this.AttackDamage;
        clone.AttackCooldown = this.AttackCooldown;

        return clone;
    }
}