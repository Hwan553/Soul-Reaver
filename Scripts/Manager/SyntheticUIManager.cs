using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SyntheticUIManager : Singleton<SyntheticUIManager>
{
    public SyntheticManager SyntheticManager;

    [Header("TransContent")]
    public Transform WeaponContent;
    public Transform TopContent;
    public Transform BottomContent;
    public Transform ShoesContent;
    public Transform GlovesContent;

    public GameObject InventoySlotPrefab;

    public TMP_Text WarningText;

    private List<ItemSlot> slots = new List<ItemSlot>();

    private Dictionary<Grade, int> _gradeOrder = new Dictionary<Grade, int>()
    {
        { Grade.Legendary, 0 },
        { Grade.Rare, 1 },
        { Grade.Uncommon, 2 },
        { Grade.Common, 3 }
    };
    public void RefreshInventory()
    {
        ClearContents();

        // ID 범위 기준으로 카테고리 분류해서 슬롯 생성
        DrawSortedItems(WeaponContent, 0, 100);
        DrawSortedItems(TopContent, 101, 200);
        DrawSortedItems(BottomContent, 201, 300);
        DrawSortedItems(GlovesContent, 301, 400);
        DrawSortedItems(ShoesContent, 401, 500);
    }
    private void DrawSortedItems(Transform parent, int minID, int maxID)
    {
        var filteredItems = InventoryManager.Instance.InventoryItems
            .Where(item =>
            {
                if (int.TryParse(item.ItemSo.ID, out int idNum))
                    return idNum >= minID && idNum < maxID;
                return false;
            })
            .OrderByDescending(item => item.IsEquipped)
            .ThenBy(item => _gradeOrder.TryGetValue(item.ItemSo.grade, out int order) ? order : int.MaxValue)
            .ThenByDescending(item => item.EnhanceLevel)
            .ThenBy(item => item.ItemSo.ID)
            .ToList();

        foreach (var item in filteredItems)
        {
            GameObject iconObj = Instantiate(InventoySlotPrefab, parent);
            ItemSlot slot = iconObj.GetComponent<ItemSlot>();
            Button btn = iconObj.GetComponent<Button>();

            slot.Setup(item);
            slot.SetUpEquipped(item.IsEquipped);

            btn.onClick.AddListener(() =>
            {
                if(item.IsEquipped)
                {
                    WarningText.color = Color.red; 
                    WarningText.text = "장착중인 장비입니다.";
                    return;
                }

                SyntheticManager.StartSynthat(item);
            });

            slots.Add(slot);
        }
    }

    public void ClearContents()
    {
        foreach (Transform child in WeaponContent) Destroy(child.gameObject);
        foreach (Transform child in TopContent) Destroy(child.gameObject);
        foreach (Transform child in BottomContent) Destroy(child.gameObject);
        foreach (Transform child in GlovesContent) Destroy(child.gameObject);
        foreach (Transform child in ShoesContent) Destroy(child.gameObject);

        slots.Clear();
    }
}