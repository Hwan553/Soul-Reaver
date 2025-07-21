using UnityEngine;

[CreateAssetMenu(menuName = "Rune/DefenseRune")]
public class DefenseRune : RuneBase, IRuneOption, IRuneEffect
{
    public float defense;   // 방어력 (%)
    public float health;    // 체력(%)
    public float mp;        // 마나(%)

    public string GetOptionText()
    {
        return $"방어력: {defense}%\n체력: {health}%\n마나: {mp}";
    }

    public void ApplyEffect(CharacterInventoryManager manager)
    {
        manager.DefenseBonus = defense;
        manager.HealthBouns = health;
        manager.ManaBouns = mp;
    }

    public void RemoveEffect(CharacterInventoryManager manager)
    {
        manager.DefenseBonus = 0;
        manager.HealthBouns = 0;
        manager.ManaBouns = 0;
    }
}