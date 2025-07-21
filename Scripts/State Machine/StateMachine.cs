using System.Collections.Generic;
using UnityEngine;

// 상태 클래스 : 상태 머신에서 사용할 개별 상태를 정의하는 추상 클래스
public abstract class State<T>
{
    protected StateMachine<T> stateMachine; // 상태 머신 참조
    protected T context;                    // 현재 상태에서 사용할 컨텍스트 객체
    protected int mecanimStateHash;         // 애니메이터 상태 해시값

    // 기본 생성자
    public State() { }
    // 애니메이터 상태 해시를 설정하는 생성자
    public State(int mecanimStateHash)
    {
        this.mecanimStateHash = mecanimStateHash;
    }

    // 애니메이터 상태 이름을 해서로 변환하여 설정하는 생성자
    public State(string mecanimStateName) : this(Animator.StringToHash(mecanimStateName)) { }

    // 상태 머신과 컨텍스트를 설정하는 메서드
    public void SetMachine(StateMachine<T> stateMachine, T context)
    {
        this.stateMachine = stateMachine;
        this.context = context;

        OnInitialized();    // 상태 초기화 호출
    }

    // 상태 초기화 시 실행될 메서드 (추상 메서드, 반드시 구현 필요)
    public abstract void OnInitialized();
    // 상태 진입 시 실행될 가상 메서드 (필요 시 오버라이드 가능)
    public virtual void OnEnter() { }
    // 상태 갱신 시 실행될 가상 메서드 (프레임마다 실행)
    public virtual void OnUpdate(float deltaTime) { }
    // 물리 업데이트 시 실행될 가상 메서드 (FixedUpdate에서 실행)
    public virtual void OnFixedUpdate(float fixedDeltaTime) { }
    // 상태 종료 시 실행될 가상 메서드
    public virtual void OnExit() { }
}

// 상태 머신 클래스 : 상태를 관리하는 상태 머신
public class StateMachine<T>
{
    // 등록된 상태 목록을 저장하는 딕셔너리
    private Dictionary<System.Type, State<T>> states = new Dictionary<System.Type, State<T>>();

    private State<T> _currentState;     // 현재 상태
    private State<T> _previousState;    // 이전 상태

    private T _context; // 상태 머신이 관리하는 컨텍스트 객체

    private float _elapsedTime;  // 현재 상태에서 경과한 시간

    public State<T> CurrentState => _currentState;      // 현재 상태 반환
    public State<T> PreviousState => _previousState;    // 이전 상태 반환

    public event System.Action OnChangedState;          // 상태 변경 이벤트

    // 상태 머신 생성자
    public StateMachine(T context, State<T> state)
    {
        this._context = context;

        AddState(state);    // 초기 상태 추가

        _currentState = state;      // 초기 상태 설정
        _currentState.OnEnter();    // 초기 상태 진입 처리
    }

    // 새로운 상태를 추가하는 메서드
    public void AddState(State<T> state)
    {
        state.SetMachine(this, _context);   // 상태에 상태 머신과 컨텍스트 설정
        states[state.GetType()] = state;    // 상태 목록에 추가
    }

    // 매 프레임 상태 갱신
    public void OnUpdate(float deltaTime)
    {
        _elapsedTime += deltaTime;          // 상태에서 경과한 시간 업데이트
        _currentState.OnUpdate(deltaTime);  // 현재 상태의 업데이트 로직 실행
    }
    
    // 물리 연산 업데이트 (FixedUpdate에서 호출)
    public void OnFixedUpdate(float fixedDeltaTime)
    {
        _currentState.OnFixedUpdate(fixedDeltaTime);    // 현재 상태의 물리 업데이트 실행
    }

    // 상태 변경 메서드
    public R ChangeState<R>() where R : State<T>
    {
        var newType = typeof(R);    // 변경할 상태 타입 가져오기

        // 현재 상태와 변경할 상태가 같으면 변경하지 않음
        if (_currentState.GetType() == newType)
        {
            return _currentState as R;
        }

        // 현재 상태 종료 처리
        if (_currentState != null)
        {
            _currentState.OnExit();
        }

        _previousState = _currentState;     // 이전 상태 저장
        _currentState = states[newType];    // 새로운 상태 설정
        _currentState.OnEnter();            // 새로운 상태 진입 처리
        _elapsedTime = 0.0f;                // 상태 전환 후 시간 초기화

        OnChangedState?.Invoke();   // 상태 변경 이벤트 실행

        return _currentState as R;  // 변경된 상태 반환
    }

    // 현재 상태 리로드 메서드
    public R ReloadState<R>() where R : State<T>
    {
        var newType = typeof(R);    // 변경할 상태 타입 가져오기

        // 현재 상태 종료 처리
        if (_currentState != null)
        {
            _currentState.OnExit();
        }

        _previousState = _currentState;     // 이전 상태 저장
        _currentState = states[newType];    // 새로운 상태 설정
        _currentState.OnEnter();            // 새로운 상태 진입 처리
        _elapsedTime = 0.0f;                // 상태 전환 후 시간 초기화

        OnChangedState?.Invoke();   // 상태 변경 이벤트 실행

        return _currentState as R;  // 변경된 상태 반환
    }

}