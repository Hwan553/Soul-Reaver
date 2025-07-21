using UnityEngine;
using DG.Tweening;

public class PopUp : MonoBehaviour
{
    // DOTween 설치해서 만들었음 안되면 에셋에서 검색 후 무료 다운
    private float Duration = 0.2f;   // 커지는 시간

    public void PlayGrow(GameObject gameObject)
    {
        gameObject.transform.localScale = Vector3.zero;
        gameObject.SetActive(true);

        gameObject.transform.DOScale(Vector3.one, Duration)
            .SetEase(Ease.OutBack);
    }

    // SetEase로 시작
    // Ease.Linear(일정한 속도)
    // Ease.InOutQuad(부드러운 시작과 끝)
    // Ease.OutBack(끝날 때 튕기듯 확장)
}