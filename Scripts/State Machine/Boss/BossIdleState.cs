using UnityEngine;

public class BossIdleState : State<BossController>
{
    public override void OnInitialized() { }

    public override void OnEnter()
    {
        base.OnEnter();

        Debug.Log($"<color=red>{context.name} State : {this.GetType().Name}</color>");

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
            stateMachine.ChangeState<BossDieState>();
            return;
        }
        // 피격 상태일 경우
        else if (context.IsDamaged)
        {
            // 피격 상태 (HitState)로 전환
            stateMachine.ChangeState<BossHitState>();
            return;
        }
        // 타겟이 있는 경우
        else if (context.Target != null)
        {
            // 추적 상태 (PursuitState)로 전환
            stateMachine.ChangeState<BossPursuitState>();
            return;
        }
    }
}