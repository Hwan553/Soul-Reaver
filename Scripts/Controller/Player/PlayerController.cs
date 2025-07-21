using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Base_Controller, IDamageable
{
    private PlayerStats _player;        // 플레이어의 정보

    private Transform _spriteObject;    // 플레이어 스프라이트 오브젝트

    private Animator _animator;     // 플레이어의 애니메이터 컴포넌트
    private Rigidbody _rigidbody;   // 플레이어의 물리(Rigidbody) 컴포넌트
    private CapsuleCollider _capsuleCollider;       // 플레이어의 Collider 컴포넌트

    [SerializeField] private GameObject _target;    // 가장 가까운 적을 받아올 변수
    [SerializeField] private List<GameObject> _randomTargets;   // 가장 가까운 적을 받아올 변수

    [SerializeField] private float _radius;
    private float _targetDistance;

    private bool _isAttack;         // 공격 상태 여부
    private bool _isDamaged;        // 피격 상태 여부
    private bool _isFacingRight;    // 현재 캐릭터가 오른쪽을 보고 있는지 여부

    // 상태 머신 (State Machine) 객체, 플레이어의 상태를 관리
    protected StateMachine<PlayerController> _stateMachine;

    public StateMachine<PlayerController> StateMachine => _stateMachine;

    public Animator Animator => _animator;
    public Rigidbody Rigidbody => _rigidbody;
    public Transform SpriteObject => _spriteObject;

    public GameObject Target => _target;
    public List<GameObject> RandomTargets => _randomTargets;

    // 타겟까지와의 거리
    public float TargetDistance
    {
        get { return _targetDistance; }
        set { _targetDistance = value; }
    }

    // 플레이어 애니메이션의 이동 상태
    public bool IsMove
    {
        get { return _animator.GetBool(Define.IsMove); }
        set { _animator.SetBool(Define.IsMove, value); }
    }

    // 플레이어 애니메이션의 사망 상태
    public bool IsDeath
    {
        get { return _animator.GetBool(Define.IsDeath); }
        set { _animator.SetBool(Define.IsDeath, value); }
    }

    // 플레이어의 공격 상태
    public bool IsAttack
    {
        get { return _isAttack; }
        set { _isAttack = value; }
    }

    // 플레이어의 피격 상태
    public bool IsDamaged
    {
        get { return _isDamaged; }
        set { _isDamaged = value; }
    }

    // 플레이어가 바라보는 방향 판단
    public bool IsFacingRight
    {
        get { return _isFacingRight; }
        set { _isFacingRight = value; }
    }

    protected override void Initialized()
    {
        Setting();
    }

    private void Start()
    {
        DisableAllBuffs();
    }

    private void FixedUpdate()
    {
        // 물리 업데이트에서 상태 머신 업데이트 수행
        _stateMachine.OnFixedUpdate(Time.fixedDeltaTime);
    }

    private void Update()
    {
        // 매 프레임 상태 머신 업데이트 수행
        _stateMachine.OnUpdate(Time.deltaTime);

        FindPlayerObject();
        FindTargetObject();
        FindRandomTargetObject();
    }

    // 설정 초기화
    private void Setting()
    {
        // Player 정보 가져오기
        _player = GetComponent<PlayerStats>();

        // GameObject의 컴포넌트 가져오기
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody>();
        _capsuleCollider = GetComponent<CapsuleCollider>();

        // 상태 머신 초기화 및 상태 추가
        _stateMachine = new StateMachine<PlayerController>(this, new PlayerIdleState());    // 기본 상태를 Idle로 설정
        _stateMachine.AddState(new PlayerMoveState());      // 이동 상태 추가
        _stateMachine.AddState(new PlayerPursuitState());   // 추적 상태 추가
        _stateMachine.AddState(new PlayerAttackState());    // 공격 상태 추가
        /*_stateMachine.AddState(new PlayerHitState());*/       // 피격 상태 추가
        _stateMachine.AddState(new PlayerDieState());       // 사망 상태 추가

        // Transform 설정
        transform.position = new Vector3(0.0f, 0.15f, 0.0f);
        transform.rotation = Quaternion.Euler(new Vector3(50.0f, 0.0f, 0.0f));
        transform.localScale = new Vector3(3.5f, 3.5f, 3.5f);

        // Rigidbody 설정
        //_rigidbody.constraints = RigidbodyConstraints.FreezeAll;
        _rigidbody.mass = 100.0f;
        _rigidbody.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;

        // Capsule Collider 설정
        _capsuleCollider.center = new Vector3(0.0f, 0.44f, -0.1f);
        _capsuleCollider.radius = 0.4f;
        _capsuleCollider.height = 0.5f;

        _targetDistance = 0.0f;

        _radius = 10.0f;
        _targetDistance = 0.0f;

        _isAttack = false;
        _isDamaged = false;

        // 현재 HP 초기화
        PlayerStats.Instance.CurrentHp = PlayerStats.Instance.MaxHp;
    }

    // 첫 생성 시 보유 버프 비활성화
    private void DisableAllBuffs()
    {
        string[] buffNames = { "Blessing of Regeneration", "Blessing of the Goddess", "Lightning Armament" };

        foreach (var buffName in buffNames)
        {
            Transform buffTransform = transform.Find($"Buff/{buffName}");

            if (buffTransform != null)
            {
                buffTransform.gameObject.SetActive(false);
            }
        }
    }

    // 플레이어 오브젝트 탐색
    private void FindPlayerObject()
    {
        if (_spriteObject != null) return;

        var player = FindAnyObjectByType<PlayerController>();

        if (player != null)
        {
            _spriteObject = player.transform.Find("Sprite")?.transform;

            // 캐릭터의 현재 방향 (Local Scale 기준)으로 _isFacingRight 설정
            _isFacingRight = _spriteObject.localScale.x < 0.0f; ;
        }
    }

    // 가장 가까운 적 탐색 및 거리 계산
    private void FindTargetObject()
    {
        _target = gameObject.GetNearestTarget(Define.Enemy_Layer, _radius);

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

    // 범위 내 적 랜덤 지정
    private void FindRandomTargetObject()
    {
        _randomTargets = gameObject.GetRandomTarget(Define.Enemy_Layer, _radius);
    }


    // 스프라이트 반전 처리
    private void SetFacing(bool shouldFaceRight)
    {
        if (IsFacingRight == shouldFaceRight) return;

        IsFacingRight = shouldFaceRight;

        Vector3 scale = SpriteObject.localScale;
        scale.x = Mathf.Abs(scale.x) * (shouldFaceRight ? -1 : 1);
        SpriteObject.localScale = scale;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _radius);
    }

    private void OnTriggerEnter(Collider other)
    {
        // 마법구체(MagicProjectile)와 충돌
        if (other.CompareTag(Define.MagicProjectile_Tag))
        {
            MagicProjectile magic = other.GetComponent<MagicProjectile>();

            TakeDamage(magic.Damage, magic.transform);
        }
        // 화살(ArrowProjectile)과 충돌
        else if (other.CompareTag(Define.ArrowProjectile_Tag))
        {
            ArrowProjectile arrow = other.GetComponent<ArrowProjectile>();

            TakeDamage(arrow.Damage, arrow.transform);
        }
    }

    public void PerformAttackDamage()
    {
     
        // 1) 현재 타겟이 없거나, 범위 바깥이면 아무것도 하지 않음
        if (_target == null)
            return;

        // 2) 타겟과의 거리를 재확인
        float dist = Vector3.Distance(
            new Vector3(_target.transform.position.x, 0f, _target.transform.position.z),
            new Vector3(transform.position.x, 0f, transform.position.z)
        );
        if (dist > _radius)
            return;

        // 3) IDamageable을 구현한 적에게 데미지 전달
        if (_target.TryGetComponent<IDamageable>(out var enemy))
        {
            // 3-1) 치명타 여부 판정
            bool isCritical = Random.value < (PlayerStats.Instance.CritChance / 100f);
            float baseDamage = PlayerStats.Instance.Attack;
            float finalDamage = baseDamage;

            // 3-2) 치명타일 경우 최종 데미지 계산
            if (isCritical)
            {
                finalDamage = baseDamage * (1 + PlayerStats.Instance.CritDamage / 100f);
            }
            else
            {
                finalDamage = baseDamage;
            }

            // 3-3) 적의 TakeDamage 함수에 최종 데미지와 치명타 여부를 함께 전달
            enemy.TakeDamage(finalDamage, transform, isCritical);
        }
    }

    // 자동 이동 시 반전 처리
    public void UpdateFacingByTarget()
    {
        if (_target == null) return;

        bool shouldFaceRight = _target.transform.position.x > transform.position.x;
        SetFacing(shouldFaceRight);
    }

    // 수동 이동 시 반전 처리
    public void UpdateFacingByInput(Vector3 moveDirection)
    {
        if (moveDirection.x == 0) return;

        bool shouldFaceRight = moveDirection.x > 0;
        SetFacing(shouldFaceRight);
    }

    // 피격 시 호출
    public void TakeDamage(float damage, Transform attacker = null, bool isCritical = false)
    {
        if (IsDamaged || IsDeath) return;

        // 방어력 적용: 최소 1 데미지 보장
        float damageAfterDef = damage - PlayerStats.Instance.Defense;
        damageAfterDef = Mathf.Max(damageAfterDef, 1f);

        // 체력 감소
        PlayerStats.Instance.CurrentHp -= damageAfterDef;
        PlayerStats.Instance.CurrentHp = Mathf.Max(PlayerStats.Instance.CurrentHp, 0f);

        if (PlayerStats.Instance.CurrentHp <= 0 && !IsDeath)
        {
            StateMachine.ChangeState<PlayerDieState>();
        }
        _isDamaged = true;
    }

    // 플레이어 리스폰
    public void ReSpawnPlayer()
    {
        IsDeath = false;
        Setting();
        PlayerStats.Instance.CurrentHp = PlayerStats.Instance.MaxHp;
        PlayerStats.Instance.CurrentMp = PlayerStats.Instance.MaxMp;

        StateMachine.ChangeState<PlayerIdleState>();
    }
}