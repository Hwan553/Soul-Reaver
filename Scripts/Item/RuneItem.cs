using UnityEngine;

public enum RuneType { Critical, Gold, Defence, Power, Reproduction, Speed }

public class RuneItem : MonoBehaviour
{
    [SerializeField] private RuneBase runeBase;
    private Transform target;
    private bool followPlayer = false;

    [Header("Component")]
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private CapsuleCollider _capsuleCollider;

    [Header("RuneType")]
    [SerializeField] private RuneType runeType;

    public CapsuleCollider CapsuleCollider => _capsuleCollider;
    public Rigidbody Rigidbody => _rigidbody;

    public RuneType RuneType => runeType;

    private void OnEnable()
    {
        Setting();
        Initialize();
        ResourceAllLoad();
    }

    private void Update()
    {
        if (followPlayer && target != null)
        {
            float speed = 30.0f;
            transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        }
    }

    public void Setting()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _capsuleCollider = GetComponent<CapsuleCollider>();

        transform.position = new Vector3(transform.position.x, 1.15f, transform.position.z);
        transform.rotation = Quaternion.Euler(50.0f, 0.0f, 0.0f);
        transform.localScale = Vector3.one * 1.5f;

        _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        _rigidbody.isKinematic = true;

        _capsuleCollider.radius = 0.4f;
        _capsuleCollider.enabled = true;
        _capsuleCollider.isTrigger = false;

        followPlayer = false;
    }

    private void Initialize()
    {
        target = GameObject.FindGameObjectWithTag(Define.Player_Tag)?.transform;
        Invoke(nameof(StartFollowPlayer), 0.5f);
    }

    private void ResourceAllLoad()
    {
        switch (RuneType)
        {
            case RuneType.Critical:
                runeBase = Resources.Load<RuneBase>(Define.Shadows_Path);
                break;
            case RuneType.Defence:
                runeBase = Resources.Load<RuneBase>(Define.Earth_Path);
                break;
            case RuneType.Gold:
                runeBase = Resources.Load<RuneBase>(Define.Fortune_Path);
                break;
            case RuneType.Power:
                runeBase = Resources.Load<RuneBase>(Define.Rage_Path);
                break;
            case RuneType.Reproduction:
                runeBase = Resources.Load<RuneBase>(Define.Life_Path);
                break;
            case RuneType.Speed:
                runeBase = Resources.Load<RuneBase>(Define.Swiftness_Path);
                break;
        }
    }

    private void StartFollowPlayer()
    {
        followPlayer = true;
        _rigidbody.useGravity = false;
        _capsuleCollider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Define.Player_Tag))
        {
            PoolManager.Instance.DeactivateObj(gameObject);

            InventoryManager.Instance.RuneAddItem(runeBase);
        }
    }
}