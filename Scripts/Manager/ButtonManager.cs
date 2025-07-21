using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ButtonManager : MonoBehaviour
{
    [SerializeField] private Button _autoSkillButton;
    private Image _autoButtonSprite;

    // 각 탭 버튼에 대한 정보를 묶는 클래스
    [System.Serializable]
    public class TabButton
    {
        public Button button;                    // 탭으로 사용할 버튼 컴포넌트
        public TextMeshProUGUI text;             // 버튼 내 텍스트 (TextMeshPro)
        public GameObject contentPanel;          // 탭 클릭 시 보여줄 내용이 담긴 패널
    }

    public List<TabButton> tabButtons;           // 관리할 탭 버튼 리스트

    // 선택된 탭 버튼의 배경색상
    private Color selectedButtonColor = new Color(0.3529412f, 0.282353f, 0.3411765f, 1f);
    // 선택된 탭 버튼의 텍스트 색상
    private Color selectedTextColor = new Color(0.9490196f, 0.772549f, 0.6509804f, 1f);

    // 선택 안 된 탭 버튼 배경색상
    private Color unselectedButtonColor = new Color(0.07843138f, 0.06666667f, 0.1215686f, 0.8784314f);
    // 선택 안 된 탭 버튼 텍스트 색상
    private Color unselectedTextColor = new Color(0.3301887f, 0.3099413f, 0.3099413f, 1f);

    private TabButton currentSelected;          // 현재 선택된 탭 버튼 저장용

    // 초기화 함수
    private void Start()
    {
        _autoSkillButton.onClick.AddListener(OnAutoSkillButtonClick);

        // 모든 탭 버튼에 클릭 이벤트 리스너 등록
        foreach (var tab in tabButtons)
        {
            // 람다식 사용, 클릭 시 해당 탭 처리 함수 호출
            tab.button.onClick.AddListener(() => OnTabClicked(tab));
        }

        // 초기 탭 선택: 리스트에 버튼이 하나라도 있으면 첫 번째 버튼 선택
        if (tabButtons.Count > 0)
            OnTabClicked(tabButtons[0], playSound : false);

        _autoButtonSprite = _autoSkillButton.GetComponent<Image>();
        _autoButtonSprite.color = Color.yellow;
    }

    // 탭 클릭 시 실행되는 함수
    private void OnTabClicked(TabButton clickedTab, bool playSound = true)
    {
        // 모든 탭을 순회하면서
        foreach (var tab in tabButtons)
        {
            // 클릭한 탭과 현재 순회 중인 탭이 같은지 여부
            bool isSelected = (tab == clickedTab);

            if (isSelected && playSound)
            {
                SoundManager.Instance.PlaySFX("Button");
            }

            // 버튼 배경색 변경 (targetGraphic는 보통 Image 컴포넌트)
            var targetGraphic = tab.button.targetGraphic;

            if (targetGraphic != null)
            {
                targetGraphic.color = isSelected ? selectedButtonColor : unselectedButtonColor;
            }

            // 버튼 텍스트 색상 변경
            if (tab.text != null)
            {
                tab.text.color = isSelected ? selectedTextColor : unselectedTextColor;
            }

            // 해당 탭에 연결된 콘텐츠 패널 활성/비활성 설정
            if (tab.contentPanel != null)
            {
                tab.contentPanel.SetActive(isSelected);
            }
        }

        // 현재 선택된 탭 업데이트
        currentSelected = clickedTab;
    }

    private void OnAutoSkillButtonClick()
    {
        if (SkillManager.Instance.AutoCastEnabled)
        {
            SkillManager.Instance.AutoCastEnabled = false;
            _autoButtonSprite.color = Color.white;
        }
        else
        {
            SkillManager.Instance.AutoCastEnabled = true;
            _autoButtonSprite.color = Color.yellow;
        }
    }

    // 특정 패널을 닫는 함수
    public void ClosePanel(GameObject panelToClose)
    {
        if (panelToClose != null)
        {
            panelToClose.SetActive(false);
        }
    }

    // 특정 패널을 여는 함수
    public void OpenPanel(GameObject panelToOpen)
    {
        if (panelToOpen != null)
        {
            panelToOpen.SetActive(true);
        }
    }
}