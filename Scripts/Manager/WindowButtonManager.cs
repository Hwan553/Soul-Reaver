using UnityEngine;
using UnityEngine.UI;

public class WindowButtonManager : MonoBehaviour
{
    private GameObject _currentOpenedWindow = null;

    public PopUp PopUp;
    public PopDown PopDown;

    [Header("Window List")]
    public GameObject CharacterObj;
    public GameObject InventoryObj;
    public GameObject SkillWindowPanel;
    public GameObject SyntheticObj;
    public GameObject DrawingObj;
    public GameObject CollectionObj;
    public GameObject DrawInforObj;
    public GameObject MailboxPanel;
    public GameObject RankingPanel;
    public GameObject NoticePanel;
    public GameObject SettingPanel;
    public GameObject FriendPanel;

    [Header("Window List Open Button")]
    public Button CharacterObjOpenButton;
    public Button InventoryObjOpenButton;
    public Button SkillWindowPanelOpenButton;
    public Button SyntheticObjOpenButton;
    public Button DrawingObjOpenButton;
    public Button CollectionObjOpenButton;
    public Button MailboxPanelOpenButton;
    public Button RankingPanelOpenButton;
    public Button NoticePanelOpenButton;
    public Button SettingPanelOpenButton;
    public Button FriendPanelOpenButton;

    [Header("WIndow List Close Button")]
    public Button CharacterObjCloseButton;
    public Button InventoryObjCloseButton;
    public Button SkillWindowPanelCloseButton;
    public Button SyntheticObjCloseButton;
    public Button DrawingObjCloseButton;
    public Button CollectionObjCloseButton;
    public Button MailboxPanelCloseButton;
    public Button RankingPanelCloseButton;
    public Button NoticePanelCloseButton;
    public Button SettingPanelCloseButton;
    public Button FriendPanelCloseButton;

    [Header("DrawWindow")]
    public Button DrawInforOpenButton;
    public Button DrawInforcloseButton;

    private void Start()
    {
        // 창 열기 버튼
        CharacterObjOpenButton.onClick.AddListener(OpenWindowCharacter);
        CollectionObjOpenButton.onClick.AddListener(OpenWindowCollect);
        SkillWindowPanelOpenButton.onClick.AddListener(OpenWindowSkill);
        InventoryObjOpenButton.onClick.AddListener(OpenWindowInventory);
        SyntheticObjOpenButton.onClick.AddListener(OpenWindowSynthetic);
        DrawingObjOpenButton.onClick.AddListener(OpenWindowDrawing);
        DrawInforOpenButton.onClick.AddListener(OpenDrawWindowDrawInfor);
        MailboxPanelOpenButton.onClick.AddListener(OpenMailPanel);
        RankingPanelOpenButton.onClick.AddListener(OpenRankingPanel);
        NoticePanelOpenButton.onClick.AddListener(OpenNoticePanel);
        SettingPanelOpenButton.onClick.AddListener(OpenSettingPanel);
        FriendPanelOpenButton.onClick.AddListener(OpenFriendPanel);

        // 창 닫기 버튼
        CharacterObjCloseButton.onClick.AddListener(CloseWindowCharacter);
        CollectionObjCloseButton.onClick.AddListener(CloseWindowCollect);
        SkillWindowPanelCloseButton.onClick.AddListener(CloseWindowSkill);
        InventoryObjCloseButton.onClick.AddListener(CloseWindowInventory);
        SyntheticObjCloseButton.onClick.AddListener(CloseWindowSynthetic);
        DrawingObjCloseButton.onClick.AddListener(CloseWindowDrawing);
        DrawInforcloseButton.onClick.AddListener(CloseWindowDrawInfor);
        MailboxPanelCloseButton.onClick.AddListener(CloseMailPanel);
        RankingPanelCloseButton.onClick .AddListener(CloseRankingPanel);
        NoticePanelCloseButton.onClick.AddListener(CloseNoticePanel);
        SettingPanelCloseButton.onClick.AddListener(CloseSettingPanel);
        FriendPanelCloseButton.onClick.AddListener(CloseFriendPanel);
    }

 #region Open

    private void OpenWindowCharacter()
    {
        if (_currentOpenedWindow != null) return;
        _currentOpenedWindow = CharacterObj;

        SoundManager.Instance.PlaySFX("Button");

        PopUp.PlayGrow(CharacterObj);
    }

    private void OpenWindowCollect()
    {
        if (_currentOpenedWindow != null) return;
        _currentOpenedWindow = CollectionObj;

        SoundManager.Instance.PlaySFX("Button");

        PopUp.PlayGrow(CollectionObj);
    }

    private void OpenWindowInventory()
    {
        if (_currentOpenedWindow != null) return;
        _currentOpenedWindow = InventoryObj;

        SoundManager.Instance.PlaySFX("Button");

        PopUp.PlayGrow(InventoryObj);
    }

