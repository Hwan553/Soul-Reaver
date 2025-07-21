using UnityEngine;
using UnityEngine.UI;

public class FriendPanel : MonoBehaviour
{
    [Header("Friend Window")]
    public GameObject friend_List_Window;
    public GameObject friend_Accept_Window;
    public GameObject friend_Request_Window;
   

    [Header("Friend Window Button")]
    public Button friend_List_WinodowButton;
    public Button friend_Accept_WindowButton;
    public Button friend_Request_WindowButton;
   

    [Header("Friend window Panel")]
    public GameObject friend_List_Panel;
    public GameObject friend_Accept_Panel;
    public GameObject friend_Request_Panel;


    [Header("Friend Icon Image")]
    public Image friend_List_Icon;
    public Image friend_Accept_Icon;
    public Image friend_Request_Icon;
   

    [SerializeField] private GameObject[] friend_WindowPanel;

    // 선택된 탭 버튼의 텍스트 색상
    private Color selectedTextColor = new Color(0.9490196f, 0.772549f, 0.6509804f, 1f);

    // 선택 안 된 탭 버튼 텍스트 색상
    private Color unselectedTextColor = new Color(0.3301887f, 0.3099413f, 0.3099413f, 1f);

    private void Start()
    {
        friend_List_WinodowButton.onClick.AddListener(OpenListWindow);
        friend_Accept_WindowButton.onClick.AddListener(OpenAcceptWindow);
        friend_Request_WindowButton.onClick.AddListener(OpenRequestWindow);
   

        // 색상 변경
        friend_List_Icon.color = selectedTextColor;
        friend_Accept_Icon.color = unselectedTextColor;
        friend_Request_Icon.color = unselectedTextColor;
        

        OpenFriendPanel(friend_List_Panel);
        OpenListWindow();
    }

    private void OpenFriendPanel(GameObject targetPanel)
    {
        foreach (GameObject panel in friend_WindowPanel)
            panel.SetActive(false);

        targetPanel.SetActive(true);
    }

    private void OpenListWindow()
    {
        friend_List_Window.SetActive(true);
        friend_Accept_Window.SetActive(false);
        friend_Request_Window.SetActive(false);

        // 색상 변경
        friend_List_Icon.color = selectedTextColor;
        friend_Accept_Icon.color = unselectedTextColor;
        friend_Request_Icon.color = unselectedTextColor;

        OpenFriendPanel(friend_List_Panel);

        SoundManager.Instance.PlaySFX("Button");
    }

    private void OpenAcceptWindow()
    {
        friend_List_Window.SetActive(false);
        friend_Accept_Window.SetActive(true);
        friend_Request_Window.SetActive(false);

        // 색상 변경
        friend_List_Icon.color = unselectedTextColor;
        friend_Accept_Icon.color = selectedTextColor;
        friend_Request_Icon.color = unselectedTextColor;

        OpenFriendPanel(friend_Accept_Panel);

        SoundManager.Instance.PlaySFX("Button");
    }

    private void OpenRequestWindow()
    {
        friend_List_Window.SetActive(false);
        friend_Accept_Window.SetActive(false);
        friend_Request_Window.SetActive(true);

        // 색상 변경
        friend_List_Icon.color = unselectedTextColor;
        friend_Accept_Icon.color = unselectedTextColor;
        friend_Request_Icon.color = selectedTextColor;

        OpenFriendPanel(friend_Request_Panel);

        SoundManager.Instance.PlaySFX("Button");
    }
}
