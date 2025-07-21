using BackEnd;
using LitJson;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SkillManager : Singleton<SkillManager>
{
    private string skillRowInDate = string.Empty;   // inDate 저장

    [SerializeField] private List<Skill_Base> _allSkills;

    [SerializeField] private List<Image> _skillSlots;
    [SerializeField] private List<Image> _skillCoolDowns;
    [SerializeField] private List<GameObject> _skillLocks;

    [SerializeField] private bool _autoCastEnabled;

    private GameObject _buffSkillObject;
    private List<GameObject> _debuffSkillObject;

    public IReadOnlyList<Skill_Base> AllSkills => _allSkills;   // 모든 스킬 데이터

    public IReadOnlyList<Image> SkillSlots => _skillSlots;      // 스킬 슬롯
    public IReadOnlyList<Image> SkillCoolDowns => _skillCoolDowns;  // 스킬 쿨타임
    public IReadOnlyList<GameObject> SkillLocks => _skillLocks; // 스킬 잠금

    // 자동 스킬 여부
    public bool AutoCastEnabled
    {
        get { return _autoCastEnabled; }
        set { _autoCastEnabled = value; }
    }

    public GameObject BuffSkillObject => _buffSkillObject;
    public List<GameObject> DeBuffSkillObject => _debuffSkillObject;

    protected override void Initialized()
    {
        base.Initialized();

        Setting();
    }

    // 초기화
    private void Setting()
    {
        _allSkills = new List<Skill_Base>();

        _skillSlots = new List<Image>();
        _skillCoolDowns = new List<Image>();
        _skillLocks = new List<GameObject>();

        _autoCastEnabled = true;
    }


    // 버프 오브젝트 탐색
    private void FindBuffObject()
    {
        if (_buffSkillObject != null) return;

        var player = FindAnyObjectByType<PlayerController>();

        if (player != null)
        {
            _buffSkillObject = player.transform.Find("Buff")?.gameObject;
        }
    }

    // 디버프 오브젝트 탐색
    public void FindDebuffObject()
    {
        PlayerController player = FindAnyObjectByType<PlayerController>();

        if (_debuffSkillObject != null) _debuffSkillObject.Clear();
        _debuffSkillObject = new List<GameObject>();

        foreach (var targetList in player.RandomTargets)
        {
            _debuffSkillObject.Add(targetList.transform.Find("Debuff")?.gameObject);
        }
    }

    // 모든 게임 오브젝트 리소스를 로드하는 함수
    public void ResourceAllLoad()
    {
        int skillIndex = 0;

        FindBuffObject();

        _allSkills = Resources.LoadAll<Skill_Base>(Define.Skill_Scriptable_Path).ToList();

        foreach (var skill in _allSkills)
        {
            skillIndex++;

            skill.ConnectResource();    // 개별 리소스 연결
            skill.Setting();            // 스킬 설정

                     // 첫 실행이면 초기화
            if (skill.IsFirstPlay)
            {
                skill.ResetSkill();     // 스킬 리셋
                skill.RefreshExplanation();
                skill.IsFirstPlay = false; // 이후엔 실행되지 않도록
            }
        }

        Transform skillPanelTransform = Object.FindAnyObjectByType<SkillSlot>()?.transform;

        for (int i = 0; i < _allSkills.Count; i++)
        {
            Transform slot = skillPanelTransform.Find($"Skill Slot {i + 1}");
            _skillSlots.Add(slot.Find("Mask").GetComponent<Image>());
        }

        for (int i = 0; i < _skillSlots.Count; i++)
        {
            Transform slot = skillPanelTransform.Find($"Skill Slot {i + 1}");
            var coolDownObj = slot.Find("CoolDown").gameObject;
            _skillCoolDowns.Add(coolDownObj.GetComponent<Image>());
            coolDownObj.SetActive(false);
        }

        for (int i = 0; i < _skillSlots.Count; i++)
        {
            Transform slot = skillPanelTransform.Find($"Skill Slot {i + 1}");
            _skillLocks.Add(slot.Find("Lock").gameObject);
        }
    }

    public void SaveSkillDataToBackend()
    {
        SkillSaveData saveData = new SkillSaveData();

        foreach (var skill in _allSkills)
        {
            saveData.skills.Add(new SkillData
            {
                skillName = skill.name,
                skillLevel = skill.Skill_Level,
                isFirstPlay = skill.IsFirstPlay
            });
        }

        string json = JsonUtility.ToJson(saveData);
        Param param = new Param();
        param.Add("SkillData", json);

        var bro = Backend.GameData.Insert("USER_SKILL", param);
        if (bro.IsSuccess())
        {
            Debug.Log("스킬 데이터 저장 성공");
            skillRowInDate = bro.GetInDate();
        }
        else
        {
            Debug.LogError("스킬 데이터 저장 실패: " + bro);
        }
    }

    public void UpdateSkillDataToBackend()
    {
        if (string.IsNullOrEmpty(skillRowInDate))
        {
            Debug.LogError("먼저 SaveSkillDataToBackend()를 호출하여 저장을 해야 합니다.");
            return;
        }

        SkillSaveData saveData = new SkillSaveData();

        foreach (var skill in _allSkills)
        {
            saveData.skills.Add(new SkillData
            {
                skillName = skill.name,
                skillLevel = skill.Skill_Level,
                isFirstPlay = skill.IsFirstPlay
            });
        }

        string json = JsonUtility.ToJson(saveData);
        Param param = new Param();
        param.Add("SkillData", json);

        var bro = Backend.GameData.UpdateV2("USER_SKILL", skillRowInDate, Backend.UserInDate, param);
        if (bro.IsSuccess())
        {
            Debug.Log("스킬 데이터 업데이트 성공");
        }
        else
        {
            Debug.LogError("스킬 데이터 업데이트 실패: " + bro);
        }
    }

    public void LoadSkillDataFromBackend()
    {
        var bro = Backend.GameData.GetMyData("USER_SKILL", new Where());

        if (!bro.IsSuccess())
        {
            Debug.LogError("스킬 데이터 불러오기 실패: " + bro);
            return;
        }

        JsonData rows = bro.FlattenRows();

        if (rows.Count == 0)
        {
            Debug.Log("저장된 스킬 데이터 없음 - 최초 저장 수행");
            SaveSkillDataToBackend();
            return;
        }

        skillRowInDate = rows[0]["inDate"].ToString();
        string json = rows[0]["SkillData"].ToString();

        SkillSaveData saveData = JsonUtility.FromJson<SkillSaveData>(json);

        foreach (var savedSkill in saveData.skills)
        {
            Skill_Base skill = _allSkills.Find(s => s.name == savedSkill.skillName);
            if (skill != null)
            {
                skill.Skill_Level = savedSkill.skillLevel;
                skill.IsFirstPlay = savedSkill.isFirstPlay;

                            // ✅ 스킬 레벨에 맞춰 설명 및 골드 비용 재계산
                skill.RefreshExplanation();
                skill.GoldCost = 1000 * skill.Skill_Level;
            }
        }
    }

    // 스킬 데이터, 슬롯 번호, 쿨타임 정보 받아오기
    public void CoolDown(Skill_Base skill, int index, float coolDown)
    {
        StartCoroutine(CoCoolDown(skill, index, coolDown));
    }

    // 스킬 데이터, 지속 시간 정보 받아오기
    public void Duration(Skill_Base skill, float duration)
    {
        StartCoroutine(CoDuration(skill, duration));
    }

    // 스킬 쿨타임 계산
    private IEnumerator CoCoolDown(Skill_Base skill, int index, float coolDown)
    {
        float elapsed = 0.0f;
        Image coolDownImage = _skillCoolDowns[index];

        coolDownImage.gameObject.SetActive(true);
        coolDownImage.fillAmount = 1.0f; // 시작은 100%

        while (elapsed < coolDown)
        {
            elapsed += Time.deltaTime;
            float ratio = Mathf.Clamp01(1.0f - (elapsed / coolDown));
            coolDownImage.fillAmount = ratio;

            yield return null;
        }

        coolDownImage.fillAmount = 0.0f;
        skill.CurrentCooldown = 0.0f;   // 쿨타임 초기화
        skill.OnCoolDownEnd();          // 쿨타임 종료 알림
    }

    // 스킬 지속 시간 계산
    private IEnumerator CoDuration(Skill_Base skill, float duration)
    {
        yield return new WaitForSeconds(duration);

        skill.RemainingDuration = 0.0f; // 지속 시간 종료
        skill.OnDurationEnd();          // 지속 시간 종료 알림
    }

    public void UseSkillInformation(Skill_Base skill, float damage, GameObject skillObject, DetectionType detectionType)
    {
        switch (detectionType)
        {
            case DetectionType.Trigger:
                TriggerDetector trigger = skillObject.GetComponentInChildren<TriggerDetector>();
                if (trigger != null)
                    trigger.SetSkillInfo(skill, damage);
                break;

            case DetectionType.Collision:
                CollisionDetector collision = skillObject.GetComponentInChildren<CollisionDetector>();
                if (collision != null)
                    collision.SetSkillInfo(skill, damage);
                break;
        }
    }

    // 스킬 슬롯 해금
    public void SkillSlotUnlocked()
    {
        int stageWave = StageManager.Instance.StageWave;

        // 현재 해금 가능한 슬롯 개수 계산 (최대 8개)
        int unlockedSlotCount = Mathf.Min(stageWave / 5, _skillLocks.Count);

        for (int i = 0; i < unlockedSlotCount; i++)
        {
            if (_skillLocks[i].activeSelf)
            {
                _skillLocks[i].SetActive(false);
            }

            // 해금 가능한 슬롯일 경우 무조건 true로 설정
            _allSkills[i].IsUnlocked = true;
        }
    }

    protected override void Clear()
    {
        base.Clear();

        _allSkills.Clear();

        _skillSlots.Clear();
        _skillCoolDowns.Clear();
        _skillLocks.Clear();
    }
}