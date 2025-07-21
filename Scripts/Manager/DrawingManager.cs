using UnityEngine;
using TMPro;

public class DrawingManager : MonoBehaviour
{
    public TMP_Text GpText;

    private void Update()
    {
        GpText.text = GameManager.Instance.GpNum.ToString();
    }

    public void OnOneDrawButtonClick()
    {
        SoundManager.Instance.PlaySFX("Button");

        DrawingPanelManager.Instance.StartCoroutine(DrawingPanelManager.Instance.Draw(1, 100, 0.05f));
    }

    public void OnTenDrawButtonClick()
    {
        SoundManager.Instance.PlaySFX("Button");

        DrawingPanelManager.Instance.StartCoroutine(DrawingPanelManager.Instance.Draw(10, 1000, 0.05f));
    }

    public void OnThirtyDrawButtonClick()
    {
        SoundManager.Instance.PlaySFX("Button");

        DrawingPanelManager.Instance.StartCoroutine(DrawingPanelManager.Instance.Draw(30, 3000, 0.05f));
    }
}