using UnityEngine;

public class DamageTextManager : Singleton<DamageTextManager>
{
    [SerializeField] private GameObject _damageTextPrefab;
    [SerializeField] private Transform _canvasTransform;

    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color criticalColor = Color.yellow;

    // Awake() 함수는 기존과 동일합니다.
    private void Awake()
    {
        if (_damageTextPrefab == null)
            _damageTextPrefab = Resources.Load<GameObject>(Define.DamageText_Path);

        if (_canvasTransform == null)
        {
            GameObject canvasObj = GameObject.Find("DamageText"); // 캔버스 이름이 "DamageText"라고 가정
            if (canvasObj != null)
                _canvasTransform = canvasObj.transform;
            else
                Debug.LogError("DamageText Canvas not found in scene!");
        }
    }

    public void ShowDamageText(float damage, GameObject targetObj, bool isCritical)
    {
        GameObject obj = PoolManager.Instance.ActivateTextObj(_damageTextPrefab, _canvasTransform);

        var dmg = obj.GetComponent<DamageText>();

        if (dmg != null)
        {
            Color color = isCritical ? criticalColor : normalColor;
            dmg.Show(damage, color, targetObj.transform.position);
        }
    }
}