using System.Collections;
using UnityEngine;

public class EnemyDieState : State<EnemyController>
{
    private Coroutine _coDisableObject;

    public override void OnInitialized() {}

    public override void OnEnter()
    {
        base.OnEnter();

        Debug.Log($"<color=red>{context.name} State : {this.GetType().Name}</color>");

        // 애니메이션 파라미터 값 초기화
        context.IsMove = false;
        context.IsAttack = false;
        context.IsDamaged = false;
        context.IsDeath = true;

        // CapsuleCollider 비활성화
        context.CapsuleCollider.enabled = false;

        // 사망 모션 트리거 실행
        context.Animator.SetTrigger(Define.Death);

        if (_coDisableObject == null)
        {
            _coDisableObject = context.StartCoroutine(CoDisableAfterDeathAnimation());
        }
    }

    public override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);

        // 만약 부활했을 경우
        if (!context.IsDeath)
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
        context.IsDeath = false;
    }

    private IEnumerator CoDisableAfterDeathAnimation()
    {
        yield return new WaitForSeconds(5.0f);

        var spawnManager = context.GetSpawnManager();
        if (spawnManager != null)
        {
            spawnManager.OnEnemyKilled();
        }
        else
        {
            Debug.LogWarning("SpawnManager 참조가 없습니다.");
        }

        _coDisableObject = null;
       
        PoolManager.Instance.DeactivateObj(context.gameObject);

        DropManager.Instance.DropFromEnemy(context.transform.position, context.IsBoss);
    }
}