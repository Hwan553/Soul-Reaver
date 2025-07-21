using UnityEngine;

[CreateAssetMenu(fileName = "Lightning Armament Data", menuName = "Skill/Lightning Armament")]

public class Skill_Lightning_Armament : Skill_Base
{
    private GameObject _buffObject;
    private GameObject _lightningArmament;

    [Header("버프 수치")]
    public float MoveSpeedPercent; // 이동 속도 증가량 (%)

    [Header("접촉 피해")]
    public float Damage;           // 고정 피해량 (스킬 고유)
    public float FinalDamage;      // 계산된 최종 피해량

    [Header("이펙트 및 사운드")]
    public GameObject LightningArmamentEffectPrefab;    // 이펙트 프리팹
    public AudioClip LightningArmamentSound;            // 사운드

    private float _moveSpeedBuff;

    private void OnEnable()
    {
        Skill_ID = "007";
        Skill_Name = "속전무장";
    }

    // 스킬 설정
    public override void Setting()
    {
        // 오브젝트 받아오기
        _buffObject = SkillManager.Instance.BuffSkillObject;
        _lightningArmament = _buffObject.transform.Find("Lightning Armament")?.gameObject;

        BuffOneScalingPerLevel = 0.05f; // 레벨 당 5% 씩 증가

        RefreshExplanation();
    }

    // Resources 연결
    public override void ConnectResource()
    {
        // Resources 폴더에서 프리팹 연결
        Skill_Icon = Resources.Load<Sprite>(Define.Lightning_Armament_Icon_Path);
        LightningArmamentEffectPrefab = Resources.Load<GameObject>(Define.Lightning_Armament_Prefab_Path);
    }

    // 스킬 사용
    public override void UseSkill(int index)
    {
        if (_lightningArmament == null || _lightningArmament.activeSelf || CurrentCooldown > 0f)
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

        ApplyBuff();

        SoundManager.Instance.PlaySFX("Electric Sparks");

        // 스킬 오브젝트 활성화
        _lightningArmament.SetActive(true);
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

        if (RemainingDuration <= 0.0f)
        {
            RemoveBuff();

            SoundManager.Instance.StopSFX("Electric Sparks");

            // 스킬 오브젝트 비활성화
            _lightningArmament?.SetActive(false);
        }
    }

    // 설명 다시 계산
    public override void RefreshExplanation()
    {
        base.RefreshExplanation();

        // 플레이어 현재 공격력 & 이동 속도 가져오기
        float baseAttack = PlayerStats.Instance.Attack;
        float baseMoveSpeed = PlayerStats.Instance.MoveSpeed;

        // 스킬 자체 데미지 + 공격력 기반 추가 데미지
        float baseSkillDamage = Damage * (1.0f + DamageScalingPerLevel * (Skill_Level - 1));
        float attackBasedDamage = baseAttack * (1.0f + 0.05f * (Skill_Level - 1));
        FinalDamage = baseSkillDamage + attackBasedDamage;

        // 이동 속도 증가 계산
        float moveSpeedIncrease = baseMoveSpeed * (MoveSpeedPercent / 100.0f);

        Skill_Explanation = "일정 시간 동안 주변 적에게 피해를 주고 이동 속도를\n증가시킵니다.\n\n" +
                            $"• 총 피해량: <color=red>{FinalDamage:F1}</color> (스킬 {baseSkillDamage:F1} + 공격력 {attackBasedDamage:F1})\n" +
                            $"• 이동 속도: <color=green>{baseMoveSpeed:F1} (+{moveSpeedIncrease:F1})</color>";
    }

    // 스킬 설정 초기화
    public override void ResetSkill()
    {
        base.ResetSkill();

        // 스킬 레벨 설정
        Skill_Level = 1;

        // 데미지 설정
        Damage = 10.0f;

        // 이동 속도 증가량
        MoveSpeedPercent = 100.0f;  // 100% 증가

        // 소모 마나
        ManaCost = 45.0f;

        // 골드 강화 비용
        GoldCost = 1000;

        // 쿨타임 설정
        CooldownTime = 20.0f;
        CurrentCooldown = 0.0f;

        // 지속 시간 설정
        Duration = 4.0f;
        RemainingDuration = 0.0f;
    }

    private void ApplyBuff()
    {
        float baseMoveSpeed = PlayerStats.Instance.MoveSpeed;
        _moveSpeedBuff = baseMoveSpeed * (MoveSpeedPercent / 100.0f);

        // 이동 속도 버프 적용
        PlayerStats.Instance.MoveSpeed += _moveSpeedBuff;
    }

    private void RemoveBuff()
    {
        // 이동 속도 버프 제거
        PlayerStats.Instance.MoveSpeed -= _moveSpeedBuff;

        _moveSpeedBuff = 0.0f;
    }
}