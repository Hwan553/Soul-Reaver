using UnityEngine;

[CreateAssetMenu(fileName = "Purify Data", menuName = "Skill/Purify")]

public class Skill_Purify : Skill_Base
{
    private PlayerController _player;

    private GameObject _spawnedEffect;

    [Header("공격 수치")]
    public float Damage;                // 고유 피해량
    public float FinalDamage;           // 최종 피해량

    [Header("이펙트 및 사운드")]
    public GameObject PurifyPrefab;     // 이펙트 프리팹
    public AudioClip PurifySound;       // 발동 사운드

    private void OnEnable()
    {
        Skill_ID = "008";
        Skill_Name = "정화";
    }

    // 스킬 설정
    public override void Setting()
    {
        _player = FindAnyObjectByType<PlayerController>();

        DamageScalingPerLevel = 0.16f;  // 레벨 당 16% 씩 증가

        RefreshExplanation();
    }

    // Resources 연결
    public override void ConnectResource()
    {
        // Resources 폴더에서 프리팹 연결
        Skill_Icon = Resources.Load<Sprite>(Define.Purify_Icon_Path);
        PurifyPrefab = Resources.Load<GameObject>(Define.Purify_Prefab_Path);
    }

    // 스킬 사용
    public override void UseSkill(int index)
    {
        if (CurrentCooldown > 0.0f || _player == null || PurifyPrefab == null)
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

        SoundManager.Instance.PlaySFX("Angelic Pad Loop");

        // 스킬 오브젝트 생성
        _spawnedEffect = Instantiate(PurifyPrefab, _player.transform.position, Quaternion.identity);

        // 사용한 스킬 정보 전달
        SkillManager.Instance.UseSkillInformation(this, FinalDamage, _spawnedEffect, DetectionType.Trigger);

        // 쿨타임 및 지속 시간 계산
        SkillManager.Instance.CoolDown(this, index, CurrentCooldown);
        SkillManager.Instance.Duration(this, RemainingDuration);
    }

    // 쿨타임 종료
    public override void OnCoolDownEnd()
    {
        base.OnCoolDownEnd();

        // 쿨다운 오브젝트 비활성화
        SkillManager.Instance.SkillCoolDowns[SlotIndex].gameObject.SetActive(false);
    }

    // 유지 시간 종료
    public override void OnDurationEnd()
    {
        base.OnDurationEnd();

        SoundManager.Instance.StopSFX("Angelic Pad Loop");

        // 스킬 오브젝트 제거
        Destroy(_spawnedEffect);
    }

    // 설명 다시 계산
    public override void RefreshExplanation()
    {
        base.RefreshExplanation();

        float baseAttack = PlayerStats.Instance.Attack;

        float skillBaseDamage = Damage * (1.0f + DamageScalingPerLevel * (Skill_Level - 1));
        float attackBasedDamage = baseAttack;

        FinalDamage = skillBaseDamage + attackBasedDamage;

        Skill_Explanation = "모든 부정함을 불태우며 범위 내 적에게 강력한 피해를\n입힙니다.\n\n" +
                            $"• 총 피해량: <color=red>{FinalDamage:F1}</color> (스킬 {skillBaseDamage:F1} + 공격력 {attackBasedDamage:F1})";
    }

    // 스킬 설정 초기화
    public override void ResetSkill()
    {
        base.ResetSkill();

        // 스킬 레벨 설정
        Skill_Level = 1;

        // 데미지 설정
        Damage = 10.0f;

        // 소모 마나
        ManaCost = 100.0f;

        // 골드 강화 비용
        GoldCost = 1000;

        // 쿨타임 설정
        CooldownTime = 120.0f;
        CurrentCooldown = 0.0f;

        // 지속 시간 설정
        Duration = 4.0f;
        RemainingDuration = 0.0f;
    }
}