using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Gacha Database", menuName = "Gacha/Database")]
public class GachaDatabaseSO : ScriptableObject
{
    public List<ItemBase> AllItmes;
    
    public List<ItemBase> GetItemsByGrade(Grade grade) // 같은 등급에서 아이템 뽑기
    {
        return AllItmes.FindAll(item => item.grade == grade);
    }
}