    private void OpenWindowSkill()
    {
        if (_currentOpenedWindow != null) return;
        _currentOpenedWindow = SkillWindowPanel;

        SoundManager.Instance.PlaySFX("Button");

        PopUp.PlayGrow(SkillWindowPanel);
    }

    private void OpenWindowSynthetic()
    {
        if (_currentOpenedWindow != null) return;
        _currentOpenedWindow = SyntheticObj;

        SoundManager.Instance.PlaySFX("Button");

        SyntheticUIManager.Instance.RefreshInventory();

        PopUp.PlayGrow(SyntheticObj);
    }

    private void OpenWindowDrawing()
    {
        if (_currentOpenedWindow != null) return;
        _currentOpenedWindow = DrawingObj;

        SoundManager.Instance.PlaySFX("Button");

        PopUp.PlayGrow(DrawingObj);
    }

    private void OpenDrawWindowDrawInfor()
    {
        if (_currentOpenedWindow != null) return;
        _currentOpenedWindow = DrawingObj;

        SoundManager.Instance.PlaySFX("Button");

        PopUp.PlayGrow(DrawInforObj);
    }

    private void OpenMailPanel()
    {
        if (_currentOpenedWindow != null) return;
        _currentOpenedWindow = MailboxPanel;

        SoundManager.Instance.PlaySFX("Button");

        PopUp.PlayGrow(MailboxPanel);
    }

    private void OpenRankingPanel()
    {
        if (_currentOpenedWindow != null) return;
        _currentOpenedWindow = RankingPanel;

        SoundManager.Instance.PlaySFX("Button");

        UserRankingSlotManager.Instance.LoadRanking();

        PopUp.PlayGrow(RankingPanel);
    }

    private void OpenNoticePanel()
    {
        if (_currentOpenedWindow != null) return;
        _currentOpenedWindow = NoticePanel;

        SoundManager.Instance.PlaySFX("Button");

        PopUp.PlayGrow(NoticePanel);
    }

    private void OpenSettingPanel()
    {
        if (_currentOpenedWindow != null) return;
        _currentOpenedWindow = SettingPanel;

        SoundManager.Instance.PlaySFX("Button");

        PopUp.PlayGrow(SettingPanel);
    }

    private void OpenFriendPanel()
    {
        if (_currentOpenedWindow != null) return;
        _currentOpenedWindow = FriendPanel;

        SoundManager.Instance.PlaySFX("Button");

        PopUp.PlayGrow(FriendPanel);
    }

    #endregion

    #region Close

    private void CloseWindowCharacter()
    {
        SoundManager.Instance.PlaySFX("Button");

        PopDown.PlayShrink(CharacterObj);
        _currentOpenedWindow = null;
    }

    private void CloseWindowCollect()
    {
        SoundManager.Instance.PlaySFX("Button");

        PopDown.PlayShrink(CollectionObj);
        _currentOpenedWindow = null;
    }

    private void CloseWindowInventory()
    {
        SoundManager.Instance.PlaySFX("Button");

        PopDown.PlayShrink(InventoryObj);
        _currentOpenedWindow = null;
    }

    private void CloseWindowSkill()
    {
        SoundManager.Instance.PlaySFX("Button");

        PopDown.PlayShrink(SkillWindowPanel);
        _currentOpenedWindow = null;
    }

    private void CloseWindowSynthetic()
    {
        SoundManager.Instance.PlaySFX("Button");

        PopDown.PlayShrink(SyntheticObj);
        _currentOpenedWindow = null;
    }

    private void CloseWindowDrawing()
    {
        SoundManager.Instance.PlaySFX("Button");

        PopDown.PlayShrink(DrawingObj);
        _currentOpenedWindow = null;
    }

    private void CloseWindowDrawInfor()
    {
        SoundManager.Instance.PlaySFX("Button");

        PopDown.PlayShrink(DrawInforObj);
        _currentOpenedWindow = null;
    }

    private void CloseMailPanel()
    {
        SoundManager.Instance.PlaySFX("Button");

        PopDown.PlayShrink(MailboxPanel);
        _currentOpenedWindow = null;
    }

    private void CloseRankingPanel()
    {
        SoundManager.Instance.PlaySFX("Button");

        PopDown.PlayShrink(RankingPanel);
        _currentOpenedWindow = null;
    }
    private void CloseNoticePanel()
    {
        SoundManager.Instance.PlaySFX("Button");

        PopDown.PlayShrink(NoticePanel);
        _currentOpenedWindow = null;
    }

    private void CloseSettingPanel()
    {
        SoundManager.Instance.PlaySFX("Button");

        PopDown.PlayShrink(SettingPanel);
        _currentOpenedWindow = null;
    }

    private void CloseFriendPanel()
    {
        SoundManager.Instance.PlaySFX("Button");

        PopDown.PlayShrink(FriendPanel);
        _currentOpenedWindow = null;
    }

    #endregion
}