using UnityEngine;
using UnityEngine.AI;

public class EnemyPursuitState : State<EnemyController>
{
    private Vector3 _direction;

    // 현재 캐릭터가 오른쪽을 보고 있는지 여부
    private bool _isFacingRight;

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

        // 캐릭터의 현재 방향 (Local Scale 기준)으로 IsFacingRight 설정
        context.IsFacingRight = context.SpriteObject.localScale.x < 0.0f;
    }

    public override void OnFixedUpdate(float fixedDeltaTime)
    {
        base.OnFixedUpdate(fixedDeltaTime);

        MoveToPlayer();
    }

    public override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);

        FlipSprite();

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
        // 타겟이 존재하며, 공격 범위 이내에 있을 경우
        else if (context.Target != null && context.TargetDistance <= context.RuntimeStats.AttackRange)
        {
            // 공격 상태 (AttackState)로 전환
            stateMachine.ChangeState<EnemyAttackState>();
            return;
        }
        // 타겟이 존재하지 않는 경우
        else if (context.Target == null)
        {
            // 대기 상태 (IdleState)로 전환
            stateMachine.ChangeState<EnemyIdleState>();
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

    private void MoveToPlayer()
    {
        if (context.Target == null) return;
        if (context.NavMeshAgent == null || !context.NavMeshAgent.isOnNavMesh) return;

        context.NavMeshAgent.SetDestination(context.Target.transform.position);

        Vector3 direction = (context.NavMeshAgent.steeringTarget - context.transform.position).normalized;
        direction.y = 0.0f;

        if (direction != Vector3.zero)
        {
            context.Rigidbody.MovePosition(context.transform.position + direction * context.RuntimeStats.MoveSpeed * Time.deltaTime);
        }
    }

    private void FlipSprite()
    {
        if (context.Target != null)
        {
            // 플레이어와 나의 위치 벡터 계산
            Vector3 directionToPlayer = context.Target.transform.position - context.transform.position;

            bool isMovingRight = directionToPlayer.x > 0 ? true : false;
            context.IsFacingRight = isMovingRight;

            Vector3 scale = context.SpriteObject.localScale;
            scale.x = Mathf.Abs(scale.x) * (isMovingRight ? -1 : 1);
            context.SpriteObject.localScale = scale;
        }
    }
}