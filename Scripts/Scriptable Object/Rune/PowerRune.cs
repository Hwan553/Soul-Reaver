using UnityEngine;

[CreateAssetMenu(menuName = "Rune/Power")]
public class PowerRune : RuneBase, IRuneOption, IRuneEffect
{
    public float atkDamage; // 공격 데미지 (%)
    public float skillDamage; // 스킬 피해량 (%)

    public string GetOptionText()
    {
        return $"공격력: {atkDamage}%\n스킬 피해량: {skillDamage}%";
    }

    public void ApplyEffect(CharacterInventoryManager manager)
    {
        manager.AtkDamageBonus = atkDamage;
        manager.SkillDamageBonus = skillDamage;
    }

    public void RemoveEffect(CharacterInventoryManager manager)
    {
        manager.AtkDamageBonus = 0;
        manager.SkillDamageBonus = 0;
    }
}