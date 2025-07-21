using UnityEngine;

[CreateAssetMenu(fileName = "NewWeaponData", menuName = "Item/Weapon")]

public class WaponDataSo : ItemBase
{
    public override EquipmentType EquipType => EquipmentType.Weapon; // 타입 무기

    public float SkillDamage;           // 스킬 데미지 (%)
    public float AtkDamage;             // 공격력      (%)

}
