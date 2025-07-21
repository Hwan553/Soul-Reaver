using UnityEngine;
using DG.Tweening;

// GameStart 씬에 있는 SignUpPanel 
public class SignUpPanelMove : MonoBehaviour
{
    public void UpMove()
    {
        SoundManager.Instance.PlaySFX("Button");

        RectTransform rect = transform as RectTransform;

        rect.DOKill();

        // Y를 -48으로 3초간 부드럽게 이동
        rect.DOAnchorPosY(-48f, 0.5f).SetEase(Ease.Linear).OnComplete(() =>
        {
            Debug.Log("UI 패널 이동 완료! 현재 Y: " + rect.anchoredPosition.y);
        });
    }

    public void DownMove()
    {
        SoundManager.Instance.PlaySFX("Button");

        RectTransform rect = transform as RectTransform;

        rect.DOKill();

        // Y를 -48으로 3초간 부드럽게 이동
        rect.DOAnchorPosY(-870f, 0.5f).SetEase(Ease.Linear).OnComplete(() =>
        {
            Debug.Log("UI 패널 이동 완료! 현재 Y: " + rect.anchoredPosition.y);
        });
    }
}
