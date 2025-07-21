using System.Collections;
using UnityEngine;

public class EnemyAttackState : State<EnemyController>
{
    private Coroutine _coAttackAnim;

    public override void OnInitialized()
    {
        _coAttackAnim = null;
    }

    public override void OnEnter()
    {
        base.OnEnter();

        // 애니메이션 파라미터 값 초기화
        context.IsMove = false;
        context.IsAttack = true;
        context.IsDamaged = false;
        context.IsDeath = false;

        // 공격 모션 트리거 실행
        context.Animator.SetTrigger(Define.Attack);

        // 공격 딜레이 코루틴 실행
        if (_coAttackAnim == null)
        {
            switch (context.EnemyClassType)
            {
                case ClassType.Boss:
                    _coAttackAnim = context.StartCoroutine(CoBossAttack());
                    break;
                case ClassType.Warrior:
                    _coAttackAnim = context.StartCoroutine(CoWarriorAttack());
                    break;
                case ClassType.Archer:
                    _coAttackAnim = context.StartCoroutine(CoArcherAttack());
                    break;
                case ClassType.Wizard:
                    _coAttackAnim = context.StartCoroutine(CoWizardAttack());
                    break;
            }
        }
    }

    public override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);

        // 체력이 0이하로 내려갈 경우
        if (context.IsDeath)
        {
            // 사망 상태 (DieState)로 전환
            stateMachine.ChangeState<EnemyDieState>();
            return;
        }
        // 피격 상태일 경우
        else if (context.IsDamaged)
        {
            // 피격 상태 (HitState)로 전환
            stateMachine.ChangeState<EnemyHitState>();
            return;
        }

        // 공격 딜레이가 끝난 경우
        if (!context.IsAttack)
        {
            // 타겟이 존재하는 경우
            if (context.Target != null)
            {
                // 공격 범위 밖에 타겟이 있는 경우
                if (context.TargetDistance > 3.0f)
                {
                    // 추적 상태 (PursuitState)로 전환
                    stateMachine.ChangeState<EnemyPursuitState>();
                    return;
                }
                // 공격 범위 안에 타겟이 있는 경우 상태 리로드
                else
                {
                    // 공격 상태 (Attacktate)로 전환
                    stateMachine.ReloadState<EnemyAttackState>();
                    return;
                }
            }

            // 타겟이 없는 경우
            if (context.Target == null)
            {
                // 대기 상태 (IdleState)로 전환
                stateMachine.ChangeState<EnemyIdleState>();
                return;
            }
        }
    }

    public override void OnExit()
    {
        base.OnExit();

        // 애니메이션 파라미터 값 초기화
        context.IsAttack = false;
    }

    private void PerformAreaDamage(float offset, float radius, float damage)
    {
        // 1) 판정 지점 계산
        Vector3 hitPoint = context.transform.position + context.transform.forward * offset;

        // 2) OverlapSphere를 사용할 때, whatIsPlayer 레이어 마스크를 추가합니다.
        // 이렇게 하면 'Player' 레이어에 속한 콜라이더만 감지합니다.
        Collider[] hitColliders = Physics.OverlapSphere(hitPoint, radius, context.TargetLayer);

        // 3) 감지된 Collider는 이제 플레이어 뿐이므로, 바로 데미지를 줍니다.
        foreach (var col in hitColliders)
        {
            // TryGetComponent는 여전히 좋은 습관입니다.
            if (col.TryGetComponent<IDamageable>(out var target))
            {
                target.TakeDamage(damage, context.transform);
            }
        }
    }

    private IEnumerator CoBossAttack()
    {
        // 애니메이션 상에서 타격이 일어나는 시점(예: 0.3초)만큼 대기
        yield return new WaitForSeconds(0.3f);

        // PerformAreaDamage 헬퍼 호출 → hitOffset, hitRadius, damage만 넘겨 주면 된다.
        //PerformAreaDamage(_hitOffset, _hitRadius, context.EnemyRuntimeStats.AttackDamage);S

        // 쿨다운 대기
        yield return new WaitForSeconds(context.RuntimeStats.AttackCooldown);

        context.IsAttack = false;

        context.StopCoroutine(_coAttackAnim);
        _coAttackAnim = null;
    }

    private IEnumerator CoWarriorAttack()
    {
        //Warrior warrior = context as Warrior;
        Enemy enemy = context as Enemy;

        // 애니메이션 상에서 타격이 일어나는 시점(예: 0.3초)만큼 대기
        yield return new WaitForSeconds(0.3f);

        // PerformAreaDamage 헬퍼 호출 → hitOffset, hitRadius, damage만 넘겨 주면 된다.
        PerformAreaDamage(enemy.HitOffset, enemy.HitRadius, enemy.RuntimeStats.AttackDamage);

        // 쿨다운 대기
        yield return new WaitForSeconds(context.RuntimeStats.AttackCooldown);

        context.IsAttack = false;

        context.StopCoroutine(_coAttackAnim);
        _coAttackAnim = null;
    }

    private IEnumerator CoArcherAttack()
    {
        //Archer archer = context as Archer;
        Enemy enemy = context as Enemy;

        yield return new WaitForSeconds(0.4f); // 발사 타이밍

        if (enemy.ArrowProjectile != null && context.Target != null)
        {
            Vector3 direction = (context.Target.transform.position - context.transform.position).normalized;
            direction.y = 0.0f;

            Vector3 spawnOffset = new Vector3(0.0f, 1.5f, 0.0f);
            Vector3 spawnPos = context.transform.position + direction * enemy.ArrowFireOffset + spawnOffset;

            GameObject projectile = PoolManager.Instance.ActivateObj(enemy.ArrowProjectile, spawnPos, Quaternion.identity);
            ArrowProjectile arrow = projectile.GetComponent<ArrowProjectile>();

            if (arrow != null)
            {
                // 적의 공격력(runtimeStats.attackDamage)을 투사체에 설정
                arrow.SetArrowDamage(context.RuntimeStats.AttackDamage);

                // 투사체 방향 설정
                arrow.SetDirectionWithAtan2(context.Target.transform.position);
                arrow.SetArrowSpeed(enemy.ArrowProjectileSpeed);
            }
        }

        yield return new WaitForSeconds(0.2f);

        context.IsAttack = false;

        context.StopCoroutine(_coAttackAnim);
        _coAttackAnim = null;
    }

    private IEnumerator CoWizardAttack()
    {
        //Wizard wizard = context as Wizard;
        Enemy enemy = context as Enemy;

        yield return new WaitForSeconds(0.4f); // 발사 타이밍

        if (enemy.MagicProjectile != null && context.Target != null)
        {
            Vector3 direction = (context.Target.transform.position - context.transform.position).normalized;
            direction.y = 0.0f;

            Vector3 spawnOffset = new Vector3(0.0f, 1.5f, 0.0f);
            Vector3 spawnPos = context.transform.position + direction * enemy.MagicFireOffset + spawnOffset;

            GameObject projectile = PoolManager.Instance.ActivateObj(enemy.MagicProjectile, spawnPos, Quaternion.identity);
            MagicProjectile magicBall = projectile.GetComponent<MagicProjectile>();

            if (magicBall != null)
            {
                // 적의 공격력(runtimeStats.attackDamage)을 투사체에 설정
                magicBall.SetMagicDamage(context.RuntimeStats.AttackDamage);

                // 투사체 방향 설정
                magicBall.SetDirectionWithAtan2(context.Target.transform.position);
                magicBall.SetMagicSpeed(enemy.MagicProjectileSpeed);
            }
        }

        yield return new WaitForSeconds(context.RuntimeStats.AttackCooldown);

        context.IsAttack = false;

        context.StopCoroutine(_coAttackAnim);
        _coAttackAnim = null;
    }
}