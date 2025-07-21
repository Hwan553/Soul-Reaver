using UnityEngine;

public class BossController : Base_Controller, IDamageable
{
    [Header("Type")]
    [SerializeField] protected ClassType ClassType;    // 적의 직업 타입
    [SerializeField] protected TribeType TribeType;    // 적의 종족 타입

    [Header("Stats")]
    [SerializeField] private EnemyStats _enemyClassStats;   // 적의 클래스 별 능력치
    [SerializeField] private EnemyStats _runtimeStats;      // 복사해서 사용하는 런타임 전용 Stats

    [Header("Component")]
    [SerializeField] private Animator _animator;     // 적의 애니메이터 컴포넌트
    [SerializeField] private Rigidbody _rigidbody;   // 적의 물리(Rigidbody) 컴포넌트
    [SerializeField] private CapsuleCollider _capsuleCollider;       // 적의 Collider 컴포넌트

    [Header("Sprite")]
    [SerializeField] private Transform _spriteObject;    // 적 스프라이트 오브젝트

    [Header("Target")]
    [SerializeField] private GameObject _target;        // 타겟을 받아올 변수
    [SerializeField] private LayerMask _targetLayer;    // 타겟 레이어를 저장할 변수
    [SerializeField] private float _targetDistance;     // 타겟과의 거리를 나타낼 변수

    [Header("State")]
    [SerializeField] private bool _isAttack;         // 공격 상태 여부
    [SerializeField] private bool _isDamaged;        // 피격 상태 여부
    [SerializeField] private bool _isFacingRight;    // 현재 캐릭터가 오른쪽을 보고 있는지 여부

    [Header("SpawnManager")]
    private SpawnManager _spawnManager;

    [Header("Enemy Type")]
    [SerializeField] private bool isBoss = true;
    public bool IsBoss => isBoss;

    // 상태 머신 (State Machine) 객체, 적의 상태를 관리
    protected StateMachine<BossController> _stateMachine;

    public StateMachine<BossController> StateMachine => _stateMachine;

    public CapsuleCollider CapsuleCollider => _capsuleCollider;

    // Enemy 관련 프로퍼티
    public ClassType EnemyClassType => ClassType;
    public EnemyStats RuntimeStats => _runtimeStats;

    // 컴포넌트 프로퍼티
    public Animator Animator => _animator;
    public Rigidbody Rigidbody => _rigidbody;

    // 스프라이트 오브젝트 프로퍼티
    public Transform SpriteObject => _spriteObject;

    // 타겟 프로퍼티
    public GameObject Target => _target;
    public LayerMask TargetLayer => _targetLayer;

    // 타겟까지와의 거리
    public float TargetDistance
    {
        get { return _targetDistance; }
        set { _targetDistance = value; }
    }

    // 적 애니메이션의 이동 상태
    public bool IsMove
    {
        get { return _animator.GetBool(Define.IsMove); }
        set { _animator.SetBool(Define.IsMove, value); }
    }

    // 적 애니메이션의 사망 상태
    public bool IsDeath
    {
        get { return _animator.GetBool(Define.IsDeath); }
        set { _animator.SetBool(Define.IsDeath, value); }
    }

    // 적의 공격 상태
    public bool IsAttack
    {
        get { return _isAttack; }
        set { _isAttack = value; }
    }

    // 적의 피격 상태
    public bool IsDamaged
    {
        get { return _isDamaged; }
        set { _isDamaged = value; }
    }

    // 적이 바라보는 방향 판단
    public bool IsFacingRight
    {
        get { return _isFacingRight; }
        set { _isFacingRight = value; }
    }

    protected override void Initialized() { }

    // 설정 초기화
    protected virtual void Setting()
    {
        // GameObject의 컴포넌트 가져오기
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody>();
        _capsuleCollider = GetComponent<CapsuleCollider>();

        // 상태 머신 초기화 및 상태 추가
        _stateMachine = new StateMachine<BossController>(this, new BossIdleState());    // 기본 상태를 Idle로 설정
        _stateMachine.AddState(new BossPursuitState());   // 추적 상태 추가
        _stateMachine.AddState(new BossAttackState());    // 공격 상태 추가
        _stateMachine.AddState(new BossHitState());       // 피격 상태 추가
        _stateMachine.AddState(new BossDieState());       // 사망 상태 추가

        // Transform 설정
        transform.position = new Vector3(transform.position.x, 0.15f, transform.position.z);
        transform.rotation = Quaternion.Euler(new Vector3(50.0f, 0.0f, 0.0f));
        transform.localScale = new Vector3(7.0f, 7.0f, 7.0f);

        // Rigidbody 설정
        _rigidbody.useGravity = false;
        _rigidbody.isKinematic = true;
        _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;


        // Capsule Collider 설정
        _capsuleCollider.center = new Vector3(0.0f, 0.33f, 0.0f);
        _capsuleCollider.radius = 0.3f;
        _capsuleCollider.height = 1f;
        _capsuleCollider.enabled = true;

        _targetLayer = LayerMask.GetMask(Define.Player_Layer);

        _isAttack = false;
        _isDamaged = false;
        _isFacingRight = false;

        DisableReaperCurse();
    }

    protected virtual void ResourceAllLoad()
    {
        switch (ClassType)
        {
            case ClassType.Boss:
                _enemyClassStats = Resources.Load<EnemyStats>(Define.Boss_Stat_Scriptable_Path);
                break;
        }
    }

    // 적 스프라이트 오브젝트 탐색
    protected virtual void FindEnemyObject()
    {
        if (_spriteObject != null) return;

        if (this.gameObject.activeSelf)
        {
            _spriteObject = this.gameObject.transform.Find("Sprite")?.transform;
        }
    }

    // 가장 가까운 적 탐색 및 거리 계산
    protected virtual void FindTargetObject()
    {
        _target = FindAnyObjectByType<PlayerController>().gameObject;

        if (_target != null)
        {
            _targetDistance = Vector3.Distance(new Vector3(_target.transform.position.x, 0.0f, _target.transform.position.z),
                                               new Vector3(transform.position.x, 0.0f, transform.position.z));
        }
        else
        {
            _targetDistance = float.MaxValue;
        }
    }

    public void InitializeStats(int wave)
    {
        _runtimeStats = _enemyClassStats.Clone();
        _runtimeStats.Health *= Mathf.Pow(1.2f, wave);
        _runtimeStats.AttackDamage *= Mathf.Pow(1.1f, wave);
    }

    private void DisableReaperCurse()
    {
        // Enemy 내부의 Debuff 오브젝트 하위에 있는 Curse of the Reaper 오브젝트 찾기
        Transform curseTransform = transform.Find("Debuff/Curse of the Reaper");

        if (curseTransform != null)
        {
            curseTransform.gameObject.SetActive(false);
        }
    }


    public void SetSpawnManager(SpawnManager manager)
    {
        _spawnManager = manager;
    }

    public SpawnManager GetSpawnManager()
    {
        return _spawnManager;
    }

    public void TakeDamage(float damage, Transform attacker = null, bool isCritical = false)
    {
        if (IsDeath) return;

        _runtimeStats.Health -= damage;

        DamageTextManager.Instance.ShowDamageText(damage, this.gameObject, isCritical);

        if (_runtimeStats.Health <= 0)
        {
            IsDeath = true;
        }
        else
        {
            _isDamaged = true;
        }
    }
}