using UnityEngine;

[CreateAssetMenu(menuName = "Rune/Gold")]
public class GoldRune : RuneBase, IRuneOption, IRuneEffect
{
    public float goldEarned; // °ñµå È¹µæ·®(%)

    public string GetOptionText()
    {
        return $"°ñµå È¹µæ·®: {goldEarned}";
    }

    public void ApplyEffect(CharacterInventoryManager manager)
    {
        manager.GoldBonus = goldEarned;
    }

    public void RemoveEffect(CharacterInventoryManager manager)
    {
        manager.GoldBonus = 0;
    }
}