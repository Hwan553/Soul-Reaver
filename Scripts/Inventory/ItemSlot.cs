using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ItemSlot : MonoBehaviour
{
    public Image BackGroundImage;           // 아이템 등급 색깔
    public Image ItemImage;                 // 아이템 이미지
    public GameObject Equipped;             // 착용중 오브젝트
    public TextMeshProUGUI EnhanceText;     // 아이템 강화

    public ItemInstance ItemInstance { get; private set; }
    public RuneInstance RuneInstance { get; private set; }

    private Dictionary<Grade, Color> _gradeColors = new Dictionary<Grade, Color>()
    {
        { Grade.Legendary, new Color32(237, 171, 95, 255) },
        { Grade.Rare, new Color32(190, 119, 231, 255) },
        { Grade.Uncommon, new Color32(87, 152, 241, 255) },
        { Grade.Common, new Color32(114, 143, 115, 255) }
    };

    public void Setup(ItemInstance item)
    {
        ItemInstance = item;    // 꼭 필요!

        ItemImage.sprite = item.ItemSo.ItemImage;
        EnhanceText.text = item.EnhanceLevel > 0 ? $"Lv {item.EnhanceLevel}" : "Lv 0";

        if (_gradeColors.TryGetValue(item.ItemSo.grade, out var color))
            BackGroundImage.color = color;
        else
            BackGroundImage.color = Color.white;
    }

    public void RuneSetup(RuneInstance item)
    {
        RuneInstance = item;    // 꼭 필요!

        ItemImage.sprite = item.RuneSo.ItemImage;

        BackGroundImage.color = new Color32(221, 160, 221, 255);
    }

    public void SetUpEquipped(bool active)
    {
        // 기본 장착됨 이미지 비활성화
        Equipped.SetActive(active);
    }
}