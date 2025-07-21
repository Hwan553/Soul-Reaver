using System.Collections.Generic;
using UnityEngine;

public class CollisionDetector : MonoBehaviour
{
    // ============================================================
    // 🔥 [파티클 관련] - 파티클 시스템 및 충돌 파티클 정보
    // ============================================================
    [Header("▶ 파티클 설정")]
    [Tooltip("충돌 감지를 위한 파티클 시스템")]
    [SerializeField] private ParticleSystem _particleSystem;

    [Tooltip("충돌한 파티클 목록 (디버깅용, OnParticleCollision에선 사용되지 않음)")]
    [SerializeField] private List<ParticleSystem.Particle> _enter = new List<ParticleSystem.Particle>();

    // ============================================================
    // 🧠 [스킬 정보] - 적용할 스킬 및 데미지 수치
    // ============================================================
    [Header("▶ 스킬 정보")]
    [Tooltip("현재 적용 중인 스킬 정보")]
    [SerializeField] private Skill_Base _skill;

    [Tooltip("스킬로 가할 데미지 수치")]
    [SerializeField] private float _damage;

    private void Awake()
    {
        Setting();
    }

    private void Setting()
    {
        _particleSystem = GetComponent<ParticleSystem>();

        _skill = null;
        _damage = 0.0f;
    }

    public void SetSkillInfo(Skill_Base skill, float damage)
    {
        _skill = skill;
        _damage = damage;
    }

    private void OnParticleCollision(GameObject other)
    {
        if (other.CompareTag(Define.Enemy_Tag))
        {
            EnemyController enemy = other.GetComponent<EnemyController>();

            if (enemy != null)
            {
                if (_skill != null && _damage > 0.0f)
                {
                    Debug.Log($"{other.name}가 {_skill.name} 스킬로 인해 {_damage} 데미지를 받음!!");
                    enemy.TakeDamage(_damage);  // 기본 데미지 적용
                }
            }
        }
    }
}