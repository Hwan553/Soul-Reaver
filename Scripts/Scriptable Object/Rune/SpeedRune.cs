using UnityEngine;

[CreateAssetMenu(menuName = "Rune/Speed")]
public class SpeedRune : RuneBase, IRuneOption, IRuneEffect
{
    public float atkSpeed;  // 공격속도 (%)
    public float moveSpeed; // 이동속도 (%)

    public string GetOptionText()
    {
        return $"공격 속도: {atkSpeed}%\n이동 속도: {moveSpeed}%";
    }

    public void ApplyEffect(CharacterInventoryManager manager)
    {
        manager.AtkSpeedBonus = atkSpeed;
        manager.MoveSpeedBonus = moveSpeed;
    }

    public void RemoveEffect(CharacterInventoryManager manager)
    {
        manager.AtkSpeedBonus = 0;
        manager.MoveSpeedBonus = 0;
    }
}