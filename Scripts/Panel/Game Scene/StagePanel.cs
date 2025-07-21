using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StagePanel : MonoBehaviour
{
    [SerializeField] private Slider _stageWaveSliderBar;
    [SerializeField] private TMP_Text _stageWaveText;
    [SerializeField] private TMP_Text _stageTimerText;

    private void Start()
    {
        _stageWaveSliderBar.maxValue = StageManager.Instance.TotalTime;
        _stageWaveSliderBar.minValue = 0.0f;
    }

    private void Update()
    {
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        if (_stageWaveSliderBar != null)
        {
            if (_stageWaveSliderBar.value <= 0.0f) _stageWaveSliderBar.maxValue = StageManager.Instance.TotalTime;

            _stageWaveSliderBar.value = StageManager.Instance.RemainingTime;
        }
        if (_stageWaveText != null) _stageWaveText.text = $"Wave:{StageManager.Instance.StageWave}";
        if (_stageTimerText != null) _stageTimerText.text = StageManager.Instance.StageTimer;
    }
}