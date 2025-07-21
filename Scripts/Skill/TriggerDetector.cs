using System.Collections.Generic;
using UnityEngine;

public class TriggerDetector : MonoBehaviour
{
    // ============================================================
    // 🔎 [감지 설정] - 감지 대상 레이어 및 반경 설정
    // ============================================================
    [Header("▶ 감지 설정")]
    [Tooltip("감지 대상 레이어 (예: Enemy)")]
    [SerializeField] private LayerMask _targetLayer;

    [Tooltip("파티클 기준 적 감지 반경")]
    [SerializeField] private float _detectRadius = 0.3f;

    // ============================================================
    // 🔥 [파티클 관련] - 파티클 시스템 및 트리거 감지 정보
    // ============================================================
    [Header("▶ 파티클 설정")]
    [Tooltip("파티클 시스템")]
    [SerializeField] private ParticleSystem _particleSystem;

    [Tooltip("트리거에 들어온 파티클 리스트")]
    [SerializeField] private List<ParticleSystem.Particle> _enterParticles;

    [Tooltip("OverlapSphereNonAlloc용 충돌체 버퍼")]
    [SerializeField] private Collider[] _hitBuffer;

    // ============================================================
    // 🧱 [더미 콜라이더] - 파티클 트리거 기능 활성화용
    // ============================================================
    [Header("▶ 트리거 콜라이더 (더미)")]
    [Tooltip("Trigger 이벤트 활성화를 위한 더미 콜라이더")]
    [SerializeField] private Collider _dummyCollider;

    // ============================================================
    // 🧠 [스킬 정보] - 적용할 스킬 및 데미지 정보
    // ============================================================
    [Header("▶ 스킬 정보")]
    [Tooltip("적용할 스킬")]
    [SerializeField] private Skill_Base _skill;

    [Tooltip("적용할 데미지 수치")]
    [SerializeField] private float _damage;

    private void Awake()
    {
        Setting();
    }

    // 초기 설정 함수
    private void Setting()
    {
        _particleSystem = GetComponent<ParticleSystem>();       // 파티클 시스템 컴포넌트 가져오기
        _enterParticles = new List<ParticleSystem.Particle>();  // 입장 파티클 리스트 초기화
        _hitBuffer = new Collider[10];  // 충돌 검사 시 사용할 버퍼 배열 초기화

        _particleSystem.trigger.SetCollider(0, _dummyCollider); // 파티클 시스템 트리거 모듈에 더미 콜라이더 등록

        //_skill = null;
        _damage = 0.0f;
    }

    private void OnDrawGizmos()
    {
        // 파티클 시스템이 없으면 종료
        if (_particleSystem == null) _particleSystem = GetComponent<ParticleSystem>();
        if (_particleSystem == null) return;

        // 이펙트가 월드 공간이면 transform.position 기준, 아니면 TransformPoint(Vector3.zero) 기준으로 구체 그리기
        Vector3 centerPos = _particleSystem.main.simulationSpace == ParticleSystemSimulationSpace.World
            ? transform.position
            : transform.TransformPoint(Vector3.zero);

        Gizmos.color = new Color(1f, 0f, 0f, 0.25f); // 반투명 빨간색
        Gizmos.DrawSphere(centerPos, _detectRadius);

        // 더미 콜라이더도 표시 (초록색 와이어 구체)
        if (_dummyCollider != null)
        {
            Gizmos.color = Color.green;
            SphereCollider sphere = _dummyCollider as SphereCollider;

            if (sphere != null)
            {
                Vector3 colliderPos = _dummyCollider.transform.position + sphere.center;
                Gizmos.DrawWireSphere(colliderPos, sphere.radius * _dummyCollider.transform.lossyScale.x);
            }
        }
    }

    public void SetSkillInfo(Skill_Base skill, float damage)
    {
        _skill = skill;
        _damage = damage;
    }

    // 파티클 트리거 이벤트 콜백 함수
    private void OnParticleTrigger()
    {
        // 이전 데이터 클리어
        _enterParticles.Clear();
        // 트리거에 들어온 파티클들 가져오기
        _particleSystem.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, _enterParticles);

        foreach (var p in _enterParticles)
        {
            // 파티클 좌표가 월드 공간인지 로컬 공간인지에 따라 계산
            Vector3 worldPos = _particleSystem.main.simulationSpace == ParticleSystemSimulationSpace.World ? p.position : transform.TransformPoint(p.position);

            // DetectRadius 반경 내에 TargetLayer에 해당하는 충돌체 검색 (Trigger 콜라이더 포함)
            int hitCount = Physics.OverlapSphereNonAlloc(worldPos, _detectRadius, _hitBuffer, _targetLayer, QueryTriggerInteraction.Collide);

            for (int i = 0; i < hitCount; ++i)
            {
                Collider hit = _hitBuffer[i];

                if (hit.tag == Define.Enemy_Tag)
                {
                    EnemyController enemy = hit.GetComponent<EnemyController>();

                    if (enemy != null)
                    {
                        if (_skill != null && _damage > 0.0f)
                        {
                            Debug.Log($"{enemy.name}가 {_skill.name} 스킬로 인해 {_damage} 데미지를 받음!!");
                            enemy.TakeDamage(_damage);  // 기본 데미지 적용
                        }
                    }
                }
            }
        }
    }
}