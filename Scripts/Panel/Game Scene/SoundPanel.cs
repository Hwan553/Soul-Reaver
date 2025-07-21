using BackEnd;
using UnityEngine;
using UnityEngine.UI;

public class SoundPanel : MonoBehaviour
{
    [SerializeField] private Slider _masterSlider;
    [SerializeField] private Slider _bgmSlider;
    [SerializeField] private Slider _sfxSlider;

    private void Awake()
    {
        ResourceAllLoad();

        Setting();
    }

    private void ResourceAllLoad()
    {
        _masterSlider = transform.Find("Options/Master Sound/Master Slider").GetComponent<Slider>();
        _bgmSlider = transform.Find("Options/Background Music/Background Music Slider").GetComponent<Slider>();
        _sfxSlider = transform.Find("Options/Sound Effect/Sound Effect Slider").GetComponent<Slider>();
    }
    
    private void Setting()
    {
        _masterSlider.minValue = 0.0001f;
        _masterSlider.maxValue = 1.0f;
        _masterSlider.wholeNumbers = false;

        _bgmSlider.minValue = 0.0001f;
        _bgmSlider.maxValue = 1.0f;
        _bgmSlider.wholeNumbers = false;

        _sfxSlider.minValue = 0.0001f;
        _sfxSlider.maxValue = 1.0f;
        _sfxSlider.wholeNumbers = false;

        ConnectSound();
    }

    private void ConnectSound()
    {
        // 현재 볼륨 상태를 슬라이더에 반영
        float master, bgm, sfx;

        SoundManager.Instance.GetCurrentVolumes(out master, out bgm, out sfx);

        _masterSlider.SetValueWithoutNotify(master);
        _bgmSlider.SetValueWithoutNotify(bgm);
        _sfxSlider.SetValueWithoutNotify(sfx);

        // 슬라이더 변경 시 사운드에 반영
        _masterSlider.onValueChanged.AddListener(SoundManager.Instance.SetMasterVolume);
        _bgmSlider.onValueChanged.AddListener(SoundManager.Instance.SetBGMVolume);
        _sfxSlider.onValueChanged.AddListener(SoundManager.Instance.SetSFXVolume);
    }


}