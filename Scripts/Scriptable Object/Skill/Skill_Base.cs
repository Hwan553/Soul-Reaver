using System.Collections.Generic;
using UnityEngine;

public enum SkillType
{
    Attack,
    Buff,
    Heal,
    Debuff
}

public enum DetectionType
{
    Trigger,
    Collision
}

[System.Serializable]
public class SkillSaveData
{
    public List<SkillData> skills = new List<SkillData>();
}

[System.Serializable]
public class SkillData
{
    public int skillLevel;
    public string skillName;
    public bool isFirstPlay;
}

public abstract class Skill_Base : ScriptableObject
{
    [Header("기본 정보")]
    public int SlotIndex;                   // 할당된 슬롯 위치
    public string Skill_ID;                 // 고유 아이디
    public Sprite Skill_Icon;               // 스킬 아이콘
    public string Skill_Name;               // 스킬 이름

    [TextArea]
    public string Skill_Explanation;        // 설명

    [Header("상태")]
    public SkillType Type;                  // 스킬 타입
    public int Skill_Level;                 // 스킬 레벨
    public bool IsUnlocked;                 // 잠금 여부

    [Header("쿨타임")]
    public float CooldownTime;              // 재사용 대기 시간
    public float CurrentCooldown;           // 현재 쿨다운 상태 (관리용)

    [Header("지속 시간")]
    public float Duration;                  // 효과 지속 시간 (초)
    public float RemainingDuration;         // 남은 효과 지속 시간 (초)

    [Header("자원")]
    public float ManaCost;                  // 마나 또는 자원 소모량

    [Header("강화 관련")]
    public int GoldCost;                    // 골드 강화 비용
    public float DamageScalingPerLevel;     // 레벨당 증가
    public float RegenScalingPerLevel;
    public float BuffOneScalingPerLevel;    // 레벨당 버프 1 증가
    public float BuffTwoScalingPerLevel;    // 레벨당 버프 2 증가

    public bool IsFirstPlay = true;

    public abstract void Setting();         // 스킬 설정
    public abstract void ConnectResource(); // 스킬 연동
    public abstract void UseSkill(int index);   // 스킬 사용

    public virtual void OnCoolDownEnd() { }  // 스킬 쿨타임 종료
    public virtual void OnDurationEnd() { }  // 스킬 지속 시간 종료

    public virtual void RefreshExplanation() { }    // 설명 다시 계산

    public virtual void ResetSkill() { }  // 스킬 리셋
}