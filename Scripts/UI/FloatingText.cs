using UnityEngine;
using TMPro;

// 일정 시간 동안 위로 떠오르며 점점 사라지는 텍스트를 표현하는 클래스

public class FloatingText : MonoBehaviour
{
    private float _moveSpeed = 50f;         // 위로 이동하는 속도
    private float _fadeDuration = 0.5f;     // 사라지기까지 시간
    private float _startDelay = 0.1f;       // 시작 전 대기 시간

    private TMP_Text _text;
    private RectTransform _rect;
    private CanvasGroup _canvasGroup;

    private float _timer;
    private bool _iswaiting;

    private Vector2 _startPosition;         // 처음 위치 저장

    // 오브젝트가 생성되거나 켜질 때 실행됨
    private void Awake()
    {
        _text = GetComponent<TMP_Text>();                        // 텍스트 컴포넌트 가져오기
        _rect = GetComponent<RectTransform>();                   // 위치 조절을 위한 RectTransform
        _canvasGroup = gameObject.AddComponent<CanvasGroup>();   // 알파 조절을 위한 CanvasGroup 추가

        _startPosition = _rect.anchoredPosition;                 // 처음위치 저장
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }

    // 외부에서 호출하여 텍스트를 표시하는 함수
    public void Show(string message)
    {
        _text.text = message;          // 표시할 메시지 설정
        _timer = 0f;                   // 타이머 초기화
        _iswaiting = true;
        _canvasGroup.alpha = 1f;       // 투명도 초기화 (완전 보이게)
        _rect.anchoredPosition = _startPosition;    // 위치 초기화
        gameObject.SetActive(true);   // 오브젝트 활성화
    }

    // 매 프레임마다 호출되는 함수
    private void Update()
    {
        _timer += Time.deltaTime;      // 타이머 증가

        if(_iswaiting)
        {
            // 대기 시간 안 끝났을 때
            if (_timer < _startDelay)
                return;
            // 대기 시간 끝남, 애니메이션 시작
            _iswaiting = false;
            _timer = 0f; // 애니메이션 타이머 초기화
        }

        // 텍스트를 위로 이동
        _rect.anchoredPosition += Vector2.up * _moveSpeed * Time.deltaTime;

        // 점점 투명하게 (시간에 따라 알파값 감소)
        _canvasGroup.alpha = Mathf.Lerp(1f, 0f, _timer / _fadeDuration);

        // 일정 시간 지나면 비활성화 (다시 사용할 수 있도록)
        if (_timer > _fadeDuration)
        {
            gameObject.SetActive(false);
        }
    }
}