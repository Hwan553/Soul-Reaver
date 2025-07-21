using BackEnd;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum StatType
{
    Attack, Defense, HpRegen, MaxHp, CurrentHp,
    MaxMp, CurrentMp, MpRegen,
    AttackSpeed, CritChance, CritDamage,
    MoveSpeed, GoldGain, SkillDamage
}

// 개별 스탯 데이터와 계산 로직을 포함한 클래스
[Serializable]
public class StatData
{
    public StatType statType;           // 스탯 종류
    public float baseValue;             // 기본값 (초기 스탯)
    public float increasePerUpgrade;    // 강화 1단계 당 증가하는 값
    public float maxUpgradeValue;       // 최대 강화 가능 수치 (현재 업그레이드가 이걸 넘을 수 없음)
    public float currentUpgradeValue;   // 현재 강화된 값
    public bool IsPercent;              // 이 스탯이 % 단위인지 여부

    // % 단위 스탯일 때, 현재 업그레이드된 수치를 퍼센트로 반환 (아닐 경우 0)
    public float TotalPercent => IsPercent ? currentUpgradeValue : 0;

    // 기본값 + 현재 강화값을 합산한 최종 값 반환
    public float TotalValue => baseValue + currentUpgradeValue;

    // 생성자: 스탯 타입, 기본값, 단계당 증가량, 최대 강화량, % 단위 여부 설정
    public StatData(StatType type, float baseVal, float perUpgrade, float maxVal, bool isPercent)
    {
        statType = type;
        baseValue = baseVal;
        increasePerUpgrade = perUpgrade;
        maxUpgradeValue = maxVal;
        currentUpgradeValue = 0.0f;
        IsPercent = isPercent;
    }

    // 강화 가능 여부 체크 (현재 강화값이 최대치보다 작은지)
    public bool CanUpgrade() => currentUpgradeValue < maxUpgradeValue;

    // 강화 수행 (최대치를 넘지 않도록 제한)
    public void Upgrade()
    {
        if (!CanUpgrade()) return;

        currentUpgradeValue += increasePerUpgrade;

        if (currentUpgradeValue > maxUpgradeValue)
            currentUpgradeValue = maxUpgradeValue;
    }

    // 최종값 반환 (기본값 + 강화값)
    public float GetValue()
    {
        return baseValue + currentUpgradeValue;
    }

    // 현재 강화된 수치만 반환 (기본값 제외)
    public float GetAddvalue()
    {
        return currentUpgradeValue;
    }
}

// 메인 강화 시스템 관리자 클래스
public class StatUpgradeManager : Singleton<StatUpgradeManager>
{
    public List<StatData> stats = new List<StatData>();  // 모든 스탯 데이터 리스트

    // UI 관련 텍스트와 버튼 배열
    public TextMeshProUGUI[] statValueTexts;          // 각 스탯의 현재 값 텍스트
    public TextMeshProUGUI[] statEnhancementTexts;    // 각 스탯의 강화량 표시 텍스트
    public TextMeshProUGUI[] upgradeCostTexts;        // 각 스탯 강화 비용 텍스트
    public Button[] upgradeButtons;                     // 각 스탯 강화 버튼

    public int[] upgradeCosts;              // 각 스탯별 현재 강화 비용
    public float upgradeCooldown = 0.3f;    // 연타 방지용 쿨다운 (0.3초)
    private float[] lastUpgradeTimes;       // 각 스탯별 마지막 강화 시간 저장

    public TMP_Text playerGoldText;         // 골드 UI 텍스트

    public PlayerStats playerStats;         // 플레이어 실제 스탯에 적용할 객체

    private string statDataRowInDate = string.Empty;    // 수정 키값

    private void Awake()
    {
        InitStats();    // 스탯 리스트 초기화
    }

    private void Start()
    {
        int statCount = stats.Count;
        upgradeCosts = new int[statCount];          // 강화 비용 배열 초기화
        lastUpgradeTimes = new float[statCount];    // 마지막 강화 시간 배열 초기화

        for (int i = 0; i < statCount; i++)
        {
            upgradeCosts[i] = 100;      // 각 스탯 초기 강화 비용 100골드로 설정
            lastUpgradeTimes[i] = 0.0f;
        }

        LoadStatsFromBackend();
        UpdateUI();   // UI 갱신
    }

