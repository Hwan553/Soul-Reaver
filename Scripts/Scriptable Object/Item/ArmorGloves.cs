using UnityEngine;

[CreateAssetMenu(fileName = "NewAromorGlovesData", menuName = "Item/ArmorGloves")]

public class ArmorGlovesDataSo : ItemBase
{
    public override EquipmentType EquipType => EquipmentType.Gloves; // 타입 장갑

    public float AttackSpeede;          // 공격속도 (%)
}
