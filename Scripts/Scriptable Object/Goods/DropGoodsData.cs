using UnityEngine;

[CreateAssetMenu(menuName = "Drop/DropGoods")]
public class DropGoodsData : ScriptableObject
{
    public int goldAmount;      // 고정 골드 수량
    public int gachaAmount;     // 고정 뽑기재화 수량

    public int GetGoldAmount()
    {
        return goldAmount;
    }

    public int GetGachaAmount()
    {
        return gachaAmount;
    }
}