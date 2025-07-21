using UnityEngine;

// 플레이어의 이동 상태를 정의하는 PlayerMoveState 클래스 (PlayerController에 적용)
public class PlayerMoveState : State<PlayerController>
{
    // 캐릭터의 이동 속도
    private float _moveSpeed;

    public override void OnInitialized() {}

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

        _moveSpeed = PlayerStats.Instance.MoveSpeed;

        Move();
    }

    public override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);

        context.UpdateFacingByInput(GameManager.Instance.MoveDirection);

        // 체력이 0이하로 내려갈 경우
        if (context.IsDeath)
        {
            // 사망 상태 (DieState)로 전환
            stateMachine.ChangeState<PlayerDieState>();
        }
        // 입력 방향이 없는 상태일 경우
        else if (GameManager.Instance.MoveDirection == Vector3.zero)
        {
            // 이동 상태 (IdleState)로 전환
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

    // 이동 및 스프라이트 반전을 처리하는 함수
    private void Move()
    {
        Vector3 direction = GameManager.Instance.MoveDirection;
        Vector3 targetPosition = context.Rigidbody.position + direction.normalized * _moveSpeed * Time.fixedDeltaTime;
        context.Rigidbody.MovePosition(targetPosition);
    }
}