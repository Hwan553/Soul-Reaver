using UnityEngine;

[CreateAssetMenu(fileName = "NewAromorBottomData", menuName = "Item/ArmorBottom")]

public class ArmorBottomDataSo : ItemBase
{
    public override EquipmentType EquipType => EquipmentType.Bottom; // 타입 하의

    public float Defense;               // 방어력      (%)
    public float Hp;                    // 체력        (%)
    public float Mp;                    // 마나        (%)
}
