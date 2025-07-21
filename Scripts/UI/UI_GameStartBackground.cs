using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class UI_GameStartBackground : UI_Base
{
    [SerializeField] private TMP_Text _tapToStartText;

    protected override void Initialized()
    {
        base.Initialized();

        NewSetCanvas();

        // 기본 스케일 설정
        _tapToStartText.transform.localScale = Vector3.one;

        // DOTween 애니메이션 : 커졌다 작아졌다 반복
        _tapToStartText.transform
            .DOScale(1.1f, 0.6f)            // 1.1배로 커지는데 0.6초 걸림
            .SetEase(Ease.InOutSine)        // 부드럽게
            .SetLoops(-1, LoopType.Yoyo);   // 무한 반복 + 되돌아오기
    }

    private void NewSetCanvas()
    {
        // 현재 GameObject에 Canvas 컴포넌트를 가져오거나 없으면 추가
        Canvas canvas = gameObject.GetOrAddComponent<Canvas>();

        // Canvas가 존재하는 경우 설정 적용
        if (canvas != null)
        {
            // UI를 화면 좌표 기준으로 렌더링
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            // 정렬 우선 순위를 수동으로 제어 가능하도록 설정
            canvas.overrideSorting = true;
        }

        // 현재 GameObject에 CanvasScaler 컴포넌트를 가져오거나 없으면 추가
        CanvasScaler canvasScaler = gameObject.GetOrAddComponent<CanvasScaler>();
        // UI Scale Mode를 Scale With Screen Size로 설정
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        // Reference Resolution을 FHD (1920 x 1080) 비율로 설정
        canvasScaler.referenceResolution = new Vector2(1920.0f, 1080.0f);
        // Screen Match Mode를 Match Width Or Height로 설정
        canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
    }
}