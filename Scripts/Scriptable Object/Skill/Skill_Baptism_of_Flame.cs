using UnityEngine;

[CreateAssetMenu(fileName = "Baptism of Flame Data", menuName = "Skill/Baptism of Flame")]

public class Skill_Baptism_of_Flame : Skill_Base
{
    private PlayerController _player;

    private GameObject _spawnedEffect;
    private Transform _target;

    [Header("공격 스킬 데이터")]
    public float Damage;                // 고유 피해량
    public float FinalDamage;           // 최종 피해량

    [Header("이펙트 및 사운드")]
    public GameObject BaptismOfFlameEffectPrefab;   // 스킬 이펙트 프리팹
    public AudioClip BaptismOfFlameSound;           // 스킬 발동 사운드

    private void OnEnable()
    {
        Skill_ID = "001";
        Skill_Name = "불의 세례";
    }

    // 스킬 설정
    public override void Setting()
    {
        _player = FindAnyObjectByType<PlayerController>();

        DamageScalingPerLevel = 0.08f;  // 레벨 당 8% 씩 증가

        RefreshExplanation();
    }

    // Resources 연결
    public override void ConnectResource()
    {
        // Resources 폴더에서 프리팹 연결
        Skill_Icon = Resources.Load<Sprite>(Define.Baptism_of_Flame_Icon_Path);
        BaptismOfFlameEffectPrefab = Resources.Load<GameObject>(Define.Baptism_of_Flame_Prefab_Path);
    }

    // 스킬 사용
    public override void UseSkill(int index)
    {
        if (CurrentCooldown > 0.0f || _player == null || BaptismOfFlameEffectPrefab == null)
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

        // 쿨타임 및 유지 시간 초기화
        CurrentCooldown = CooldownTime;
        RemainingDuration = Duration;

        // 가장 가까운 공격 대상 받아오기
        Vector3 spawnPosition = GetTargetPosition();

        SoundManager.Instance.PlaySFX("Meteor Rain");

        // 스킬 오브젝트 설정 위치에 생성
        _spawnedEffect = Instantiate(BaptismOfFlameEffectPrefab, spawnPosition, Quaternion.identity);

        // 사용한 스킬 정보 전달
        SkillManager.Instance.UseSkillInformation(this, FinalDamage, _spawnedEffect, DetectionType.Collision);

        // 쿨타임 및 유지 시간 계산
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

        SoundManager.Instance.StopSFX("Meteor Rain");

        // 스킬 오브젝트 제거
        Destroy(_spawnedEffect);
    }

    // 설명 다시 계산
    public override void RefreshExplanation()
    {
        base.RefreshExplanation();

        float attack = PlayerStats.Instance.Attack;

        float skillBase = Damage * (1.0f + DamageScalingPerLevel * (Skill_Level - 1));
        float bonusFromAttack = attack;
        FinalDamage = skillBase + bonusFromAttack;

        Skill_Explanation = "하늘에서 정의의 불꽃이 떨어집니다!\n\n" +
                            $"• <color=orange>기본 스킬 피해량</color>: <color=red>{skillBase:F1}</color>\n" +
                            $"• <color=orange>공격력 추가 피해</color>: <color=red>{bonusFromAttack:F1}</color>\n" +
                            $"• <color=yellow>총 피해량</color>: <color=red>{FinalDamage:F1}</color>";
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
        ManaCost = 15.0f;

        // 골드 강화 비용
        GoldCost = 1000;

        // 쿨타임 설정
        CooldownTime = 8.0f;
        CurrentCooldown = 0.0f;

        // 유지 시간 설정
        Duration = 4.0f;
        RemainingDuration = 0.0f;
    }

    private Vector3 GetTargetPosition()
    {
        if (_player.Target != null)
        {
            _target = _player.Target.transform;
            return new Vector3(_target.position.x, 0f, _target.position.z);
        }

        return _player.transform.position;
    }
}