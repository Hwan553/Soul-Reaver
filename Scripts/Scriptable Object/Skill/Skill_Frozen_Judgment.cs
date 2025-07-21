using UnityEngine;

[CreateAssetMenu(fileName = "Frozen Judgment Data", menuName = "Skill/Frozen Judgment")]

public class Skill_Frozen_Judgment : Skill_Base
{
    private PlayerController _player;

    private GameObject _spawnedEffect;
    private Transform _target;

    [Header("공격 수치")]
    public float Damage;            // 고유 피해량
    public float FinalDamage;       // 최종 피해량

    [Header("이펙트 및 사운드")]
    public GameObject FrozenJudgmentPrefab;     // 스킬 이펙트 프리팹
    public AudioClip FrozenJudgmentSound;       // 스킬 발동 사운드

    private void OnEnable()
    {
        Skill_ID = "006";
        Skill_Name = "빙하의 심판";
    }

    // 스킬 설정
    public override void Setting()
    {
        _player = FindAnyObjectByType<PlayerController>();

        DamageScalingPerLevel = 0.09f;  // 레벨 당 9% 씩 증가

        RefreshExplanation();
    }

    // Resources 연결
    public override void ConnectResource()
    {
        // Resources 폴더에서 프리팹 연결
        Skill_Icon = Resources.Load<Sprite>(Define.Frozen_Judgment_Icon_Path);
        FrozenJudgmentPrefab = Resources.Load<GameObject>(Define.Frozen_Judgment_Prefab_Path);
    }

    // 스킬 사용
    public override void UseSkill(int index)
    {
        if (CurrentCooldown > 0.0f || FrozenJudgmentPrefab == null || _player == null)
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

        // 슬롯 위치 받아오기
        SlotIndex = index;

        // 쿨타임 및 유지 시간 초기화
        CurrentCooldown = CooldownTime;
        RemainingDuration = Duration;

        Vector3 spawnPosition = GetSkillSpawnPosition();

        SoundManager.Instance.PlaySFX("Glass Cinematic Hit");

        // 스킬 오브젝트 생성
        _spawnedEffect = Instantiate(FrozenJudgmentPrefab, spawnPosition, Quaternion.identity);

        // 사용한 스킬 정보 전달
        SkillManager.Instance.UseSkillInformation(this, FinalDamage, _spawnedEffect, DetectionType.Trigger);

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

        if (_spawnedEffect != null)
        {
            // 스킬 오브젝트 제거
            Destroy(_spawnedEffect);
        }
    }

    // 설명 다시 계산
    public override void RefreshExplanation()
    {
        base.RefreshExplanation();

        float baseAttack = PlayerStats.Instance.Attack;

        float skillBaseDamage = Damage * (1.0f + DamageScalingPerLevel * (Skill_Level - 1));
        float attackDamage = baseAttack;

        FinalDamage = skillBaseDamage + attackDamage;

        Skill_Explanation = "거대한 얼음 창이 떨어지며 적을 관통합니다.\n\n" +
                            $"• 총 피해량: <color=red>{FinalDamage:F1}</color> (스킬 피해 {skillBaseDamage:F1} + 공격력 {attackDamage:F1})";
    }

    // 스킬 설정 초기화
    public override void ResetSkill()
    {
        base.ResetSkill();

        // 스킬 레벨 설정
        Skill_Level = 1;

        // 데미지 설정
        Damage = 20.0f;

        // 소모 마나
        ManaCost = 40.0f;

        // 골드 강화 비용
        GoldCost = 1000;

        // 쿨타임 설정
        CooldownTime = 10.0f;
        CurrentCooldown = 0.0f;

        // 유지 시간 설정
        Duration = 1.5f;
        RemainingDuration = 0.0f;
    }

    private Vector3 GetSkillSpawnPosition()
    {
        // 타겟 여부 판단
        if (_player.Target != null)
        {
            // 가장 가까운 공격 대상 받아오기
            _target = _player.Target.transform;

            // 스킬 생성 좌표 반환
            return new Vector3(_target.position.x, 0f, _target.position.z);
        }
        else
        {
            // 스킬 생성 좌표 반환
            return _player.transform.position;
        }
    }
}