    private void Update()
    {
        playerGoldText.text = GameManager.Instance.GoldNum.ToString();
    }

    // 초기 스탯 데이터 세팅 (기본값, 강화량, 최대치, % 여부 설정)
    private void InitStats()
    {
        stats = new List<StatData>
        {
            new StatData(StatType.Attack, 10, 2, 1000000, false),
            new StatData(StatType.Defense, 10, 2,  50000, false),
            new StatData(StatType.MaxHp, 100, 10, 100000000, false),
            new StatData(StatType.HpRegen, 1, 0.5f, 500f, true),
            new StatData(StatType.AttackSpeed, 1.0f, 0.2f, 50.0f, true),
            new StatData(StatType.CritChance, 5f, 0.1f, 100f, true),
            new StatData(StatType.CritDamage, 50f, 1f, 500f, true),
            new StatData(StatType.MoveSpeed, 3, 0.5f, 200f, true),
            new StatData(StatType.MaxMp, 100, 5, 50000, false),
            new StatData(StatType.MpRegen, 1, 0.2f, 200f, true),
            new StatData(StatType.GoldGain, 0, 1f, 1000f, true),
            new StatData(StatType.SkillDamage, 0, 2f, 500f, true)
        };
    }

    // 강화 버튼 클릭 시 실행되는 함수
    public void TryUpgradeStat(int index)
    {
        SoundManager.Instance.PlaySFX("Button");

        if (index < 0 || index >= stats.Count) return;

        var stat = stats[index];

        if (!stat.CanUpgrade()) return;     // 최대치면 종료

        // 쿨타임 내에 클릭 방지
        if (Time.time - lastUpgradeTimes[index] < upgradeCooldown)
            return;

        // 골드가 충분할 경우 강화 진행
        if (GameManager.Instance.GoldNum >= upgradeCosts[index])
        {
            GameManager.Instance.LoseMoney(upgradeCosts[index]);
            stat.Upgrade();                         // 스탯 강화
            upgradeCosts[index] += 50;              // 강화 비용 상승
            lastUpgradeTimes[index] = Time.time;    // 마지막 강화 시간 기록
            UpdateStatsToBackend();                 // 강화 후 수정(저장)
            //SaveStats();                          // 저장 기능 (필요 시 활성화)
        }
        else
        {
            // 골드 부족 시 비용 텍스트를 빨간색으로 표시
            upgradeCostTexts[index].color = Color.red;
            statValueTexts[index].color = Color.red;
            Invoke(nameof(ResetTextColors), 0.5f);  // 0.5초 후 색 복구
        }

        UpdateUI();  // 강화 후 UI 갱신
    }

    // 텍스트 색상을 원래대로 되돌리는 함수
    private void ResetTextColors()
    {
        foreach (var t in upgradeCostTexts) t.color = Color.white;
        foreach (var t in statValueTexts) t.color = Color.white;
        foreach (var t in statEnhancementTexts) t.color = Color.white;
    }

    public float GetBaseStat(StatType statType)
    {
        var stat = stats.Find(s => s.statType == statType);
        return stat != null ? stat.baseValue : 0f;
    }

    public float GetUpgradeValue(StatType statType)
    {
        var stat = stats.Find(s => s.statType == statType);
        return stat != null ? stat.currentUpgradeValue : 0f;
    }


