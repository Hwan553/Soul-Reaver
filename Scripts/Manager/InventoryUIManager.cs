using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUIManager : Singleton<InventoryUIManager>
{
    List<ItemSlot> slots = new List<ItemSlot>();

    public ItemInfoPanelManager ItemInfoPanelManager;

    public Transform WeaponContent;
    public Transform TopContent;
    public Transform BottomContent;
    public Transform ShoesContent;
    public Transform GlovesContent;
    public Transform RuneContent;

    public GameObject InventorySlotPrefab;

    private Dictionary<Grade, int> _gradeOrder = new Dictionary<Grade, int>()
    {
        { Grade.Legendary, 0 },
        { Grade.Rare, 1 },
        { Grade.Uncommon, 2 },
        { Grade.Common, 3 }
    };

    public void RefreshInventory()
    {
        ClearContents(); // 기존 슬롯들 제거

        // 종류별로 아이템을 나눠서 정렬해서 그리기
        DrawSortedItems(WeaponContent, 0, 100);
        DrawSortedItems(TopContent, 101, 200);
        DrawSortedItems(BottomContent, 201, 300);
        DrawSortedItems(GlovesContent, 301, 400);
        DrawSortedItems(ShoesContent, 401, 500);
        DrawSortedRunes(RuneContent, 701, 800);

    }

    private void DrawSortedItems(Transform parent, int minID, int maxID)
    {
        // 아이템 필터링 
        var filteredItems = InventoryManager.Instance.InventoryItems
            .Where(item =>
            {
                // 숫자로 변환 가능 확인 범위(minID ~ max ID)
                if (int.TryParse(item.ItemSo.ID, out int idNum))
                    return idNum >= minID && idNum < maxID;
                return false;
            })
            // 먼저 장착된 장비 정렬
            .OrderByDescending(item => item.IsEquipped)
            // 등급 + 강화 수치 정렬
            // 오름차순으로 정렬
            .ThenBy(item => _gradeOrder.TryGetValue(item.ItemSo.grade, out int order) ? order : int.MaxValue)
            // 2차 정렬 내림차순으로
            .ThenByDescending(item => item.EnhanceLevel)
            // 이름 순까지 정령
            .ThenBy(item => item.ItemSo.ID)
            // 컬렉션 List<T> 변환
            .ToList();

        foreach (var item in filteredItems)
        {
            GameObject iconObj = Instantiate(InventorySlotPrefab, parent);
            ItemSlot slot = iconObj.GetComponent<ItemSlot>();
            Button btn = iconObj.GetComponent<Button>();

            slot.Setup(item);
            slot.SetUpEquipped(item.IsEquipped);

            // 인벤토리 장비 클릭
            btn.onClick.AddListener(() =>
            {
                SoundManager.Instance.PlaySFX("Button");

                ItemInfoPanelManager.ShowInfo(item);
            });

            slots.Add(slot);
        }
    }

    private void DrawSortedRunes(Transform parent, int minID, int maxID)
    {
        // 룬 필터링 
        var filteredItems = InventoryManager.Instance.InventoryRunes
            .Where(item =>
            {
                if (item?.RuneSo == null || string.IsNullOrEmpty(item.RuneSo.ID))
                    return false;

                if (int.TryParse(item.RuneSo.ID, out int idNum))
                    return idNum >= minID && idNum < maxID;

                return false;
            })

        // ID 순 정렬
        .OrderByDescending(item => item.IsEquipped)
        .ThenBy(item => item.RuneSo.ID)
        .ToList();

        foreach (var item in filteredItems)
        {
            GameObject iconObj = Instantiate(InventorySlotPrefab, parent);
            ItemSlot slot = iconObj.GetComponent<ItemSlot>();
            Button btn = iconObj.GetComponent<Button>();

            slot.RuneSetup(item);
            slot.SetUpEquipped(item.IsEquipped);

            btn.onClick.AddListener(() =>
            {
                SoundManager.Instance.PlaySFX("Button");

                ItemInfoPanelManager.RuneShowInfo(item);
            });

            slots.Add(slot);
        }
    }

    // 아이탬 장착 여부 체크
    public void UpdateEquippedSlot(ItemInstance item, bool isEquipped)
    {
        foreach (var slot in slots)
        {
            if (slot.ItemInstance == item)
            {
                slot.SetUpEquipped(isEquipped);
                break;
            }
        }
        InventoryManager.Instance.UpdateInventoryToBackend();
    }

    // 룬 장착 여부 체크
    public void UpdateEquippedRuneSlot(RuneInstance item, bool isEquipped)
    {
        foreach (var slot in slots)
        {
            if (slot.RuneInstance == item)
            {
                slot.SetUpEquipped(isEquipped);
                break;
            }
        }
        InventoryManager.Instance.UpdateInventoryToBackend();

    }

    // 인벤토리 업데이트 함수
    private void ClearContents()
    {
        foreach (Transform child in WeaponContent) Destroy(child.gameObject);
        foreach (Transform child in TopContent) Destroy(child.gameObject);
        foreach (Transform child in BottomContent) Destroy(child.gameObject);
        foreach (Transform child in ShoesContent) Destroy(child.gameObject);
        foreach (Transform child in GlovesContent) Destroy(child.gameObject);
        foreach (Transform child in RuneContent) Destroy(child.gameObject);

        slots.Clear();
    }
}