using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NoticeContent : MonoBehaviour
{
    [Header("Vertical Layout Group")]
    [SerializeField] private VerticalLayoutGroup _verticalLayoutGroup;

    [Header("Scrollbar Vertical")]
    [SerializeField] private GameObject _scrollbarVertical;

    [Header("NoticeContent Content")]
    [SerializeField] private TMP_Text _noticeTitleText;
    [SerializeField] private Image _noticeImage;
    [SerializeField] private TMP_Text _noticeContentText;

    private RectTransform _titleRect;
    private RectTransform _imageRect;
    private RectTransform _contentRect;
    private RectTransform _scrollbarRect;

    private bool _isAdjustment;

    private const float DefaultWidth = 1030.0f;

    private void Start()
    {
        Setting();
    }

    private void Update()
    {
        AutomaticWidthAdjustment();
    }

    private void Setting()
    {
        _titleRect = _noticeTitleText.GetComponent<RectTransform>();
        _imageRect = _noticeImage.GetComponent<RectTransform>();
        _contentRect = _noticeContentText.GetComponent<RectTransform>();
        _scrollbarRect = _scrollbarVertical.GetComponent<RectTransform>();

        _isAdjustment = false;

        AutomaticWidthAdjustment();
    }

    private void SetWidth(RectTransform rect, float width)
    {
        var sizeDelta = rect.sizeDelta;
        sizeDelta.x = width;
        rect.sizeDelta = sizeDelta;
    }

    private void AutomaticWidthAdjustment()
    {
        bool isScrollbarActive = _scrollbarVertical.activeSelf;
        float scrollbarWidth = _scrollbarRect.rect.width;
        float targetWidth = DefaultWidth - (isScrollbarActive ? scrollbarWidth : 0.0f);

        if (_isAdjustment == _scrollbarVertical.activeSelf) return;

        SetWidth(_titleRect, targetWidth);
        //SetWidth(_imageRect, targetWidth);
        SetWidth(_contentRect, targetWidth);

        // LayoutGroup Padding 조절
        var padding = _verticalLayoutGroup.padding;
        padding.left = isScrollbarActive ? 15 : 20;
        _verticalLayoutGroup.padding = padding;

        _isAdjustment = _scrollbarVertical.activeSelf;
    }
}