using UnityEngine;

[CreateAssetMenu(fileName = "NewArmorTopData", menuName = "Item/ArmorTop")]

public class ArmorTopDataSo : ItemBase
{
    public override EquipmentType EquipType => EquipmentType.Top; // 타입 상의

    public float Defense;               // 방어력      (%)
    public float HpRegen;               // 체력재생    (%)
    public float MpRegen;               // 마나재생    (%)
}
