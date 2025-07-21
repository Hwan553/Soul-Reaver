using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillSlot : MonoBehaviour
{
    [SerializeField] private Dictionary<int, Skill_Base> _skillData;
    [SerializeField] private List<Button> _skillSlotButtons; // 인스펙터에서 슬롯 버튼 8개 연결

    private PlayerController _player;

    private void Start()
    {
        Setting();
    }

    private void Update()
    {
        if (!SkillManager.Instance.AutoCastEnabled) return;

        if (SkillManager.Instance.AllSkills == null) return;

        // 플레이어가 없거나 죽었으면 자동 발동 중지
        if (_player == null || _player.IsDeath) return;

        for (int i = 0; i < SkillManager.Instance.SkillSlots.Count; i++)
        {
            if (SkillManager.Instance.SkillLocks[i].activeSelf) return;

            var skill = SkillManager.Instance.AllSkills[i];

            if (skill.CurrentCooldown <= 0.0f)
            {
                skill.UseSkill(i);
            }
        }
    }

    private void Setting()
    {
        _skillData = new Dictionary<int, Skill_Base>();
        _player = FindAnyObjectByType<PlayerController>();

        // 스킬 관련 리소스 로드
        SkillManager.Instance.ResourceAllLoad();

        SetSlotButton();
        SetSkillIcons();
        BindSlotEvents();
    }

    private void SetSlotButton()
    {
        for (int i = 0; i < SkillManager.Instance.SkillSlots.Count; i++)
        {
            _skillSlotButtons.Add(SkillManager.Instance.SkillSlots[i].GetComponent<Button>());
        }
    }

    private void SetSkillIcons()
    {
        for (int i = 0; i < _skillSlotButtons.Count; i++)
        {
            if (i < SkillManager.Instance.AllSkills.Count)
            {
                var skill = SkillManager.Instance.AllSkills[i];
                _skillData[i] = skill;

                // 아이콘 설정
                SkillManager.Instance.SkillSlots[i].sprite = skill.Skill_Icon;
            }
        }
    }

    private void BindSlotEvents()
    {
        for (int i = 0; i < _skillSlotButtons.Count; i++)
        {
            int slotIndex = i; // 캡처 변수
            _skillSlotButtons[i].onClick.AddListener(() => SelectSkill(slotIndex));
        }
    }

    // 스킬 선택
    private void SelectSkill(int index)
    {
        if (_skillData.TryGetValue(index, out var skill))
        {
            Debug.Log($"[SkillSlot] {index + 1}번 슬롯 클릭됨 - 스킬명 : {skill.Skill_Name}");

            // 스킬 사용
            skill.UseSkill(index);
        }
        else
        {
            Debug.LogWarning($"[SkillSlot] {index + 1}번 슬롯에 등록된 스킬이 없습니다.");
        }
    }

    private void Clear()
    {
        _skillData.Clear();
    }
}