    // UI 전체 갱신 처리 함수
    public void UpdateUI()
    {
        for (int i = 0; i < stats.Count; i++)
        {
            var stat = stats[i];
            StatType type = stat.statType;

            float final = StatUpgradeManager.Instance.GetFinalStat(type);
            float baseValue = stat.baseValue;
            float upgradeOnlyValue = stat.currentUpgradeValue;
            float enhancementBonus = final - baseValue;

            // 퍼센트 스탯 UI
            if (stat.IsPercent)
            {
                statValueTexts[i].text = $"{final:F1}% (+{enhancementBonus:F2}%)";
            }
            else // 정수 스탯 UI
            {
                statValueTexts[i].text = $"{final:F1} (+{enhancementBonus:F2})";
            }

            // 강화량만 따로 표기 (기존처럼 currentUpgradeValue만)
            if (statEnhancementTexts.Length > i)
            {
                statEnhancementTexts[i].text = stat.IsPercent
                    ? $"{upgradeOnlyValue:F1}%"
                    : $"{upgradeOnlyValue:F0}";
            }

            // 강화 비용
            if (upgradeCostTexts.Length > i)
            {
                upgradeCostTexts[i].text = stat.CanUpgrade() ? $"{upgradeCosts[i]}G" : "MAX";
            }

            // 버튼 활성화 여부
            if (upgradeButtons.Length > i)
            {
                upgradeButtons[i].interactable = stat.CanUpgrade();
            }
        }

        // 골드 UI
        if (playerGoldText != null)
            playerGoldText.text = $"{GameManager.Instance.GoldNum}G";

        // 최종 스탯 적용
        if (playerStats != null)
            playerStats.ApplyUpgradedStats(StatUpgradeManager.Instance, CharacterInventoryManager.Instance);
    }

    // 스탯 + 장비 + 룬스텟 받아오는 함수
    public float GetFinalStat(StatType type)
    {
        // stats 리스트에서 statType이 type과 같은 StatData를 찾아서 stat 변수에 저장
        var stat = stats.Find(s => s.statType == type);

        // 만약 해당하는 StatData가 없다면 (즉, null이라면), 0.0f를 반환하고 함수 종료
        if (stat == null) return 0.0f;

        // 해당 스탯의 총합 값 (기본값 + 강화값 등 포함)을 가져옴
        float upgradedValue = stat.TotalValue;

        // 장비 보너스를 초기화 (기본값은 0으로 설정)
        float equipmentBonus = 0f;

        // CharacterInventoryManager 싱글톤 인스턴스를 가져옴
        var inv = CharacterInventoryManager.Instance;

        switch (type)
        {
            case StatType.Attack:
                equipmentBonus += upgradedValue * (inv.WeaponAtkBonus * 0.01f); // 무기 공격력 보너스
                equipmentBonus += upgradedValue * (inv.AtkDamageBonus * 0.01f); // 룬 공격력 보너스
                break;
            case StatType.SkillDamage:
                equipmentBonus += upgradedValue * (inv.WeaponSkillBonus * 0.01f); // 무기 스킬피해량 보너스
                equipmentBonus += upgradedValue * (inv.SkillDamageBonus * 0.01f); // 룬 스킬피해량 보너스
                break;
            case StatType.Defense:
                equipmentBonus += upgradedValue * (inv.TopDefenseBonus + inv.BottomDefenseBonus) * 0.01f; // 상의 / 하의 방어력 보너스
                equipmentBonus += upgradedValue * (inv.DefenseBonus * 0.01f); // 룬 방어력 보너스
                break;
            case StatType.HpRegen:
                equipmentBonus += upgradedValue * (inv.TopHpRegenBonus + inv.HealthRepBonus) * 0.01f; // 상의 / 룬 체력 재생력 보너스
                break;
            case StatType.MpRegen:
                equipmentBonus += upgradedValue * (inv.TopMpRegenBonus + inv.ManaRepBonus) * 0.01f; // 상의 / 룬 마나 재생력 보너스
                break;
            case StatType.MaxHp:
                equipmentBonus += upgradedValue * (inv.BottomHpBonus + inv.HealthBouns) * 0.01f; // 하의 / 룬 체력 보너스
                break;
            case StatType.MaxMp:
                equipmentBonus += upgradedValue * (inv.BottomMpBonus + inv.ManaBouns) * 0.01f; // 하의 / 룬 마나 보너스
                break;
            case StatType.MoveSpeed:
                equipmentBonus += upgradedValue * (inv.ShoesMoveBonus + inv.MoveSpeedBonus) * 0.01f; // 장화 / 룬 이동속도 보너스
                break;
            case StatType.AttackSpeed:
                equipmentBonus += upgradedValue * (inv.GlovesAttackSpeedBonus + inv.AtkSpeedBonus) * 0.01f; // 장갑 / 룬 공격속도 보너스
                break;
            case StatType.CritChance:
                equipmentBonus += upgradedValue * (inv.CriChanceBonus * 0.01f); // 룬 치명타 확률 보너스
                break;
            case StatType.CritDamage:
                equipmentBonus += upgradedValue * (inv.CriDamageBonus * 0.01f); // 룬 치명타 피해 보너스
                break;
            case StatType.GoldGain:
                equipmentBonus += upgradedValue * (inv.GoldBonus * 0.01f); // 룬 골드 획득량 보너스
                break;
        }

        float buffBonus = 0f;
        var matchingBuff = GetMatchingBuffType(type);
        if (matchingBuff.HasValue)
        {
            float buffPercent = BuffManager.Instance.GetBuffBonus(matchingBuff.Value);
            buffBonus = upgradedValue * (buffPercent * 0.01f);
        }
        return upgradedValue + equipmentBonus + buffBonus;
    }

