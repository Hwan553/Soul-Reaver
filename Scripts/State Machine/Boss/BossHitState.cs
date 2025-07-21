using System.Collections;
using UnityEngine;

public class BossHitState : State<BossController>
{
    private Coroutine _coHitAnimDelay;

    public override void OnInitialized()
    {
        _coHitAnimDelay = null;
    }

    public override void OnEnter()
    {
        base.OnEnter();

        Debug.Log($"<color=red>{context.name} State : {this.GetType().Name}</color>");

        // 애니메이션 파라미터 값 초기화
        context.IsMove = false;
        context.IsAttack = false;
        context.IsDamaged = true;
        context.IsDeath = false;

        // 피격 모션 트리거 실행
        context.Animator.SetTrigger(Define.Damaged);

        if (_coHitAnimDelay != null)
            return;

        if (_coHitAnimDelay == null)
        {
            _coHitAnimDelay = context.StartCoroutine(CoHitDelay());
        }
    }

    public override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);
    }

    public override void OnExit()
    {
        base.OnExit();

        if (_coHitAnimDelay != null)
        {
            context.StopCoroutine(_coHitAnimDelay);
            _coHitAnimDelay = null;
        }

        // 애니메이션 파라미터 값 초기화
        context.IsDamaged = false;
    }

    // 피격 후 애니메이션 길이만큼 딜레이
    private IEnumerator CoHitDelay()
    {
        yield return null;  // 애니메이션 진입 대기 1프레임

        AnimatorStateInfo stateInfo = context.Animator.GetCurrentAnimatorStateInfo(0);
        float hitAnimLength = stateInfo.length;

        yield return new WaitForSeconds(hitAnimLength);
        context.IsDamaged = false;
        _coHitAnimDelay = null;

        // 상태 전이 처리
        if (context.IsDeath)
        {
            stateMachine.ChangeState<BossDieState>();
        }
        else
        {
            stateMachine.ChangeState<BossIdleState>();
        }
    }
}