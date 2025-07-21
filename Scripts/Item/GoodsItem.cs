using UnityEngine;
using System.Collections;

public enum GoodsType { Gold, Gp }

public class GoodsItem : MonoBehaviour
{
    private static Transform cachedPlayer;

    private Transform target;
    private int amount;
    private bool _collected;
    private Coroutine followCoroutine;

    [Header("Component")]
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private CapsuleCollider _capsuleCollider;
    private Renderer _renderer; // 렌더러 캐싱

    [Header("Type Info")]
    [SerializeField] private GoodsType goodsType;

    [Header("Follow Settings")]
    [SerializeField] private float followDelay = 0.5f;
    [SerializeField] private float baseSpeed = 12.0f;
    [SerializeField] private float maxSpeed = 25.0f;
    [SerializeField] private float collectionRange = 0.6f;
    [SerializeField] private float maxFollowDistance = 30f;

    private void OnEnable()
    {
        InitializeComponents();
        followCoroutine = StartCoroutine(FollowToPlayer());
    }

    private void OnDisable()
    {
        if (followCoroutine != null)
        {
            StopCoroutine(followCoroutine);
            followCoroutine = null;
        }
    }

    private void InitializeComponents()
    {
        if (_rigidbody == null) _rigidbody = GetComponent<Rigidbody>();
        if (_capsuleCollider == null) _capsuleCollider = GetComponent<CapsuleCollider>();
        if (_renderer == null) _renderer = GetComponentInChildren<Renderer>(); // 자식에 있을 수도 있음

        // Transform 초기화
        transform.position = new Vector3(transform.position.x, 0.8f, transform.position.z);
        transform.rotation = Quaternion.Euler(50f, 0f, 0f);
        transform.localScale = Vector3.one * 3.5f;

        // 물리 제거
        _rigidbody.isKinematic = true;
        _rigidbody.useGravity = false;
        _capsuleCollider.enabled = false;

        // 렌더러 다시 켜기
        if (_renderer != null)
            _renderer.enabled = true;

        _collected = false;

        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Gold"), LayerMask.NameToLayer(Define.Player_Layer), true);
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Diamond"), LayerMask.NameToLayer(Define.Player_Layer), true);
    }

    public void Initialize(int value, GoodsType type)
    {
        amount = value;
        goodsType = type;

        if (cachedPlayer == null)
            cachedPlayer = GameObject.FindGameObjectWithTag("Player")?.transform;

        target = cachedPlayer;
    }

    private IEnumerator FollowToPlayer()
    {
        yield return new WaitForSeconds(followDelay + Random.Range(0f, 0.1f));

        while (!_collected && target != null)
        {
            Vector3 targetPos = target.position + Vector3.up * 0.5f;
            float distance = Vector3.Distance(transform.position, targetPos);

            if (distance > maxFollowDistance)
            {
                PoolManager.Instance.DeactivateObj(gameObject);
                yield break;
            }

            if (distance < collectionRange)
            {
                OnCollected();
                yield break;
            }

            float speed = Mathf.Lerp(baseSpeed, maxSpeed, Mathf.Clamp01(distance / 5f));
            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

            yield return null;
        }
    }

    private void OnCollected()
    {
        if (_collected) return;

        _collected = true;

        // 렌더러 비활성화
        if (_renderer != null)
            _renderer.enabled = false;

        // 수집 큐 등록
        GoodsCollectorManager.Instance.RequestCollect(this);
    }

    public void DoCollect()
    {
        int finalAmount = amount;

        if (goodsType == GoodsType.Gold)
        {
            float goldBonus = PlayerStats.Instance.GoldGain;
            finalAmount = Mathf.RoundToInt(amount * (1.0f + goldBonus));
            GameManager.Instance.GetMoney(finalAmount);
        }
        else if (goodsType == GoodsType.Gp)
        {
            GameManager.Instance.GetGp(amount);
        }

        PoolManager.Instance.DeactivateObj(gameObject);
        transform.localScale = Vector3.one * 3.5f;
    }
}
