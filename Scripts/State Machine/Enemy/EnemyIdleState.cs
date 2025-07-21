using UnityEngine;

public class EnemyIdleState : State<EnemyController>
{
    public override void OnInitialized() { }

    public override void OnEnter()
    {
        base.OnEnter();

        // 애니메이션 파라미터 값 초기화
        context.IsMove = false;
        context.IsAttack = false;
        context.IsDamaged = false;
        context.IsDeath = false;
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
        // 타겟이 있는 경우
        else if (context.Target != null)
        {
            // 추적 상태 (PursuitState)로 전환
            stateMachine.ChangeState<EnemyPursuitState>();
            return;
        }
    }
}