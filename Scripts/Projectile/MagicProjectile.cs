using UnityEngine;

public class MagicProjectile : Base_Projectile
{
    private CapsuleCollider _capsuleCollider;

    [SerializeField] private float _damage;

    public float Damage => _damage;

    private void Start()
    {
        MagicSetting();
    }

    private void OnEnable()
    {
        AutoDeactivate(this.gameObject);
    }

    private void FixedUpdate()
    {
        ProjectileMove();
    }

    private void MagicSetting()
    {
        base.Setting();

        _capsuleCollider = GetComponent<CapsuleCollider>();

        // Capsule Collider 기본 설정
        _capsuleCollider.isTrigger = true;
        _capsuleCollider.center = new Vector3(0.0f, 0.03f, 0.0f);
        _capsuleCollider.radius = 0.1f;
        _capsuleCollider.height = 0.25f;
    }

    public void SetDirectionWithAtan2(Vector3 targetPosition)
    {
        Vector3 direction = targetPosition - transform.position;
        direction.y = 0.0f; // 2.5D 느낌에서 수직축 제거
        MoveDirection = direction.normalized;

        float angle = Mathf.Atan2(MoveDirection.x, MoveDirection.z) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(90.0f, angle, 0.0f);

        transform.localScale = new Vector3(7.0f, 7.0f, 7.0f);   // 너비, 높이, 깊이 조정
    }

    // 외부에서 발사체의 대미지를 설정
    public void SetMagicDamage(float damage)
    {
        _damage = damage;
    }

    // 외부에서 발사체의 속도를 설정
    public void SetMagicSpeed(float speed)
    {
        base.SetSpeed(speed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Define.Player_Tag))
        {
            if (other.TryGetComponent<IDamageable>(out var damageable))
            {
                damageable.TakeDamage(Damage, transform);
            }
            PoolManager.Instance.DeactivateObj(gameObject);
            return;
        }
    }
}