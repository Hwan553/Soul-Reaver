using UnityEngine;

// 플레이어의 대기 상태를 정의하는 PlayerIdleState 클래스 (PlayerController에 적용)
public class PlayerIdleState : State<PlayerController>
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
        // 입력 방향이 있는 상태일 경우
        else if (GameManager.Instance.MoveDirection != Vector3.zero)
        {
            // 이동 상태 (MoveState)로 전환
            stateMachine.ChangeState<PlayerMoveState>();
            return;
        }
        // 타겟이 있는 경우
        else if (GameManager.Instance.MoveDirection == Vector3.zero && context.Target != null)
        {
            // 추적 상태 (PursuitState)로 전환
            stateMachine.ChangeState<PlayerPursuitState>();
            return;
        }
    }
}