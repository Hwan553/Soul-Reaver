using UnityEngine;

[CreateAssetMenu(fileName = "Freeze Circle Data", menuName = "Skill/Freeze Circle")]

public class Skill_Freeze_Circle : Skill_Base
{
    private PlayerController _player;

    private GameObject _spawnedEffect;
    private Transform _target;

    [Header("공격 수치")]
    public float Damage;                // 고유 피해량
    public float FinalDamage;           // 최종 피해량

    [Header("이펙트 및 사운드")]
    public GameObject FreezeCirclePrefab;     // 스킬 이펙트 프리팹
    public AudioClip FreezeCircleSound;       // 스킬 발동 사운드

    private void OnEnable()
    {
        Skill_ID = "005";
        Skill_Name = "아이스 에이지";
    }

    // 스킬 설정
    public override void Setting()
    {
        _player = FindAnyObjectByType<PlayerController>();

        DamageScalingPerLevel = 0.1f;   // 레벨 당 10% 씩 증가

        RefreshExplanation();
    }

    // Resources 연결
    public override void ConnectResource()
    {
        // Resources 폴더에서 프리팹 연결
        Skill_Icon = Resources.Load<Sprite>(Define.Freeze_Circle_Icon_Path);
        FreezeCirclePrefab = Resources.Load<GameObject>(Define.Freeze_Circle_Prefab_Path);
    }

    // 스킬 사용
    public override void UseSkill(int index)
    {
        if (CurrentCooldown > 0.0f || _player == null)
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

        Vector3 spawnPosition;

        // 타겟 여부 판단
        if (_player.Target != null)
        {
            // 가장 가까운 공격 대상 받아오기
            _target = _player.Target.transform;
            spawnPosition = new Vector3(_target.position.x, 0.0f, _target.position.z);
        }
        else
        {
            spawnPosition = _player.transform.position;
        }

        SoundManager.Instance.PlaySFX("Snow on Umbrella");

        // 스킬 오브젝트 타겟 위치에 생성
        _spawnedEffect = Instantiate(FreezeCirclePrefab, spawnPosition, Quaternion.identity);

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

        SoundManager.Instance.StopSFX("Snow on Umbrella");

        // 스킬 오브젝트 제거
        Destroy(_spawnedEffect);
    }

    // 설명 다시 계산
    public override void RefreshExplanation()
    {
        base.RefreshExplanation();

        float baseAttack = PlayerStats.Instance.Attack;

        float skillBaseDamage = Damage * (1.0f + DamageScalingPerLevel * (Skill_Level - 1));
        float playerBasedDamage = baseAttack;

        FinalDamage = skillBaseDamage + playerBasedDamage;

        Skill_Explanation = "범위 내 적들에게 얼음 마법 피해를 입히고 이동 속도를\n감소시킵니다.\n\n" +
                            $"• 총 피해량: <color=red>{FinalDamage:F1}</color> (스킬 피해 {skillBaseDamage:F1} + 공격력 {playerBasedDamage:F1})\n" +
                            $"• 지속 시간: <color=blue>{Duration:F1}초</color>";
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
        ManaCost = 35.0f;

        // 골드 강화 비용
        GoldCost = 1000;

        // 쿨타임 설정
        CooldownTime = 12.0f;
        CurrentCooldown = 0.0f;

        // 유지 시간 설정
        Duration = 4.0f;
        RemainingDuration = 0.0f;
    }
}