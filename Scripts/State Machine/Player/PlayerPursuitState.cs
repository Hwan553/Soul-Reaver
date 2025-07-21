using UnityEngine;

// 플레이어의 추적 상태를 정의하는 PlayerPursuitState 클래스 (PlayerController에 적용)
public class PlayerPursuitState : State<PlayerController>
{
    private Vector3 _direction;

    // 캐릭터의 이동 속도
    private float _moveSpeed;

    public override void OnInitialized()
    {
        // 변수 초기화
        _direction = Vector3.zero;
    }

    public override void OnEnter()
    {
        base.OnEnter();

        // 애니메이션 파라미터 값 초기화
        context.IsMove = true;
        context.IsAttack = false;
        context.IsDamaged = false;
        context.IsDeath = false;

        _moveSpeed = PlayerStats.Instance.MoveSpeed;
    }

    public override void OnFixedUpdate(float fixedDeltaTime)
    {
        base.OnFixedUpdate(fixedDeltaTime);

        TargetToMove();
    }

    public override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);

        context.UpdateFacingByTarget();

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
        // 타겟이 존재하며, 공격 범위 이내에 있을 경우
        else if (context.Target != null && context.TargetDistance <= 3.0f)
        {
            // 공격 상태 (AttackState)로 전환
            stateMachine.ChangeState<PlayerAttackState>();
            return;
        }
        // 타겟이 존재하지 않는 경우
        else if (context.Target == null)
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
        context.IsMove = false;
        context.IsAttack = false;
        context.IsDamaged = false;
        context.IsDeath = false;
    }

    private void TargetToMove()
    {
        if (context.Target == null) return;

        Vector3 position = context.Rigidbody.position; // Rigidbody 기준 위치 사용
        Vector3 targetPosition = context.Target.transform.position;

        _direction = targetPosition - position;

        Vector3 move = _direction.normalized * _moveSpeed * Time.fixedDeltaTime;
        Vector3 targetMovePosition = position + move;

        context.Rigidbody.MovePosition(targetMovePosition);
    }
}