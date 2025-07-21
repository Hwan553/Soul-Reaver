using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillWindowPanel : MonoBehaviour
{
    [Header("스킬 설명 부모 오브젝트")]
    [SerializeField] private GameObject _skillDescriptionParentObject;

    [Header("스킬 설명 관련")]
    [SerializeField] private GameObject _skillDescriptionIconObject;
    [SerializeField] private TMP_Text _skillDescriptionLevelText;
    [SerializeField] private TMP_Text _skillDescriptionText;
    [SerializeField] private TMP_Text _skillCoolTimeText;
    [SerializeField] private TMP_Text _skillManaCostText;

    [Header("현재 선택된 스킬")]
    [SerializeField] private Skill_Base _currentSkill;
    [SerializeField] private int _selectSkillIndex;

    [Header("스킬 강화 관련")]
    [SerializeField] private TMP_Text _goldText;
    [SerializeField] private TMP_Text _skillGoldCostText;
    [SerializeField] private Button _skillUpgradeButton;

    [Header("스킬 목록 부모 오브젝트")]
    [SerializeField] private GameObject _skillListParentObject;

    [Header("스킬 목록 관련")]
    [SerializeField] private GameObject _SkillIconPrefabs;
    [SerializeField] private List<TMP_Text> _skillLevelText;
    [SerializeField] private List<GameObject> _skillIconListObject;

    [Header("생성된 스킬 아이콘 버튼")]
    [SerializeField] private List<Button> _skillListButton;

    private void Start()
    {
        ResourceAllLoad();

        Setting();

        SkillManager.Instance.LoadSkillDataFromBackend();

        GenerateSkillList();

        SelectSkill(SkillManager.Instance.AllSkills[0]);
    }

    // 게임 오브젝트 리소스를 로드
    private void ResourceAllLoad()
    {
        _SkillIconPrefabs = Resources.Load<GameObject>(Define.Skill_Icon_Prefab_Path);

        // 스킬 설명
        _skillDescriptionParentObject = GameObject.Find("Skill Description");

        _skillDescriptionIconObject = _skillDescriptionParentObject.transform.Find("Skill Description Image/Skill Description Icon Image").gameObject;
        _skillDescriptionLevelText = _skillDescriptionParentObject.transform.Find("Skill Description Level Text").GetComponent<TMP_Text>();
        _skillDescriptionText = _skillDescriptionParentObject.transform.Find("Skill Description Text").GetComponent<TMP_Text>();

        // 소모 마나 & 쿨타임
        _skillCoolTimeText = _skillDescriptionParentObject.transform.Find("Skill CoolTime").GetComponent<TMP_Text>();
        _skillManaCostText = _skillDescriptionParentObject.transform.Find("Skill ManaCost").GetComponent<TMP_Text>();

        // 스킬 목록
        _skillListParentObject = GameObject.Find("Skill List/Viewport").transform.Find("Content").gameObject;

        // 스킬 강화
        _goldText = _skillDescriptionParentObject.transform.Find("Gold/Gold Text").GetComponent<TMP_Text>();
        _skillGoldCostText = _skillDescriptionParentObject.transform.Find("Skill Upgrade Button/Gold/Skill Upgrade Gold Cost Text").GetComponent<TMP_Text>();
        _skillUpgradeButton = _skillDescriptionParentObject.transform.Find("Skill Upgrade Button").GetComponent<Button>();
    }

    // 초기 설정
    private void Setting()
    {
        _skillLevelText = new List<TMP_Text>();
        _skillIconListObject = new List<GameObject>();

        _skillListButton = new List<Button>();

        _skillUpgradeButton.onClick.AddListener(() => OnUpgradeSkillClicked(_currentSkill));
    }

    // 스킬 리스트 추가
    private void GenerateSkillList()
    {
        _skillLevelText.Clear();
        _skillIconListObject.Clear();
        _skillListButton.Clear();

        var allSkills = SkillManager.Instance.AllSkills;

        for (int i = 0; i < allSkills.Count; i++)
        {
            // i번째 스킬 데이터 받아오기
            var skill = allSkills[i];

            // 프리팹 오브젝트 생성
            var iconObj = Instantiate(_SkillIconPrefabs, _skillListParentObject.transform);

            // 생성된 스킬 오브젝트 설정
            iconObj.name = $"Skill {i + 1}";
            iconObj.transform.localScale = Vector3.one;
            iconObj.transform.localPosition = Vector3.zero;

            // 스킬 아이콘 버튼 받아오기
            var button = iconObj.GetComponent<Button>();
            button.onClick.AddListener(() => SelectSkill(skill));

            // 프리팹 내부 컴포넌트 받아오기
            var levelText = iconObj.transform.Find("Skill Level Text").GetComponent<TMP_Text>();
            var iconImage = iconObj.transform.Find("Skill Icon Image").GetComponent<Image>();

            // 받아온 컴포넌트 설정
            levelText.text = $"Lv.{skill.Skill_Level}";
            iconImage.sprite = skill.Skill_Icon;

            // 추가
            _skillIconListObject.Add(iconObj);
            _skillListButton.Add(button);
            _skillLevelText.Add(levelText);
        }
    }

    // 스킬 선택
    private void SelectSkill(Skill_Base skill)
    {
        SoundManager.Instance.PlaySFX("Button");

        var allSkills = SkillManager.Instance.AllSkills;

        _currentSkill = skill;

        for (int i = 0; i < allSkills.Count; i++)
        {
            if (allSkills[i] == skill)
            {
                _selectSkillIndex = i;
                break;
            }
        }

        UpdateSkillDescription(skill);
    }

    // 선택 스킬 설명 업데이트
    private void UpdateSkillDescription(Skill_Base skill)
    {
        _goldText.text = $"{GameManager.Instance.GoldNum}G";
        _skillGoldCostText.text = $"{skill.GoldCost}G";

        _skillDescriptionIconObject.GetComponent<Image>().sprite = skill.Skill_Icon;
        _skillDescriptionLevelText.text = $"LV.{skill.Skill_Level}";
        _skillDescriptionText.text = $"{skill.Skill_Explanation}";

        _skillCoolTimeText.text = $"쿨타임 : {skill.CooldownTime:F1}";
        _skillManaCostText.text = $"소모 마나 : {skill.ManaCost:F1}";
    }

    // 스킬 업그레이드
    private void OnUpgradeSkillClicked(Skill_Base skill)
    {
        SoundManager.Instance.PlaySFX("Button");

        if (_currentSkill == null) return;

        if (GameManager.Instance.GoldNum < _currentSkill.GoldCost)
            return;

        GameManager.Instance.GoldNum -= skill.GoldCost;

        skill.Skill_Level++;
        skill.GoldCost += 1000;

        skill.RefreshExplanation();

        UpdateSkillDescription(_currentSkill);
        UpdateSkillIconLevelText(_selectSkillIndex, _currentSkill.Skill_Level);

        SkillManager.Instance.UpdateSkillDataToBackend();
    }

    private void UpdateSkillIconLevelText(int index, int level)
    {
        if (index >= 0 && index < _skillLevelText.Count)
            _skillLevelText[index].text = $"Lv.{level}";
    }
}