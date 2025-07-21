using UnityEngine;

[CreateAssetMenu(fileName = "Blessing of the Goddess Data", menuName = "Skill/Blessing of the Goddess")]

public class Skill_Blessing_of_the_Goddess : Skill_Base
{
    private GameObject _buffObject;
    private GameObject _blessingOfTheGoddess;

    [Header("버프 수치")]
    public float AttackIncreasePercent;     // 공격력 증가량
    public float DefenseIncreasePercent;    // 방어력 증가량

    [Header("이펙트 및 사운드")]
    public GameObject BlessingOfTheGoddessPrefab;     // 버프 이펙트 프리팹
    public AudioClip BlessingOfTheGoddessSound;       // 버프 발동 사운드

    private float _increasedAttack;
    private float _increasedDefense;

    private void OnEnable()
    {
        Skill_ID = "003";
        Skill_Name = "여신의 가호";
    }

    // 스킬 설정
    public override void Setting()
    {
        // 오브젝트 받아오기
        _buffObject = SkillManager.Instance.BuffSkillObject;
        _blessingOfTheGoddess = _buffObject.transform.Find("Blessing of the Goddess")?.gameObject;

        BuffOneScalingPerLevel = 0.05f; // 레벨 당 5% 씩 증가
        BuffTwoScalingPerLevel = 0.05f; // 레벨 당 5% 씩 증가

        RefreshExplanation();
    }

    // Resources 연결
    public override void ConnectResource()
    {
        // Resources 폴더에서 프리팹 연결
        Skill_Icon = Resources.Load<Sprite>(Define.Blessing_of_the_Goddess_Icon_Path);
        BlessingOfTheGoddessPrefab = Resources.Load<GameObject>(Define.Blessing_of_the_Goddess_Prefab_Path);
    }

    // 스킬 사용
    public override void UseSkill(int index)
    {
        if (_blessingOfTheGoddess == null || _blessingOfTheGoddess.activeSelf || CurrentCooldown > 0.0f)
            return;

        if (PlayerStats.Instance.CurrentMp < ManaCost)
        {
            Debug.Log("마나가 부족합니다.");
            return;
        }

        // 마나 차감
        PlayerStats.Instance.CurrentMp -= ManaCost;

        if (!_blessingOfTheGoddess.activeSelf && CurrentCooldown <= 0.0f)
        {
            // 슬롯 위치 받아오기
            SlotIndex = index;

            // 쿨타임 및 지속 시간 초기화
            CurrentCooldown = CooldownTime;
            RemainingDuration = Duration;

            // 현재 공격력 & 방어력 받아오기
            float currentAttack = StatUpgradeManager.Instance.GetFinalStat(StatType.Attack);
            float currentDefence = StatUpgradeManager.Instance.GetFinalStat(StatType.Defense);

            // 쿨타임 및 지속 시간 계산
            SkillManager.Instance.CoolDown(this, index, CurrentCooldown);
            SkillManager.Instance.Duration(this, RemainingDuration);

            ApplyBuff();

            SoundManager.Instance.PlaySFX("Sweep Sound Effect");

            // 스킬 오브젝트 활성화
            _blessingOfTheGoddess.SetActive(true);
        }
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

            // 스킬 오브젝트 비활성화
            _blessingOfTheGoddess?.SetActive(false);
        }
    }

    // 설명 다시 계산
    public override void RefreshExplanation()
    {
        base.RefreshExplanation();

        float attackPercent = AttackIncreasePercent + BuffOneScalingPerLevel * (Skill_Level - 1);
        float defensePercent = DefenseIncreasePercent + BuffTwoScalingPerLevel * (Skill_Level - 1);

        Skill_Explanation = "여신의 가호로 힘과 의지가 솟아오릅니다.\n\n" +
                            $"• 공격력 증가: <color=red>{attackPercent:F1}%</color>\n" +
                            $"• 방어력 증가: <color=blue>{defensePercent:F1}%</color>\n\n" +
                                                 $"버프 지속 시간 동안 위 수치만큼 능력이 향상됩니다.";
    }

    // 스킬 설정 초기화
    public override void ResetSkill()
    {
        base.ResetSkill();

        // 스킬 레벨 설정
        Skill_Level = 1;

        // 공격력 & 방어력 -> + 10% 증가
        AttackIncreasePercent = 10.0f;
        DefenseIncreasePercent = 10.0f;

        // 소모 마나
        ManaCost = 30.0f;

        // 골드 강화 비용
        GoldCost = 1000;    

        // 쿨타임 설정
        CooldownTime = 45.0f;
        CurrentCooldown = 0.0f;

        // 지속 시간 설정
        Duration = 4.0f;
        RemainingDuration = 0.0f;
    }

    private void ApplyBuff()
    {
        float baseAttack = PlayerStats.Instance.Attack;
        float baseDefense = PlayerStats.Instance.Defense;

        _increasedAttack = baseAttack * (1 + AttackIncreasePercent / 100.0f);
        _increasedDefense = baseDefense * (1 + DefenseIncreasePercent / 100.0f);

        PlayerStats.Instance.Attack += _increasedAttack;
        PlayerStats.Instance.Defense += _increasedDefense;
    }

    private void RemoveBuff()
    {
        PlayerStats.Instance.Attack -= _increasedAttack;
        PlayerStats.Instance.Defense -= _increasedDefense;

        _increasedAttack = 0.0f;
        _increasedDefense = 0.0f;
    }
}