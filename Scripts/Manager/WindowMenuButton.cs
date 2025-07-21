using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class WindowMenuButton : MonoBehaviour
{
    public Button OpenWindowButton;
    public Button CloseWindowButton;

    public RectTransform CurtainPanel;      // 커튼 효과를 줄 RectTransform

    public float AnimationDuration = 0.2f;  // 애니메이션 시간
    private bool _isOpen = false;           // 커튼이 열렸는지 여부

    private void Start()
    {
        // 초기 상태에서 커튼이 안 보이게 설정
        CurtainPanel.localScale = new Vector3(1.0f, 0.0f, 1.0f);    // Y축 크기를 0으로 설정 (안 보이게)
        CurtainPanel.pivot = new Vector2(0.5f, 1.0f);               // 위쪽 고정

        OpenWindowButton.onClick.AddListener(OpenWindow);
        CloseWindowButton.onClick.AddListener(CloseWindow);
    }

    public void OpenWindow()
    {
        if (!_isOpen)
        {
            _isOpen = true;

            SoundManager.Instance.PlaySFX("Button");

            // Y축으로 커튼을 아래로 늘리는 애니메이션 (위쪽은 고정, 아래만 확장)
            CurtainPanel.DOScaleY(1f, AnimationDuration).SetEase(Ease.OutQuad); // localScale.y를 1로 변경
        }
    }

    public void CloseWindow()
    {
        if(_isOpen)
        {
            _isOpen = false;

            SoundManager.Instance.PlaySFX("Button");

            // Y축으로 커튼을 위로 줄이는 애니메이션
            CurtainPanel.DOScaleY(0f, AnimationDuration).SetEase(Ease.InQuad);  // localScale.y를 1로 변경
        }
    }
}