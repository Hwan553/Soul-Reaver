
[System.Serializable]   // 직렬화
public class ItemInstance
{
    public ItemBase ItemSo;
    public int EnhanceLevel = 0;        // 강화 수치
    public bool IsEquipped = false;     // 아이템 장착 여부
}

[System.Serializable]
public class RuneInstance
{
    public RuneBase RuneSo;
    public bool IsEquipped = false;     // 아이템 장착 여부
}