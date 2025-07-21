
// 플레이어의 사망 상태를 정의하는 PlayerDieState 클래스 (PlayerController에 적용)
using UnityEngine;

public class PlayerDieState : State<PlayerController>
{
    public override void OnInitialized(){ }

    public override void OnEnter()
    {
        base.OnEnter();

        // 애니메이션 파라미터 값 초기화
        context.IsMove = false;
        context.IsAttack = false;
        context.IsDamaged = false;
        context.IsDeath = true;

        // 사망 모션 트리거 실행
        context.Animator.SetTrigger(Define.Death);
    }

    public override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);

        // 만약 부활했을 경우
        if (!context.IsDeath)
        {
            // 대기 상태 (IdleState)로 전환
            stateMachine.ChangeState<PlayerIdleState>();
            return;
        }
    }

    public override void OnExit()
    {
        base.OnExit();

        // 애니메이션 파라미터 값 초기화
        context.IsDeath = false;
    }
}