    private BuffType? GetMatchingBuffType(StatType statType)
    {
        switch (statType)
        {
            case StatType.Attack: return BuffType.AttackUp;
            case StatType.Defense: return BuffType.DefenseUp;
            case StatType.MoveSpeed: return BuffType.MoveSpeed;
            case StatType.CritChance: return BuffType.CritChance;
            // 필요하면 추가
            default: return null;
        }
    }



    // 저장
    public void SaveStatsToBackend()
    {
        StatSaveData saveData = new StatSaveData();

        foreach (var stat in stats)
            saveData.currentUpgradeValues.Add(stat.currentUpgradeValue);
        foreach (var cost in upgradeCosts)
            saveData.upgradeCosts.Add(cost);

        string json = JsonUtility.ToJson(saveData);
        Param param = new Param();

        param.Add("StatData", json);

        var bro = Backend.GameData.Insert("USER_DATA", param);

        if (bro.IsSuccess())
        {
            Debug.Log("스탯 저장 성공 (Insert): " + bro);
            statDataRowInDate = bro.GetInDate();
        }
        else
        {
            Debug.LogError("스탯 저장 실패 (Insert): " + bro);
        }
    }

    // 수정
    public void UpdateStatsToBackend()
    {
        if (string.IsNullOrEmpty(statDataRowInDate))
        {
            Debug.LogWarning("수정할 데이터가 없습니다. 먼저 SaveStatsToBackend()를 호출하세요.");

            return;
        }

        StatSaveData saveData = new StatSaveData();

        foreach (var stat in stats)
            saveData.currentUpgradeValues.Add(stat.currentUpgradeValue);
        foreach (var cost in upgradeCosts)
            saveData.upgradeCosts.Add(cost);

        string json = JsonUtility.ToJson(saveData);
        Param param = new Param();

        param.Add("StatData", json);

        var bro = Backend.GameData.UpdateV2("USER_DATA", statDataRowInDate, Backend.UserInDate, param);

        if (bro.IsSuccess())
        {
            Debug.Log("스탯 수정 성공 (Update): " + bro);
        }
        else
        {
            Debug.LogError("스탯 수정 실패 (Update): " + bro);
        }
    }

    // 불러오기
    public void LoadStatsFromBackend()
    {
        var bro = Backend.GameData.GetMyData("USER_DATA", new Where());

        if (bro.IsSuccess())
        {
            var rows = bro.FlattenRows();

            if (rows.Count <= 0)
            {
                Debug.LogWarning("저장된 스탯 데이터가 없습니다.");
                return;
            }

            string json = rows[0]["StatData"].ToString();
            statDataRowInDate = rows[0]["inDate"].ToString();
            StatSaveData saveData = JsonUtility.FromJson<StatSaveData>(json);

            for (int i = 0; i < stats.Count; i++)
                if (i < saveData.currentUpgradeValues.Count)
                { 
                    stats[i].currentUpgradeValue = saveData.currentUpgradeValues[i];
                    Debug.Log($"[로드] {stats[i].statType} = {stats[i].currentUpgradeValue}");
                }

            for (int i = 0; i < upgradeCosts.Length; i++)
                if (i < saveData.upgradeCosts.Count)
                    upgradeCosts[i] = saveData.upgradeCosts[i];

            UpdateUI();

            Debug.Log("스탯 불러오기 성공");
        }
        else
        {
            Debug.LogError("스탯 불러오기 실패: " + bro);
        }
    }
}

[Serializable]
public class StatSaveData
{
    public List<float> currentUpgradeValues = new List<float>();
    public List<int> upgradeCosts = new List<int>();
}