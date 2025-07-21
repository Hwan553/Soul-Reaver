using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FlidCard : MonoBehaviour
{
    public GameObject CardBack;
    public bool CardBackIsActive;

    private float _flipDuration = 0.1f;

    public bool IsFlipping { get; private set; }

    private BuffData currentBuff;
    private Button _button;

    private void Start()
    {
        _button = GetComponent<Button>();
        if (_button != null)
            _button.onClick.AddListener(OnClick);
    }

    public void SetupCard(BuffData buff)
    {
        currentBuff = buff;
        if (_button != null)
            _button.interactable = true; // 다시 클릭 가능하도록
    }

    public void OnClick()
    {
        if (currentBuff != null)
        {
            BuffManager.Instance.ApplyBuff(currentBuff);
            Debug.Log($"버프 적용됨: {currentBuff.buffName}");

            // 한 번 클릭하면 비활성화
            if (_button != null)
                _button.interactable = false;

            CardFlipManager.Instance.EndFlipCards();
        }
        else
        {
            Debug.LogWarning("BuffData가 설정되지 않았습니다.");
        }
    }

    public IEnumerator FlipCard()
    {
        IsFlipping = true;

        float elapsedTime = 0f;
        Quaternion startRot = transform.rotation;
        Quaternion endRot = Quaternion.Euler(0, CardBackIsActive ? 0 : 180, 0);

        while (elapsedTime < _flipDuration)
        {
            transform.rotation = Quaternion.Lerp(startRot, endRot, elapsedTime / _flipDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.rotation = endRot;

        // 앞뒤 전환
        CardBackIsActive = !CardBackIsActive;
        CardBack.SetActive(CardBackIsActive);

        IsFlipping = false;
    }
}
