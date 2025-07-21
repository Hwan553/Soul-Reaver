using UnityEngine;
public enum Grade { Legendary, Rare, Uncommon, Common}
public enum EquipmentType { Weapon, Top, Bottom, Gloves, Shoes}

public abstract class ItemBase : ScriptableObject
{
    public Sprite ItemImage;        // 이미지
    public string ID;               // 고유 아이디
    public string Name;             // 이름
    public string Explanation;      // 설명
    public Grade grade;             // 등급 (나중에 랭크 대신 이걸로 쓰면됨)

    public bool IsUnlocked = false;  // 기본 잠긴 상태

    public abstract EquipmentType EquipType { get; }

 }
