using UnityEngine;
using DG.Tweening;

public class PopDown : MonoBehaviour
{
    public float Duration = 0.2f;

    public void PlayShrink(GameObject gameObject)
    {
        gameObject.transform.localScale = Vector3.one;

        gameObject.transform.DOScale(Vector3.zero, Duration)
            .SetEase(Ease.InBack)
            .OnComplete(() => gameObject.SetActive(false));
    }
}