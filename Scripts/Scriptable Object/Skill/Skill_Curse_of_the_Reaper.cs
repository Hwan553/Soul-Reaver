using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Curse of the Reaper Data", menuName = "Skill/Curse of the Reaper")]

public class Skill_Curse_of_the_Reaper : Skill_Base
{
    private List<GameObject> _deBuffObject;
    private List<GameObject> _curseOfTheReaper;

    [Header("감소 수치 (%)")]
    public float AttackReductionPercent;    // 공격력 감소량
    public float MoveSpeedReductionPercent; // 이동 속도 감소량

    [Header("이펙트 및 사운드")]
    public GameObject CurseOfTheReaperEffectPrefab;   // 디버프 이펙트 프리팹
    public AudioClip CurseOfTheReaperSound;           // 디버프 발동 사운드

    private void OnEnable()
    {
        Skill_ID = "004";
        Skill_Name = "사신의 저주";
    }

    // 스킬 설정
    public override void Setting()
    {
        _deBuffObject = new List<GameObject>();
        _curseOfTheReaper = new List<GameObject>();

        BuffOneScalingPerLevel = 0.05f; // 레벨 당 5% 씩 증가
        BuffTwoScalingPerLevel = 0.05f; // 레벨 당 5% 씩 증가

        RefreshExplanation();
    }

    // Resources 연결
    public override void ConnectResource()
    {
        // Resources 폴더에서 프리팹 연결
        Skill_Icon = Resources.Load<Sprite>(Define.Curse_of_the_Reaper_Icon_Path);
        CurseOfTheReaperEffectPrefab = Resources.Load<GameObject>(Define.Curse_of_the_Reaper_Prefab_Path);
    }

    // 스킬 사용
    public override void UseSkill(int index)
    {
        if (CurrentCooldown > 0.0f) return;

        if (PlayerStats.Instance.CurrentMp < ManaCost)
        {
            Debug.Log("마나가 부족합니다.");
            return;
        }

        // 마나 차감
        PlayerStats.Instance.CurrentMp -= ManaCost;

        RetrieveDebuffObjects();

        SlotIndex = index;
        CurrentCooldown = CooldownTime;
        RemainingDuration = Duration;

        SkillManager.Instance.CoolDown(this, index, CurrentCooldown);
        SkillManager.Instance.Duration(this, RemainingDuration);

        SoundManager.Instance.PlaySFX("Whoosh Dark");

        for (int i = 0; i < _curseOfTheReaper.Count; i++)
        {
            var effect = _curseOfTheReaper[i];
            if (effect == null || effect.activeSelf) continue;

            var enemy = effect.GetComponentInParent<Enemy>();
            if (enemy == null || enemy.IsDebuffed) continue;

            effect.SetActive(true);
            ApplyDebuff(enemy);
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
            for (int i = 0; i < _curseOfTheReaper.Count; i++)
            {
                var effect = _curseOfTheReaper[i];
                if (effect == null || effect.activeSelf) continue;

                var enemy = effect.GetComponentInParent<Enemy>();
                if (enemy == null) continue;

                // 디버프 해제
                RevertDebuff(enemy);

                // 스킬 오브젝트 비활성화
                _curseOfTheReaper[i].SetActive(false);
            }

            _deBuffObject.Clear();
            _curseOfTheReaper.Clear();
        }
    }

    // 설명 다시 계산
    public override void RefreshExplanation()
    {
        base.RefreshExplanation();

        float attackReduction = AttackReductionPercent + BuffOneScalingPerLevel * (Skill_Level - 1);
        float moveSpeedReduction = MoveSpeedReductionPercent + BuffTwoScalingPerLevel * (Skill_Level - 1);

        Skill_Explanation =  "사신의 저주로 적의 힘과 속도를 약화시킵니다.\n\n" +
                             $"• 공격력 감소: <color=red>{attackReduction:F1}%</color>\n" +
                             $"• 이동 속도 감소: <color=blue>{moveSpeedReduction:F1}%</color>\n\n" +
                                                   $"효과는 <color=yellow>{Duration:F1}초</color> 동안 지속됩니다.";
    }

    // 스킬 설정 초기화
    public override void ResetSkill()
    {
        base.ResetSkill();

        // 스킬 레벨 설정
        Skill_Level = 1;

        // 공격력 & 이동속도 감소 배율
        AttackReductionPercent = 10.0f;
        MoveSpeedReductionPercent = 10.0f;

        // 소모 마나
        ManaCost = 20.0f;

        // 골드 강화 비용
        GoldCost = 1000;

        // 쿨타임 설정
        CooldownTime = 15.0f;
        CurrentCooldown = 0.0f;

        // 지속 시간 설정
        Duration = 4.0f;
        RemainingDuration = 0.0f;
    }

    private void ApplyDebuff(Enemy enemy)
    {
        if (enemy.IsDebuffed) return; // 중복 방지

        float attackReduction = enemy.RuntimeStats.AttackDamage * (AttackReductionPercent / 100.0f);
        float moveSpeedReduction = enemy.RuntimeStats.MoveSpeed * (MoveSpeedReductionPercent / 100.0f);

        enemy.RuntimeStats.AttackDamage -= attackReduction;
        enemy.RuntimeStats.MoveSpeed -= moveSpeedReduction;

        enemy.IsDebuffed = true; // 상태 갱신
    }

    private void RevertDebuff(Enemy enemy)
    {
        if (!enemy.IsDebuffed) return; // 디버프 중이 아니면 무시

        float attackRestoration = enemy.RuntimeStats.AttackDamage * (AttackReductionPercent / 100.0f);
        float moveSpeedRestoration = enemy.RuntimeStats.MoveSpeed * (MoveSpeedReductionPercent / 100.0f);

        enemy.RuntimeStats.AttackDamage += attackRestoration;
        enemy.RuntimeStats.MoveSpeed += moveSpeedRestoration;

        enemy.IsDebuffed = false; // 상태 해제
    }

    private void RetrieveDebuffObjects()
    {
        _deBuffObject.Clear();
        _curseOfTheReaper.Clear();

        SkillManager.Instance.FindDebuffObject();
        _deBuffObject = SkillManager.Instance.DeBuffSkillObject;

        foreach (var obj in _deBuffObject)
        {
            var curse = obj.transform.Find("Curse of the Reaper")?.gameObject;
            if (curse != null)
                _curseOfTheReaper.Add(curse);
        }
    }
}