using BackEnd;
using LitJson;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryManager : Singleton<InventoryManager>
{
    public InventoryUIManager InventoryUIManager;
    public SyntheticUIManager SyntheticManager;
    public List<ItemInstance> InventoryItems = new();   // 리스트로 아이템 리스트 생성
    public List<RuneInstance> InventoryRunes = new();   // 리스트로 룬 리스트 생성

    public void AddItem(ItemBase itemSo, int enhanceLevel = 0)  // 아이템 추가
    {
        if (itemSo == null) return;

        InventoryItems.Add(new ItemInstance { ItemSo = itemSo, EnhanceLevel = enhanceLevel });
        InventoryUIManager.RefreshInventory();
        SyntheticManager.RefreshInventory();
        UpdateInventoryToBackend();
    }

    // 저장
    public void SaveInventoryToBackend()
    {
        InventorySaveData saveData = new InventorySaveData();

        // 인벤토리 아이템 저장
        foreach (var item in InventoryItems)
        {
            saveData.items.Add(new InventoryItemData
            {
                itemId = item.ItemSo.ID,
                enhanceLevel = item.EnhanceLevel,
                isEquipped = item.IsEquipped
            });
        }
        // 룬 저장
        foreach (var rune in InventoryRunes)
        {
            saveData.runes.Add(new InventoryRuneData
            {
                runeId = rune.RuneSo.ID
            });
        }

        // 장비 슬롯 저장
        saveData.equippedItems.weaponId = CharacterInventoryManager.Instance._equippedWeapon?.ItemSo.ID;
        saveData.equippedItems.topId = CharacterInventoryManager.Instance._equippedTop?.ItemSo.ID;
        saveData.equippedItems.bottomId = CharacterInventoryManager.Instance._equippedBottom?.ItemSo.ID;
        saveData.equippedItems.shoesId = CharacterInventoryManager.Instance._equippedShoes?.ItemSo.ID;
        saveData.equippedItems.glovesId = CharacterInventoryManager.Instance._equippedGloves?.ItemSo.ID;
        saveData.equippedItems.runeId = CharacterInventoryManager.Instance._equippedRunes?.RuneSo.ID;

        string json = JsonUtility.ToJson(saveData);
        Param param = new Param();
        param.Add("StatData", json);

        var bro = Backend.GameData.Insert("USER_INVENTORY", param); // 최초 저장
        if (bro.IsSuccess())
        {
            Debug.Log("인벤토리 저장 성공");
            inventoryRowInDate = bro.GetInDate(); // inDate 저장
        }
        else
        {
            Debug.LogError("인벤토리 저장 실패: " + bro);
        }
    }

    // 수정하기
    private string inventoryRowInDate = string.Empty; // inDate 저장 변수 추가

    public void UpdateInventoryToBackend()
    {
        if (string.IsNullOrEmpty(inventoryRowInDate))
        {
            Debug.LogError("먼저 SaveInventoryToBackend()를 통해 데이터를 삽입해야 합니다.");
            return;
        }

        InventorySaveData saveData = new InventorySaveData();

        foreach (var item in InventoryItems)
        {
            saveData.items.Add(new InventoryItemData
            {
                itemId = item.ItemSo.ID,
                enhanceLevel = item.EnhanceLevel,
                isEquipped = item.IsEquipped
            });
        }

        // 룬 저장
        foreach (var rune in InventoryRunes)
        {
            saveData.runes.Add(new InventoryRuneData
            {
                runeId = rune.RuneSo.ID
            });
        }


        saveData.equippedItems.weaponId = CharacterInventoryManager.Instance._equippedWeapon?.ItemSo?.ID;
        saveData.equippedItems.topId = CharacterInventoryManager.Instance._equippedTop?.ItemSo?.ID;
        saveData.equippedItems.bottomId = CharacterInventoryManager.Instance._equippedBottom?.ItemSo?.ID;
        saveData.equippedItems.shoesId = CharacterInventoryManager.Instance._equippedShoes?.ItemSo?.ID;
        saveData.equippedItems.glovesId = CharacterInventoryManager.Instance._equippedGloves?.ItemSo?.ID;
        saveData.equippedItems.runeId = CharacterInventoryManager.Instance._equippedRunes?.RuneSo?.ID;

        string json = JsonUtility.ToJson(saveData);
        Param param = new Param();
        param.Add("StatData", json);

        var bro = Backend.GameData.UpdateV2("USER_INVENTORY", inventoryRowInDate, Backend.UserInDate, param);

        if (bro.IsSuccess())
            Debug.Log("인벤토리 수정 성공");
        else
            Debug.LogError("인벤토리 수정 실패: " + bro);
    }

    // 불러오기
    public void LoadInventoryFromBackend()
    {
        var bro = Backend.GameData.GetMyData("USER_INVENTORY", new Where());

        if (!bro.IsSuccess())
        {
            Debug.LogError("인벤토리 조회 실패: " + bro);
            return;
        }

        JsonData rows = bro.FlattenRows();

        if (rows.Count <= 0)
        {
            Debug.Log("저장된 인벤토리 없음");
            SaveInventoryToBackend(); // 초기 저장

            return;
        }

        inventoryRowInDate = rows[0]["inDate"].ToString();  // inDate 저장
        string json = rows[0]["StatData"].ToString();

        InventorySaveData saveData = JsonUtility.FromJson<InventorySaveData>(json);

        InventoryItems.Clear();

        InventoryRunes.Clear();

        foreach (var savedRune in saveData.runes)
        {
            RuneBase runeSo = ItemDatabase.Instance.GetRuneById(savedRune.runeId);
            if (runeSo != null)
            {
                InventoryRunes.Add(new RuneInstance { RuneSo = runeSo });
            }
        }


        foreach (var savedItem in saveData.items)
        {
            ItemBase itemSo = ItemDatabase.Instance.GetItemById(savedItem.itemId);
            if (itemSo != null)
            {
                ItemInstance newItem = new ItemInstance
                {
                    ItemSo = itemSo,
                    EnhanceLevel = savedItem.enhanceLevel,
                    IsEquipped = false
                };

                InventoryItems.Add(newItem);

                if (savedItem.isEquipped)
                    CharacterInventoryManager.Instance.CharacterItemWear(newItem);
            }
        }

        TryEquipById(saveData.equippedItems.weaponId);
        TryEquipById(saveData.equippedItems.topId);
        TryEquipById(saveData.equippedItems.bottomId);
        TryEquipById(saveData.equippedItems.shoesId);
        TryEquipById(saveData.equippedItems.glovesId);
        TryEquipRuneById(saveData.equippedItems.runeId);

        InventoryUIManager.RefreshInventory();
    }

    // 아이템 ID로 장비 장착
    private void TryEquipById(string id)
    {
        if (string.IsNullOrEmpty(id)) return;

        var equipped = InventoryItems.FirstOrDefault(x => x.ItemSo.ID == id);

        if (equipped != null)
        {
            equipped.IsEquipped = true;
            CharacterInventoryManager.Instance.CharacterItemWear(equipped);
        }
    }

    private void TryEquipRuneById(string id)
    {
        if (string.IsNullOrEmpty(id)) return;

        var equippedRune = InventoryRunes.FirstOrDefault(x => x.RuneSo.ID == id);

        if (equippedRune != null)
        {
            equippedRune.IsEquipped = true;
            CharacterInventoryManager.Instance.CharacterRuneWear(equippedRune);
        }
    }

    // 룬 추가
    public void RuneAddItem(RuneBase runeSo)
    {
        if (runeSo == null) return;

        InventoryRunes.Add(new RuneInstance { RuneSo = runeSo });
        InventoryUIManager.RefreshInventory();
        SyntheticManager.RefreshInventory();
    }

    public void RemoveRuneItem(RuneInstance runeItem)
    {
        // 판매시 인벤토리 리스트에서 룬 제거
        if (InventoryRunes.Contains(runeItem))
        {
            InventoryRunes.Remove(runeItem);
            InventoryUIManager.RefreshInventory();
            SyntheticManager.RefreshInventory();
        }
    }
}

// 뒤끝 저장 및 불러오기

[System.Serializable]
public class InventorySaveData
{
    public List<InventoryItemData> items = new();
    public EquippedItemData equippedItems = new();
    public List<InventoryRuneData> runes = new(); // ← 룬 리스트 추가

}

[System.Serializable]
public class InventoryItemData
{
    public string itemId;
    public int enhanceLevel;
    public bool isEquipped;
}

[System.Serializable]
public class EquippedItemData
{
    public string weaponId;
    public string topId;
    public string bottomId;
    public string shoesId;
    public string glovesId;
    public string runeId;
}

[System.Serializable]
public class InventoryRuneData
{
    public string runeId;
}


