using UnityEngine;

[CreateAssetMenu(menuName = "Rune/Reproduct")]
public class ReproductionRune : RuneBase, IRuneOption, IRuneEffect
{
    public float healthReproduction;    // 체력 재생 (%)
    public float manaReproduction;      // 마나 재생 (%)

    public string GetOptionText()
    {
        return $"체력 재생력: {healthReproduction}%\n마나 재생력: {manaReproduction}%";
    }

    public void ApplyEffect(CharacterInventoryManager manager)
    {
        manager.HealthRepBonus = healthReproduction;
        manager.ManaRepBonus = manaReproduction;
    }

    public void RemoveEffect(CharacterInventoryManager manager)
    {
        manager.HealthRepBonus = 0;
        manager.ManaRepBonus = 0;
    }
}