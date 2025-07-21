using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : Singleton<SoundManager>
{
    [SerializeField] private AudioMixer _audioMixer;
    [SerializeField] private AudioMixerGroup _bgmMixerGroup;
    [SerializeField] private AudioMixerGroup _sfxMixerGroup;

    // 배경음(Background Music)을 저장하는 딕셔너리
    private Dictionary<string, AudioClip> _bgmDictionary;
    // 효과음(Sound Effect)을 저장하는 딕셔너리
    private Dictionary<string, AudioClip> _sfxDictionary;

    // 개별 SFX용 AudioSource
    private Dictionary<string, AudioSource> _sfxSources = new();

    [Header("Audio Sources")]
    // BGM 재생을 위한 AudioSource
    [SerializeField] private AudioSource _bgmSource;
    // SFX 재생을 위한 AudioSource
    [SerializeField] private AudioSource _sfxSource;

    [Header("Audio Clips")]
    // BGM 리스트
    [SerializeField] private List<AudioClip> _bgmClips;
    // SFX 리스트
    [SerializeField] private List<AudioClip> _sfxClips;

    // 현재 설정된 BGM 및 SFX 볼륨을 보여주기 위한 변수
    [SerializeField] private float _currentBGMVolume;
    [SerializeField] private float _currentSFXVolume;

    // 싱글톤 초기화 (상속된 Singleton에서 기본 초기화 수행)
    protected override void Initialized()
    {
        base.Initialized();

        Setting();
    }

    // 초기화 설정
    private void Setting()
    {
        _audioMixer = Resources.Load<AudioMixer>("Audio/MasterMixer");
        _bgmMixerGroup = _audioMixer.FindMatchingGroups("BGM")[0];
        _sfxMixerGroup = _audioMixer.FindMatchingGroups("SFX")[0];

        _bgmDictionary = new Dictionary<string, AudioClip>();
        _sfxDictionary = new Dictionary<string, AudioClip>();

        _bgmClips = new List<AudioClip>();
        _sfxClips = new List<AudioClip>();

        // BGM 재생용 Audio Source가 없다면 자동으로 생성
        if (_bgmSource == null)
        {
            _bgmSource = gameObject.AddComponent<AudioSource>();
            _bgmSource.playOnAwake = false;
            _bgmSource.loop = true; // 기본적으로 BGM은 반복 재생
        }

        // SFX 재생용 Audio Source가 없다면 자동으로 생성
        if (_sfxSource == null)
        {
            _sfxSource = gameObject.AddComponent<AudioSource>();
            _sfxSource.playOnAwake = false; // 효과음은 즉시 재생만 수행
        }

        _bgmSource.outputAudioMixerGroup = _bgmMixerGroup;
        _sfxSource.outputAudioMixerGroup = _sfxMixerGroup;

        ResourceAllLoad();
    }

    // 모든 오디오 리소스를 로드
    private void ResourceAllLoad()
    {
        // BGM 클립 등록
        AudioClip geraltOfRiviaClip = Resources.Load<AudioClip>(Define.Geralt_of_Rivia_BGM_Path);
        AddBGM("Geralt of Rivia", geraltOfRiviaClip);
        AudioClip nightMarketClip = Resources.Load<AudioClip>(Define.Night_Market_BGM_Path);
        AddBGM("Night Market", nightMarketClip);

        // SFX 클립 등록
        AudioClip meteorRainClip = Resources.Load<AudioClip>(Define.Meteor_Rain_SFX_Path);
        AddSFX("Meteor Rain", meteorRainClip);
        AudioClip charmingTwinkleSoundforFantasyandMagicClip = Resources.Load<AudioClip>(Define.Charming_Twinkle_Sound_for_Fantasy_and_Magic_SFX_Path);
        AddSFX("Charming Twinkle Sound for Fantasy and Magic", charmingTwinkleSoundforFantasyandMagicClip);
        AudioClip sweepSoundEffectClip = Resources.Load<AudioClip>(Define.Sweep_Sound_Effect_SFX_Path);
        AddSFX("Sweep Sound Effect", sweepSoundEffectClip);
        AudioClip whooshDarkClip = Resources.Load<AudioClip>(Define.Whoosh_Dark_SFX_Path);
        AddSFX("Whoosh Dark", whooshDarkClip);
        AudioClip snowonUmbrellaClip = Resources.Load<AudioClip>(Define.Snow_on_Umbrella_SFX_Path);
        AddSFX("Snow on Umbrella", snowonUmbrellaClip);
        AudioClip glassCinematicHitClip = Resources.Load<AudioClip>(Define.Glass_Cinematic_Hit_SFX_Path);
        AddSFX("Glass Cinematic Hit", glassCinematicHitClip);
        AudioClip electricSparksClip = Resources.Load<AudioClip>(Define.Electric_Sparks_SFX_Path);
        AddSFX("Electric Sparks", electricSparksClip);
        AudioClip angelicPadLoopClip = Resources.Load<AudioClip>(Define.Angelic_Pad_Loop_SFX_Path);
        AddSFX("Angelic Pad Loop", angelicPadLoopClip);

        AudioClip buttonClip = Resources.Load<AudioClip>(Define.Button_SFX_Path);
        AddSFX("Button", buttonClip);
    }

    public void SetMasterVolume(float volume)
    {
        _audioMixer.SetFloat("MasterVolume", Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1.0f)) * 20);
        _currentBGMVolume = volume; // 필요 시 추가 추적용
    }

    // 지정된 이름으로 BGM을 등록
    public void AddBGM(string name, AudioClip clip)
    {
        // 해당 타입의 효과음이 처음 추가될 경우 (중복 방지)
        if (!_bgmDictionary.ContainsKey(name))
        {
            _bgmDictionary.Add(name, clip);
            _bgmClips.Add(clip);
        }
    }

    // 이름에 해당하는 BGM 재생 (옵션으로 반복 여부 지정 가능)
    public void PlayBGM(string name, bool isLoop = true)
    {
        if (_bgmDictionary.TryGetValue(name, out var clip))
        {
            // 이미 재생중인 BGM인지 확인
            if (_bgmSource.clip == clip) return;

            _bgmSource.clip = clip;
            _bgmSource.loop = isLoop;
            _bgmSource.Play();
        }
    }

    // 현재 재생 중인 BGM 일시정지
    public void PauseOnBGM()
    {
        _bgmSource.Pause();
    }

    // 현재 재생 중인 BGM 일시정지 해제
    public void PauseOffBGM()
    {
        _bgmSource.Play();
    }

    // 현재 재생 중인 BGM 정지
    public void StopBGM()
    {
        _bgmSource.Stop();
    }

    // BGM 볼륨 설정 및 현재 볼륨 저장
    public void SetBGMVolume(float volume)
    {
        _audioMixer.SetFloat("BGMVolume", Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1.0f)) * 20);
        _currentBGMVolume = volume;
    }

    // 이름에 해당하는 BGM 제거
    public void RemoveBGM(string name)
    {
        if (_bgmDictionary.TryGetValue(name, out var clip))
        {
            _bgmDictionary.Remove(name);
            _bgmClips.Remove(clip);
        }
    }

    // 지정된 이름으로 SFX를 등록
    public void AddSFX(string name, AudioClip clip)
    {
        // 해당 타입의 효과음이 처음 추가될 경우 (중복 방지)
        if (!_sfxDictionary.ContainsKey(name))
        {
            _sfxDictionary.Add(name, clip);
            _sfxClips.Add(clip);
        }
    }

    // 이름에 해당하는 SFX 재생 (한 번만 재생됨)
    public void PlaySFX(string name)
    {
        if (_sfxDictionary.TryGetValue(name, out var clip))
        {
            if (!_sfxSources.ContainsKey(name))
            {
                var newSource = gameObject.AddComponent<AudioSource>();
                newSource.outputAudioMixerGroup = _sfxMixerGroup;
                newSource.playOnAwake = false;
                _sfxSources.Add(name, newSource);
            }

            var source = _sfxSources[name];
            source.clip = clip;
            source.Play();
        }
    }

    // 현재 재생 중인 SFX 정지
    public void StopSFX(string name)
    {
        if (_sfxSources.TryGetValue(name, out var source))
        {
            source.Stop();
        }
    }

    // SFX 볼륨 설정 및 현재 볼륨 저장
    public void SetSFXVolume(float volume)
    {
        _audioMixer.SetFloat("SFXVolume", Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1.0f)) * 20);
        _currentSFXVolume = volume;
    }

    // 이름에 해당하는 SFX 제거
    public void RemoveSFX(string name)
    {
        if (_sfxDictionary.TryGetValue(name, out var clip))
        {
            _sfxDictionary.Remove(name);
            _sfxClips.Remove(clip);
        }
    }

    public void GetCurrentVolumes(out float master, out float bgm, out float sfx)
    {
        _audioMixer.GetFloat("MasterVolume", out float m);
        _audioMixer.GetFloat("BGMVolume", out float b);
        _audioMixer.GetFloat("SFXVolume", out float s);

        // 음량은 -80~0 dB로 저장되므로 다시 [0~1]로 변환
        master = Mathf.Pow(10f, m / 20f);
        bgm = Mathf.Pow(10f, b / 20f);
        sfx = Mathf.Pow(10f, s / 20f);
    }

    // 객체 관리에서 오브젝트를 정리하는 함수
    protected override void Clear()
    {
        base.Clear();

        _bgmDictionary.Clear(); // 모든 BGM 클립 제거
        _sfxDictionary.Clear(); // 모든 SFX 클립 제거

        _bgmClips.Clear();
        _sfxClips.Clear();
    }
}