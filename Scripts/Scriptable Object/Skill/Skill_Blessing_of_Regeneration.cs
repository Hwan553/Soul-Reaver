using UnityEngine;

[CreateAssetMenu(fileName = "Blessing of Regeneration Data", menuName = "Skill/Blessing of Regeneration")]

public class Skill_Blessing_of_Regeneration : Skill_Base
{
    private GameObject _buffObject;
    private GameObject _blessingOfRegeneration;

    [Header("회복 수치")]
    public float HpRegeneration; // 초당 체력 회복량
    public float MpRegeneration; // 초당 마나 회복량

    [Header("이펙트 및 사운드")]
    public GameObject BlessingOfRegenerationEffectPrefab;     // 회복 버프 이펙트 프리팹
    public AudioClip BlessingOfRegenerationSound;             // 버프 발동 사운드

    private float _finalHpRegen;    // 합산 체력 회복량
    private float _finalMpRegen;    // 합산 마나 회복량

    private void OnEnable()
    {
        Skill_ID = "002";
        Skill_Name = "재생의 축복";
    }

    // 스킬 설정
    public override void Setting()
    {
        // 오브젝트 받아오기
        _buffObject = SkillManager.Instance.BuffSkillObject;
        _blessingOfRegeneration = _buffObject.transform.Find("Blessing of Regeneration")?.gameObject;

        RegenScalingPerLevel = 0.05f;

        RefreshExplanation();
    }

    // Resources 연결
    public override void ConnectResource()
    {
        // Resources 폴더에서 프리팹 연결
        Skill_Icon = Resources.Load<Sprite>(Define.Blessing_of_Regeneration_Icon_Path);
        BlessingOfRegenerationEffectPrefab = Resources.Load<GameObject>(Define.Blessing_of_Regeneration_Prefab_Path);
    }

    // 스킬 사용
    public override void UseSkill(int index)
    {
        if (_blessingOfRegeneration == null || _blessingOfRegeneration.activeSelf || CurrentCooldown > 0f)
            return;

        if (PlayerStats.Instance.CurrentMp < ManaCost)
        {
            Debug.Log("마나가 부족합니다.");
            return;
        }

        // 마나 차감
        PlayerStats.Instance.CurrentMp -= ManaCost;

        // 슬롯 위치 받아오기
        SlotIndex = index;

        // 쿨타임 및 지속 시간 초기화
        CurrentCooldown = CooldownTime;
        RemainingDuration = Duration;

        // 쿨타임 및 지속 시간 계산
        SkillManager.Instance.CoolDown(this, index, CurrentCooldown);
        SkillManager.Instance.Duration(this, RemainingDuration);

        ApplyRegen();

        SoundManager.Instance.PlaySFX("Charming Twinkle Sound for Fantasy and Magic");

        // 스킬 오브젝트 활성화
        _blessingOfRegeneration.SetActive(true);
    }

    // 쿨타임 종료
    public override void OnCoolDownEnd()
    {
        base.OnCoolDownEnd();

        // 쿨다운 오브젝트 비활성화
        SkillManager.Instance.SkillCoolDowns[SlotIndex].gameObject.SetActive(false);
    }

    // 지속 시간 종료
    public override void OnDurationEnd()
    {
        base.OnDurationEnd();

        if (RemainingDuration <= 0f)
        {
            RemoveRegen();

            // 스킬 오브젝트 비활성화
            _blessingOfRegeneration?.SetActive(false);
        }
    }

    // 설명 다시 계산
    public override void RefreshExplanation()
    {
        base.RefreshExplanation();

        float baseHp = PlayerStats.Instance.HpRegen;
        float baseMp = PlayerStats.Instance.MpRegen;

        float bonusPercent = HpRegeneration + RegenScalingPerLevel * (Skill_Level - 1);

        _finalHpRegen = baseHp * (bonusPercent / 100.0f);
        _finalMpRegen = baseMp * (bonusPercent / 100.0f);

        float totalHp = _finalHpRegen * Duration;
        float totalMp = _finalMpRegen * Duration;

        Skill_Explanation = "신성한 축복의 힘으로 서서히 회복됩니다.\n\n" +
                            $"• 초당 회복량: <color=green>{_finalHpRegen:F1} HP</color> / <color=blue>{_finalMpRegen:F1} MP</color>\n" +
                            $"• 총 회복량: <color=green>{totalHp:F1} HP</color> / <color=blue>{totalMp:F1} MP</color>\n\n" +
                                                 $"(기본 회복량의 <color=yellow>{bonusPercent:F1}%</color> 만큼 증가)";
    }

    // 스킬 설정 초기화
    public override void ResetSkill()
    {
        base.ResetSkill();

        // 스킬 레벨 설정
        Skill_Level = 1;

        // 체력 & 마나 재생 -> + 초당 10% 씩 회복
        HpRegeneration = 10.0f;
        MpRegeneration = 10.0f;

        // 소모 마나
        ManaCost = 25.0f;

        // 골드 강화 비용
        GoldCost = 1000;

        // 쿨타임 설정
        CooldownTime = 30.0f;
        CurrentCooldown = 0.0f;

        // 지속 시간 설정
        Duration = 4.0f;
        RemainingDuration = 0.0f;
    }

    private void ApplyRegen()
    {
        // 현재 체력 & 마나 재생 가져오기
        float baseHp = PlayerStats.Instance.HpRegen;
        float baseMp = PlayerStats.Instance.MpRegen;

        float bonusPercent = HpRegeneration + RegenScalingPerLevel * (Skill_Level - 1);

        _finalHpRegen = baseHp * (bonusPercent / 100.0f);
        _finalMpRegen = baseMp * (bonusPercent / 100.0f);

        PlayerStats.Instance.HpRegen += _finalHpRegen;
        PlayerStats.Instance.MpRegen += _finalMpRegen;
    }

    private void RemoveRegen()
    {
        PlayerStats.Instance.HpRegen -= _finalHpRegen;
        PlayerStats.Instance.MpRegen -= _finalMpRegen;

        _finalHpRegen = 0.0f;
        _finalMpRegen = 0.0f;
    }
}