using BackEnd;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AccountPanel : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private GameObject _couponPanel;
    [SerializeField] private GameObject _successPanel;
    [SerializeField] private GameObject _notFoundPanel;
    [SerializeField] private GameObject _customerSupportPanel;
    [SerializeField] private GameObject _accountDeletePanel;
    [SerializeField] private GameObject _accountDeletionCompletePanel;

    [Header("Image")]
    [SerializeField] private Image _profileImage;

    [Header("Text")]
    [SerializeField] private TMP_Text _nicknameText;
    [SerializeField] private TMP_Text _uUIDText;
    [SerializeField] private TMP_Text _joinDateText;
    [SerializeField] private TMP_Text _totalClearStageText;

    [Header("Text Input Field")]
    [SerializeField] private TMP_InputField _couponInputField;
    [SerializeField] private TMP_InputField _accountDeleteInputField;

    [Header("Coupon Button")]
    [SerializeField] private Button _couponButton;
    [SerializeField] private Button _couponCheckButton;
    [SerializeField] private Button _couponCloseButton;

    [Header("Success Button")]
    [SerializeField] private Button _successCheckButton;

    [Header("Not Found Button")]
    [SerializeField] private Button _notFoundCheckButton;

    [Header("Customer Support Button")]
    [SerializeField] private Button _customerSupportButton;
    [SerializeField] private Button _customerSupportCloseButton;

    [Header("Account Delete Button")]
    [SerializeField] private Button _accountDeleteButton;
    [SerializeField] private Button _accountDeleteCheckButton;
    [SerializeField] private Button _accountDeleteCloseButton;
    [SerializeField] private Button _accountDeletionCompleteCheckButton;

    private void Awake()
    {
        ResourceAllLoad();
        Setting();
    }

    private void Update()
    {
        UpdateDisplay();
    }

    private void OnDisable()
    {
        ResetPanel();
    }

    private void ResourceAllLoad()
    {
        // Panel
        _couponPanel = transform.Find("Coupon Panel").gameObject;
        _successPanel = transform.Find("Success Panel").gameObject;
        _notFoundPanel = transform.Find("Not Found Panel").gameObject;
        _customerSupportPanel = transform.Find("Customer Support Panel").gameObject;
        _accountDeletePanel = transform.Find("Account Delete Panel").gameObject;
        _accountDeletionCompletePanel = transform.Find("Account Deletion Complete Panel").gameObject;

        // Image
        _profileImage = transform.Find("Account Information Panel/Profile Image").GetComponent<Image>();

        // Text
        _nicknameText = transform.Find("Account Information Panel/Nickname Text").GetComponent<TMP_Text>();
        _uUIDText = transform.Find("Account Information Panel/UUID Text").GetComponent<TMP_Text>();
        _joinDateText = transform.Find("Account Information Panel/Join Date Text").GetComponent<TMP_Text>();
        _totalClearStageText = transform.Find("Account Information Panel/Total Clear Stage Text").GetComponent<TMP_Text>();

        // Input Field
        _couponInputField = _couponPanel.transform.Find("Coupon Input Field").GetComponent<TMP_InputField>();
        _accountDeleteInputField = _accountDeletePanel.transform.Find("Account Delete Input Field").GetComponent<TMP_InputField>();

        // Coupon Button
        _couponButton = transform.Find("Account Information Panel/Button/Coupon Button").GetComponent<Button>();
        _couponCloseButton = _couponPanel.transform.Find("Coupon Close Button").GetComponent<Button>();
        _couponCheckButton = _couponPanel.transform.Find("Coupon Check Button").GetComponent<Button>();

        // Success Button
        _successCheckButton = _successPanel.transform.Find("Success Panels/Success Check Button").GetComponent<Button>();

        // Not Found Button
        _notFoundCheckButton = _notFoundPanel.transform.Find("Not Found Panels/Not Found Check Button").GetComponent<Button>();

        // Customer Support Button
        _customerSupportButton = transform.Find("Account Information Panel/Button/Customer Support Button").GetComponent<Button>();
        _customerSupportCloseButton = _customerSupportPanel.transform.Find("Customer Support Close Button").GetComponent<Button>();

        // Account Delete Button
        _accountDeleteButton = transform.Find("Account Information Panel/Button/Account Delete Button").GetComponent<Button>();
        _accountDeleteCloseButton = _accountDeletePanel.transform.Find("Account Delete Close Button").GetComponent<Button>();
        _accountDeleteCheckButton = _accountDeletePanel.transform.Find("Account Delete Check Button").GetComponent<Button>();

        _accountDeletionCompleteCheckButton = _accountDeletionCompletePanel.transform.Find("Account Deletion Complete Check Button").GetComponent<Button>();
    }

    private void Setting()
    {
        ResetPanel();

        BackendReturnObject bro = Backend.BMember.GetUserInfo();

        if (bro.IsSuccess())
        {
            string nickname = bro.GetReturnValuetoJSON()["row"]["nickname"].ToString();
            _nicknameText.text = $"{nickname}";

            string uuid = bro.GetReturnValuetoJSON()["row"]["gamerId"].ToString();
            _uUIDText.text = $"UUID : {uuid}";

            string rawUtc = bro.GetReturnValuetoJSON()["row"]["inDate"].ToString();
            DateTime utcDate = DateTime.Parse(rawUtc, null, System.Globalization.DateTimeStyles.RoundtripKind);
            DateTime kstDate = utcDate.ToLocalTime(); // 시스템 로컬 시간대 (한국 PC 기준이면 KST)

            string joinDate = kstDate.ToString("yyyy-MM-dd");
            _joinDateText.text = $"계정 생성 일자 : {joinDate}";
        }
        else
        {
            Debug.LogError("데이터 가져오기 실패: " + bro.GetMessage());
            _nicknameText.text = "하핳 이름이 없어요!";
            _uUIDText.text = $"UUID : 없어요. 그런거";
            _joinDateText.text = $"계정 생성 일자 : 아, 없다구요.";
        }

        _couponButton.onClick.AddListener(OnCouponButtonClick);
        _couponCheckButton.onClick.AddListener(OnCouponCheckButtonClick);
        _couponCloseButton.onClick.AddListener(OnCouponCloseButtonClick);

        _successCheckButton.onClick.AddListener(OnSuccessCheckButtonClick);

        _notFoundCheckButton.onClick.AddListener(OnNotFoundCheckButtonClick);

        _customerSupportButton.onClick.AddListener(OnCustomerSupportButtonClick);
        _customerSupportCloseButton.onClick.AddListener(OnCustomerSupportCloseButtonClick);

        _accountDeleteButton.onClick.AddListener(OnAccountDeleteButtonClick);
        _accountDeleteCheckButton.onClick.AddListener(OnAccountDeleteCheckButtonClick);
        _accountDeleteCloseButton.onClick.AddListener(OnAccountDeleteCloseButtonClick);

        _accountDeletionCompleteCheckButton.onClick.AddListener(OnAccountDeletionCompleteCheckButtonClick);
    }

    private void ResetPanel()
    {
        _couponInputField.text = "";

        _couponPanel.SetActive(false);
        _successPanel.SetActive(false);
        _notFoundPanel.SetActive(false);
        _customerSupportPanel.SetActive(false);
        _accountDeletePanel.SetActive(false);
        _accountDeletionCompletePanel.SetActive(false);
    }

    private void UpdateDisplay()
    {
        _totalClearStageText.text = $"클리어 스테이지 : {StageManager.Instance.StageWave}";
    }

    private void OnCouponButtonClick()
    {
        SoundManager.Instance.PlaySFX("Button");

        _couponPanel.SetActive(true);
    }

    private void OnCouponCloseButtonClick()
    {
        SoundManager.Instance.PlaySFX("Button");

        _couponInputField.text = "";

        _couponPanel.SetActive(false);
    }

    private void OnCouponCheckButtonClick()
    {
        SoundManager.Instance.PlaySFX("Button");

        string couponCode = _couponInputField.text.Trim();

        BackendCoupon.Instance.ReceiveCoupon(couponCode,
        // 실패 시
        () =>
        {
            _notFoundPanel.SetActive(true);
            _couponInputField.text = "";
        },
        // 성공 시
        () =>
        {
            _successPanel.SetActive(true);
            _couponInputField.text = "";
        });
    }

    private void OnSuccessCheckButtonClick()
    {
        SoundManager.Instance.PlaySFX("Button");

        _successPanel.SetActive(false);
    }

    private void OnNotFoundCheckButtonClick()
    {
        SoundManager.Instance.PlaySFX("Button");

        _notFoundPanel.SetActive(false);
    }

    private void OnCustomerSupportButtonClick()
    {
        SoundManager.Instance.PlaySFX("Button");

        _customerSupportPanel.SetActive(true);
    }

    private void OnCustomerSupportCloseButtonClick()
    {
        SoundManager.Instance.PlaySFX("Button");

        _customerSupportPanel.SetActive(false);
    }

    private void OnAccountDeleteButtonClick()
    {
        SoundManager.Instance.PlaySFX("Button");

        _accountDeletePanel.SetActive(true);
    }

    private void OnAccountDeleteCloseButtonClick()
    {
        SoundManager.Instance.PlaySFX("Button");

        _accountDeletePanel.SetActive(false);
    }

    private void OnAccountDeleteCheckButtonClick()
    {
        SoundManager.Instance.PlaySFX("Button");

        string inputText = _accountDeleteInputField.text;

        if (inputText == "계정 삭제")
        {
            _accountDeletePanel.SetActive(false);
            _accountDeletionCompletePanel.SetActive(true);

            // 7일 뒤에 탈퇴 예약
            Backend.BMember.WithdrawAccount(24 * 0);
            Time.timeScale = 0.0f;
        }
        else
        {
            // 입력이 올바르지 않을 때 사용자에게 안내
            Debug.Log("정확히 '계정 삭제'라고 입력해야 탈퇴할 수 있습니다.");
        }
    }

    private void OnAccountDeletionCompleteCheckButtonClick()
    {
        SoundManager.Instance.PlaySFX("Button");

        _accountDeletionCompletePanel.SetActive(false);

        Time.timeScale = 1.0f;

        SoundManager.Instance.StopBGM();

        SceneManager.LoadScene("GameStart");
    }
}