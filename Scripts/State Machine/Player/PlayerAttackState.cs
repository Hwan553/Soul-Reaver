using System.Collections;
using UnityEngine;

// 플레이어의 공격 상태를 정의하는 PlayerAttackState 클래스 (PlayerController에 적용)
public class PlayerAttackState : State<PlayerController>
{
    private Coroutine _coAttackAnimDelay;

    private PlayerStats _playerStats;
    public override void OnInitialized()
    {
        _playerStats = context.GetComponent<PlayerStats>();
        _coAttackAnimDelay = null;
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
        if (_coAttackAnimDelay == null)
        {
            _coAttackAnimDelay = context.StartCoroutine(CoAttackDelay());
        }
    }

    public override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);

        // 체력이 0이하로 내려갈 경우
        if (context.IsDeath)
        {
            // 사망 상태 (DieState)로 전환
            stateMachine.ChangeState<PlayerDieState>();
            return;
        }
        // 피격 상태일 경우
        //else if (context.IsDamaged)
        //{
        //    // 피격 상태 (HitState)로 전환
        //    stateMachine.ChangeState<PlayerHitState>();
        //    return;
        //}

        // 공격 도중 입력 방향이 감지된 경우
        if (GameManager.Instance.MoveDirection != Vector3.zero)
        {
            // 이동 상태 (MoveState)로 전환
            stateMachine.ChangeState<PlayerMoveState>();
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
                    stateMachine.ChangeState<PlayerPursuitState>();
                    return;
                }
                // 공격 범위 안에 타겟이 있는 경우 상태 리로드
                else
                {
                    // 공격 상태 (Attacktate)로 전환
                    stateMachine.ReloadState<PlayerAttackState>();
                    return;
                }
            }

            // 타겟이 없는 경우
            if (context.Target == null)
            {
                // 대기 상태 (IdleState)로 전환
                stateMachine.ChangeState<PlayerIdleState>();
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

    // 공격 후 애니메이션 길이만큼 딜레이
    private IEnumerator CoAttackDelay()
    {
        yield return null;

        AnimatorStateInfo stateInfo = context.Animator.GetCurrentAnimatorStateInfo(0);
        float baseAnimLength = stateInfo.length;

        float attackSpeed = Mathf.Max(_playerStats.AttackSpeed); // 0으로 나누는 것 방지

        // 공격 속도가 높을수록 애니메이션 길이는 짧아져야 함
        float adjustedDelay = baseAnimLength / attackSpeed;

        yield return new WaitForSeconds(adjustedDelay);

        context.PerformAttackDamage();

        context.IsAttack = false;

        context.StopCoroutine(_coAttackAnimDelay);
        _coAttackAnimDelay = null;


    }
}