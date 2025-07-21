using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase Instance;
    public List<ItemBase> AllItems;
    public List<RuneBase> AllRunes; // 인스펙터에서 등록

    private void Awake()
    {
        Instance = this;
    }

    public ItemBase GetItemById(string id)
    {
        return AllItems.FirstOrDefault(x => x.ID == id);
    }


    public RuneBase GetRuneById(string id)
    {
        return AllRunes.FirstOrDefault(x => x.ID == id);
    }

}