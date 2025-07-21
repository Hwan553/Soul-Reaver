using UnityEngine;
using UnityEngine.UI;

public class SettingPanel : MonoBehaviour
{
    [Header("Setting List Parent")]
    [SerializeField] private Transform _settingListTransform;

    [Header("Setting Panel")]
    [SerializeField] private GameObject _accountPanel;
    [SerializeField] private GameObject _soundPanel;

    [Header("Setting Button")]
    [SerializeField] private Button _accountButton;
    [SerializeField] private Button _soundButton;

    private void Awake()
    {
        ResourceAllLoad();
        Setting();
    }

    private void OnDisable()
    {
        ResetPanel();
    }

    private void ResourceAllLoad()
    {
        _settingListTransform = transform.Find("Setting List/Viewport/Content");

        _accountPanel = transform.Find("Account Panel").gameObject;
        _soundPanel = transform.Find("Sound Panel").gameObject;

        _accountButton = _settingListTransform.Find("Account Button").GetComponent<Button>();
        _soundButton = _settingListTransform.Find("Sound Button").GetComponent<Button>();
    }

    private void Setting()
    {
        _accountButton.onClick.AddListener(OnAccountButtonClick);
        _soundButton.onClick.AddListener(OnSoundButtonClick);

        ResetPanel();
    }

    private void ResetPanel()
    {
        _accountPanel.SetActive(true);
        _soundPanel.SetActive(false);
    }

    private void OnAccountButtonClick()
    {
        SoundManager.Instance.PlaySFX("Button");

        _accountPanel.SetActive(true);
        _soundPanel.SetActive(false);
    }

    private void OnSoundButtonClick()
    {
        SoundManager.Instance.PlaySFX("Button");

        _accountPanel.SetActive(false);
        _soundPanel.SetActive(true);
    }
}