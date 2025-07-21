using UnityEngine;
using UnityEngine.UI;

public class UI_Base : MonoBehaviour
{
    private void Awake()
    {
        Initialized();
    }

    protected virtual void Initialized()
    {
        SetCanvas();
    }

    private void SetCanvas()
    {
        // 현재 GameObject에 Canvas 컴포넌트를 가져오거나 없으면 추가
        Canvas canvas = gameObject.GetOrAddComponent<Canvas>();

        // Canvas가 존재하는 경우 설정 적용
        if (canvas != null)
        {
            // UI를 화면 좌표 기준으로 렌더링
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
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