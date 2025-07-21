using UnityEngine;
using UnityEngine.UI;

public class InventoryContentButton : MonoBehaviour
{
    [Header("Item Window")]
    public GameObject ShoesWindow;
    public GameObject GlovesWindow;
    public GameObject BottomWindow;
    public GameObject TopWindow;
    public GameObject WeaponWindow;
    public GameObject RuneWindow;

    [Header("Item Window Button")]
    public Button ShoesWinodowButton;
    public Button GlovesWindowButton;
    public Button BottomWindowButton;
    public Button TopWindowButton;
    public Button WeaponWindowButton;
    public Button RuneWindowButton;

    [Header("Item window Panel")]
    public GameObject ShoesPanel;
    public GameObject GlovesPanel;
    public GameObject BottomPanel;
    public GameObject TopPanel;
    public GameObject WeaponPanel;
    public GameObject RunePanel;

    [Header("Item Icon Image")]
    public Image ShoesIcon;
    public Image GlovesIcon;
    public Image BottomIcon;
    public Image TopIcon;
    public Image WeaponIcon;
    public Image RuneIcon;

    [SerializeField] private GameObject[] ItemPanel;

    // 선택된 탭 버튼의 텍스트 색상
    private Color selectedTextColor = new Color(0.9490196f, 0.772549f, 0.6509804f, 1f);

    // 선택 안 된 탭 버튼 텍스트 색상
    private Color unselectedTextColor = new Color(0.3301887f, 0.3099413f, 0.3099413f, 1f);

    private void Start()
    {
        ShoesWinodowButton.onClick.AddListener(OpenShoesWindow);
        GlovesWindowButton.onClick.AddListener(OpenGlovesWindow);
        BottomWindowButton.onClick.AddListener(OpenBottomWindow);
        TopWindowButton.onClick.AddListener(OpenTopWindow);
        WeaponWindowButton.onClick.AddListener(OpenWeaponWindow);
        RuneWindowButton.onClick.AddListener(OpenRuneWindow);

        // 색상 변경
        ShoesIcon.color = unselectedTextColor;
        GlovesIcon.color = unselectedTextColor;
        BottomIcon.color = unselectedTextColor;
        TopIcon.color = unselectedTextColor;
        WeaponIcon.color = selectedTextColor;
        RuneIcon.color = unselectedTextColor;

        OpenItemPanel(WeaponPanel);

        ShoesWindow.SetActive(false);
        GlovesWindow.SetActive(false);
        BottomWindow.SetActive(false);
        TopWindow.SetActive(false);
        WeaponWindow.SetActive(true);
        RuneWindow.SetActive(false);

        // 색상 변경
        ShoesIcon.color = unselectedTextColor;
        GlovesIcon.color = unselectedTextColor;
        BottomIcon.color = unselectedTextColor;
        TopIcon.color = unselectedTextColor;
        WeaponIcon.color = selectedTextColor;
        RuneIcon.color = unselectedTextColor;
    }

    private void OpenItemPanel(GameObject targetPanel)
    {
        foreach (GameObject panel in ItemPanel)
            panel.SetActive(false);

        targetPanel.SetActive(true);
    }

    private void OpenShoesWindow()
    {
        SoundManager.Instance.PlaySFX("Button");

        ShoesWindow.SetActive(true);
        GlovesWindow.SetActive(false);
        BottomWindow.SetActive(false);
        TopWindow.SetActive(false);
        WeaponWindow.SetActive(false);
        RuneWindow.SetActive(false);

        // 색상 변경
        ShoesIcon.color = selectedTextColor;
        GlovesIcon.color = unselectedTextColor;
        BottomIcon.color = unselectedTextColor;
        TopIcon.color = unselectedTextColor;
        WeaponIcon.color = unselectedTextColor;
        RuneIcon.color = unselectedTextColor;

        OpenItemPanel(ShoesPanel);
    }

    private void OpenGlovesWindow()
    {
        SoundManager.Instance.PlaySFX("Button");

        ShoesWindow.SetActive(false);
        GlovesWindow.SetActive(true);
        BottomWindow.SetActive(false);
        TopWindow.SetActive(false);
        WeaponWindow.SetActive(false);
        RuneWindow.SetActive(false);

        // 색상 변경
        ShoesIcon.color = unselectedTextColor;
        GlovesIcon.color = selectedTextColor;
        BottomIcon.color = unselectedTextColor;
        TopIcon.color = unselectedTextColor;
        WeaponIcon.color = unselectedTextColor;
        RuneIcon.color = unselectedTextColor;

        OpenItemPanel(GlovesPanel);
    }

    private void OpenBottomWindow()
    {
        SoundManager.Instance.PlaySFX("Button");

        ShoesWindow.SetActive(false);
        GlovesWindow.SetActive(false);
        BottomWindow.SetActive(true);
        TopWindow.SetActive(false);
        WeaponWindow.SetActive(false);
        RuneWindow.SetActive(false);

        // 색상 변경
        ShoesIcon.color = unselectedTextColor;
        GlovesIcon.color = unselectedTextColor;
        BottomIcon.color = selectedTextColor;
        TopIcon.color = unselectedTextColor;
        WeaponIcon.color = unselectedTextColor;
        RuneIcon.color = unselectedTextColor;

        OpenItemPanel(BottomPanel);
    }

    private void OpenTopWindow()
    {
        SoundManager.Instance.PlaySFX("Button");

        ShoesWindow.SetActive(false);
        GlovesWindow.SetActive(false);
        BottomWindow.SetActive(false);
        TopWindow.SetActive(true);
        WeaponWindow.SetActive(false);
        RuneWindow.SetActive(false);

        // 색상 변경
        ShoesIcon.color = unselectedTextColor;
        GlovesIcon.color = unselectedTextColor;
        BottomIcon.color = unselectedTextColor;
        TopIcon.color = selectedTextColor;
        WeaponIcon.color = unselectedTextColor;
        RuneIcon.color = unselectedTextColor;

        OpenItemPanel(TopPanel);
    }
    private void OpenWeaponWindow()
    {
        SoundManager.Instance.PlaySFX("Button");

        ShoesWindow.SetActive(false);
        GlovesWindow.SetActive(false);
        BottomWindow.SetActive(false);
        TopWindow.SetActive(false);
        WeaponWindow.SetActive(true);
        RuneWindow.SetActive(false);

        // 색상 변경
        ShoesIcon.color = unselectedTextColor;
        GlovesIcon.color = unselectedTextColor;
        BottomIcon.color = unselectedTextColor;
        TopIcon.color = unselectedTextColor;
        WeaponIcon.color = selectedTextColor;
        RuneIcon.color = unselectedTextColor;

        OpenItemPanel(WeaponPanel);
    }

    private void OpenRuneWindow()
    {
        SoundManager.Instance.PlaySFX("Button");

        ShoesWindow.SetActive(false);
        GlovesWindow.SetActive(false);
        BottomWindow.SetActive(false);
        TopWindow.SetActive(false);
        WeaponWindow.SetActive(false);
        RuneWindow.SetActive(true);

        // 색상 변경
        ShoesIcon.color = unselectedTextColor;
        GlovesIcon.color = unselectedTextColor;
        BottomIcon.color = unselectedTextColor;
        TopIcon.color = unselectedTextColor;
        WeaponIcon.color = unselectedTextColor;
        RuneIcon.color = selectedTextColor;

        OpenItemPanel(RunePanel);
    }
}