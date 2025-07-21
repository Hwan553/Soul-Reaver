using UnityEngine;

[CreateAssetMenu(fileName = "NewAromorShoesData", menuName = "Item/ArmorShoes")]

public class ArmorShoesDataSo : ItemBase
{
    public override EquipmentType EquipType => EquipmentType.Shoes; // 타입 신발

    public float MoveSpeed;             // 이동속도 (%)
}
