using BackEnd;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MailboxPanel : MonoBehaviour
{
    [Header("Mail Content Parent")]
    [SerializeField] private GameObject _mailContentObject;

    [Header("Scroll View Content Parent")]
    [SerializeField] private Transform _mailListTransform;
    [SerializeField] private Transform _mailRewardTransform;

    [Header("Prefab")]
    [SerializeField] private GameObject _mailPrefab;
    [SerializeField] private GameObject _rewardPrefab;

    [Header("List")]
    [SerializeField] private List<GameObject> _mailLists;
    [SerializeField] private List<GameObject> _rewardLists;

    [Header("Mail Content")]
    [SerializeField] private TMP_Text _mailTitleText;
    [SerializeField] private TMP_Text _mailSenderText;
    [SerializeField] private TMP_Text _mailSentDateText;
    [SerializeField] private TMP_Text _mailRecipientText;
    [SerializeField] private TMP_Text _mailContentText;

    [Header("Button")]
    [SerializeField] private Button _refreshButton;
    [SerializeField] private Button _receiveButton;

    [Header("Select Mail Data")]
    [SerializeField] private PostType _selectedMailType;
    [SerializeField] private int _selectedMailIndex;

    private void OnEnable()
    {
        LoadMails();
    }

    private void Awake()
    {
        ResourceAllLoad();

        Setting();
    }

    private void ResourceAllLoad()
    {
        _mailContentObject = transform.Find("Mail Content").gameObject;

        _mailListTransform = transform.Find("Mail List/Viewport/Content").gameObject.transform;
        _mailRewardTransform = _mailContentObject.transform.Find("Rewards/Viewport/Content").gameObject.transform;

        _mailPrefab = Resources.Load<GameObject>(Define.MailPrefab_Path);
        _rewardPrefab = Resources.Load<GameObject>(Define.RewardPrefab_Path);

        _mailTitleText = _mailContentObject.transform.Find("Title Text").GetComponent<TMP_Text>();
        _mailSenderText = _mailContentObject.transform.Find("Sender Text").GetComponent<TMP_Text>();
        _mailSentDateText = _mailContentObject.transform.Find("Sent Date Text").GetComponent<TMP_Text>();
        _mailRecipientText = _mailContentObject.transform.Find("Recipient Text").GetComponent<TMP_Text>();
        _mailContentText = _mailContentObject.transform.Find("Content Text").GetComponent<TMP_Text>();

        _refreshButton = transform.Find("Refresh Button").GetComponent<Button>();
        _receiveButton = _mailContentObject.transform.Find("Receive Button").GetComponent<Button>();
    }

    private void Setting()
    {
        _mailLists = new List<GameObject>();
        _rewardLists = new List<GameObject>();

        _refreshButton.onClick.AddListener(OnRefreshButtonClick);
        _receiveButton.onClick.AddListener(OnReceiveButtonClick);
    }

    // 메일 상세 내용 초기화
    private void ResetContent()
    {
        _mailTitleText.text = string.Empty;
        _mailSenderText.text = string.Empty;
        _mailSentDateText.text = string.Empty;
        _mailRecipientText.text = string.Empty;
        _mailContentText.text = string.Empty;

        // 기존 보상 오브젝트 제거
        foreach (var reward in _rewardLists)
        {
            Destroy(reward);
        }
        _rewardLists.Clear();
    }

    // 메일 UI 불러오기
    private void LoadMails()
    {
        // 기존 메일 오브젝트 정리
        foreach (var mail in _mailLists)
        {
            Destroy(mail);
        }
        _mailLists.Clear();
        ResetContent();

        // MailList를 받아오고 나서 UI 갱신
        BackendMail.Instance.MailListGet(PostType.Admin);

        List<Mail> posts = BackendMail.Instance.GetMailList();  // _mailList 접근용 메서드 필요

        for (int i = 0; i < posts.Count; i++)
        {
            var post = posts[i];

            GameObject mailObject = Instantiate(_mailPrefab, _mailListTransform);
            mailObject.name = $"Mail List {i}";

            var titleText = mailObject.transform.Find("Title Text").GetComponent<TMP_Text>();
            var senderText = mailObject.transform.Find("Sender Text").GetComponent<TMP_Text>();
            var expirationDateText = mailObject.transform.Find("Expiration Date Text").GetComponent<TMP_Text>();

            titleText.text = post.Title;
            senderText.text = post.Author;
            expirationDateText.text = post.InDate;  // 만료일이 없다면 InDate로 대체

            int index = i; // 클로저 캡처 방지

            mailObject.GetComponent<Button>().onClick.AddListener(() =>
            {
                SoundManager.Instance.PlaySFX("Button");

                _selectedMailType = PostType.Admin; // 현재는 Admin으로 고정이므로 그대로 사용
                _selectedMailIndex = index;

                DisplayMailDetail(post);
            });

            _mailLists.Add(mailObject);
        }
    }

    // 선택한 메일 정보 출력
    private void DisplayMailDetail(Mail mail)
    {
        _mailTitleText.text = $"제목 : {mail.Title}";
        _mailSenderText.text = mail.Author;
        _mailSentDateText.text = $"보낸 날짜 : {mail.InDate}";
        _mailRecipientText.text = $"받는 사람 : {Backend.UserNickName}";
        _mailContentText.text = mail.Content;

        // 기존 보상 오브젝트 제거
        foreach (var reward in _rewardLists)
        {
            Destroy(reward);
        }
        _rewardLists.Clear();

        // 새로운 보상 오브젝트 생성
        foreach (var rewardPair in mail.mailReward)
        {
            string itemName = rewardPair.Key;
            int itemCount = rewardPair.Value;

            GameObject rewardObject = Instantiate(_rewardPrefab, _mailRewardTransform);
            rewardObject.name = $"Reward_{itemName}";

            // 아이콘 및 수량 텍스트 설정
            Image iconImage = rewardObject.transform.Find("Icon Image").GetComponent<Image>();
            TMP_Text amountText = rewardObject.transform.Find("Amount Text").GetComponent<TMP_Text>();

            // 아이콘 스프라이트 불러오기 (아이템 이름 기준으로 Resources 폴더에 있다고 가정)
            Sprite icon = Resources.Load<Sprite>($"Sprites/Icons/{itemName}");

            if (icon != null)
            {
                iconImage.sprite = icon;
            }
            else
            {
                Debug.LogWarning($"아이콘 이미지가 없습니다: {itemName}");
            }

            amountText.text = $"{itemCount}";

            _rewardLists.Add(rewardObject);
        }
    }

    private void OnRefreshButtonClick()
    {
        SoundManager.Instance.PlaySFX("Button");

        LoadMails();
    }

    private void OnReceiveButtonClick()
    {
        SoundManager.Instance.PlaySFX("Button");
        BackendMail.Instance.MailReceive(_selectedMailType, _selectedMailIndex);

        LoadMails();
    }
}