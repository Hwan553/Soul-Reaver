using UnityEngine;

[CreateAssetMenu(menuName = "Rune/Critical")]
public class CriticalRune : RuneBase, IRuneOption, IRuneEffect
{
    public float criChance; // 치명타 확률 (%)
    public float criDamage; // 치명타 피해 (%)

    public string GetOptionText()
    {
        return $"치명타 확률: {criChance}%\n치명타 피해: {criDamage}%";
    }

    public void ApplyEffect(CharacterInventoryManager manager)
    {
        manager.CriChanceBonus = criChance;
        manager.CriDamageBonus = criDamage;
    }

    public void RemoveEffect(CharacterInventoryManager manager)
    {
        manager.CriChanceBonus = 0;
        manager.CriDamageBonus = 0;
    }
}