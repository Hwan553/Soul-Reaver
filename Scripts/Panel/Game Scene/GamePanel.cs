using UnityEngine;
using TMPro;

public class GamePanel : MonoBehaviour
{
    [SerializeField] private TMP_Text _goldText;
    [SerializeField] private TMP_Text _gpText;

    private void Start()
    {
        Setting();
    }

    private void Update()
    {
        UpdateDisplay();
    }

    private void Setting()
    {
        _goldText = transform.Find("Gold Panel/Gold Image").transform.Find("Gold Text").GetComponent<TMP_Text>();
        _gpText = transform.Find("Gp Panel/Gp Image").transform.Find("Gp Text").GetComponent<TMP_Text>();
    }

    private void UpdateDisplay()
    {
        _goldText.text = $"{GameManager.Instance.GoldNum.ToString()}";
        _gpText.text = $"{GameManager.Instance.GpNum.ToString()}";
    }
}