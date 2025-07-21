using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemInfoPanelManager : MonoBehaviour
{
    [Header("Scripts")]
    public PopUp PopUp;
    public PopDown PopDown;
    public InventoryUIManager InventoryUIManager;

    public GameObject Panel;
    public Button PanelButton;
    public Button WearButton;
    public Button SellButton;
    public Image ItemIconImage;
    public Image ItemIconBackGround2;

    [Header("TMP_Text")]
    public TMP_Text WearText;
    public TMP_Text ItemIconText;
    public TMP_Text ItemNameText;
    public TMP_Text GradeText;
    public TMP_Text LvText;
    public TMP_Text OptionText;

    private void Start()
    {
        PanelButton.onClick.AddListener(HideInfo);
        CharacterInventoryManager.Instance._equippedWeapon = null;
        CharacterInventoryManager.Instance._equippedTop = null;
        CharacterInventoryManager.Instance._equippedShoes = null;
        CharacterInventoryManager.Instance._equippedGloves = null;
        CharacterInventoryManager.Instance._equippedBottom = null;
        CharacterInventoryManager.Instance._equippedRunes = null;
    }

    // 아이템 설명 패널
    public void ShowInfo(ItemInstance itemInstance)
    {
        var item = itemInstance.ItemSo;

        ItemIconBackGround2.color = _gradeColors[item.grade];
        ItemIconImage.sprite = item.ItemImage;
        ItemIconText.text = $"Lv {itemInstance.EnhanceLevel}";
        ItemNameText.text = $"이름: {item.Name}";
        GradeText.text = $"{item.grade}";
        LvText.text = $"LV: {itemInstance.EnhanceLevel}";

        // 아이템 옵션 텍스트(종류에 따라 다르게)
        if(item is WaponDataSo wapon)
        {
            OptionText.text = $"공격력: {wapon.AtkDamage * (itemInstance.EnhanceLevel + 1) }%\n스킬 데미지: {wapon.SkillDamage * (itemInstance.EnhanceLevel + 1)}%";
        }
        else if(item is ArmorTopDataSo top)
        {
            OptionText.text = $"방어력: {top.Defense * (itemInstance.EnhanceLevel + 1)}%\n체력 재생: {top.HpRegen * (itemInstance.EnhanceLevel + 1)}%\n마나 재생: {top.MpRegen * (itemInstance.EnhanceLevel + 1)}%";
        }
        else if(item is ArmorShoesDataSo shoes)
        {
            OptionText.text = $"이동 속도: {shoes.MoveSpeed * (itemInstance.EnhanceLevel + 1)}%";
        }
        else if(item is ArmorGlovesDataSo gloves)
        {
            OptionText.text = $"공격 속도: {gloves.AttackSpeede * (itemInstance.EnhanceLevel + 1)}%";
        }
        else if(item is ArmorBottomDataSo bottom)
        {
            OptionText.text = $"방어력: {bottom.Defense * (itemInstance.EnhanceLevel + 1)}%\n체력: {bottom.Hp * (itemInstance.EnhanceLevel + 1)}%\n마나: {bottom.Mp * (itemInstance.EnhanceLevel + 1)}%";
        }
        else
        {
            OptionText.text = "으아악 버그야 버그";
        }

        PopUp.PlayGrow(Panel);

        // 장착, 해제, 교체 버튼
        WearButton.onClick.RemoveAllListeners(); // 이전 리스너 제거(누를때마다 새로운 아이템 갱신)
        WearButton.onClick.AddListener(() =>
        {
            SoundManager.Instance.PlaySFX("Button");

            switch (item.EquipType)
            {
                case EquipmentType.Weapon:
                    HandleEquip(CharacterInventoryManager.Instance._equippedWeapon, itemInstance,
                        () => CharacterInventoryManager.Instance.CharacterWaponItemUnwear(),
                        () => CharacterInventoryManager.Instance.CharacterItemWear(itemInstance));
                    break;
                case EquipmentType.Top:
                    HandleEquip(CharacterInventoryManager.Instance._equippedTop, itemInstance,
                        () => CharacterInventoryManager.Instance.CharacterTopItemUnwear(),
                        () => CharacterInventoryManager.Instance.CharacterItemWear(itemInstance));
                    break;
                case EquipmentType.Bottom:
                    HandleEquip(CharacterInventoryManager.Instance._equippedBottom, itemInstance,
                        () => CharacterInventoryManager.Instance.CharacterBottomItemUnwear(),
                        () => CharacterInventoryManager.Instance.CharacterItemWear(itemInstance));
                    break;
                case EquipmentType.Gloves:
                    HandleEquip(CharacterInventoryManager.Instance._equippedGloves, itemInstance,
                        () => CharacterInventoryManager.Instance.CharacterGlovesItemUnwear(),
                        () => CharacterInventoryManager.Instance.CharacterItemWear(itemInstance));
                    break;
                case EquipmentType.Shoes:
                    HandleEquip(CharacterInventoryManager.Instance._equippedShoes, itemInstance,
                        () => CharacterInventoryManager.Instance.CharacterShoesItemUnwear(),
                        () => CharacterInventoryManager.Instance.CharacterItemWear(itemInstance));
                    break;
            }
            HideInfo();
        });

        // 버튼 UI 텍스트 바꾸기
        switch(item.EquipType)
        {
            case EquipmentType.Weapon:
                TextEqulp(CharacterInventoryManager.Instance._equippedWeapon, itemInstance);
                break;
            case EquipmentType.Top:
                TextEqulp(CharacterInventoryManager.Instance._equippedTop, itemInstance);
                break;
            case EquipmentType.Bottom:
                TextEqulp(CharacterInventoryManager.Instance._equippedBottom, itemInstance);
                break;
            case EquipmentType.Gloves:
                TextEqulp(CharacterInventoryManager.Instance._equippedGloves, itemInstance);
                break;
            case EquipmentType.Shoes:
                TextEqulp(CharacterInventoryManager.Instance._equippedShoes, itemInstance);
                break;
        }

        SellButton.gameObject.SetActive(false);
    }

#region RuneShowInfo

    // 룬 옵션 텍스트(종류에 따라 다르게)
    public void RuneShowInfo(RuneInstance runeInstance)
    {
        var rune = runeInstance.RuneSo;
        ItemIconImage.sprite = rune.ItemImage;
        ItemIconBackGround2.color = new Color32(221, 160, 221, 255);
        ItemNameText.text = $"이름: {rune.Name}";

        if (rune is IRuneOption optionProvider)
        {
            OptionText.text = optionProvider.GetOptionText();
        }
        else
        {
            OptionText.text = "으아악 버그야 버그";
        }

        PopUp.PlayGrow(Panel);

        // 장착, 해제, 교체 버튼
        WearButton.onClick.RemoveAllListeners();
        WearButton.onClick.AddListener(() =>
        {
            SoundManager.Instance.PlaySFX("Button");

            HandleRuneEquip(
                CharacterInventoryManager.Instance._equippedRunes,
                runeInstance,
                () => CharacterInventoryManager.Instance.CharacterRuneUnwear(),
                () => CharacterInventoryManager.Instance.CharacterRuneWear(runeInstance)
            );
            runeInstance.IsEquipped = !runeInstance.IsEquipped; // 장착 상태 토글
            HideInfo();
        });

        TextRuneEqulp(CharacterInventoryManager.Instance._equippedRunes, runeInstance);

        SellButton.onClick.RemoveAllListeners();
        SellButton.onClick.AddListener(() => SellRune(runeInstance));
    }

#endregion

    private void SellRune(RuneInstance runeInstance)
    {
        int sellGold = 3000;

        // 판매시 슬롯에서 장착해제
        CharacterInventoryManager.Instance.RemoveItem(runeInstance);
        // 판매시 인벤토리 리스트에서 삭제
        InventoryManager.Instance.RemoveRuneItem(runeInstance);

        // 골드 지급
        GameManager.Instance.GetMoney(sellGold); // 골드 지급 방식은 프로젝트에 따라 다름

        // UI 업데이트
        InventoryUIManager.RefreshInventory();

        // 패널 숨기기
        HideInfo();
    }

    // 텍스트 아이템 교체, 해제, 장착
    private void TextEqulp(ItemInstance equipped, ItemInstance clicked)
    {
        if (equipped == null)
        {
            WearText.text = "장착하기";
        }
        else if (ReferenceEquals(equipped, clicked) || equipped == clicked)
        {
            WearText.text = "해제하기";
        }
        else if (equipped.ItemSo != null && clicked.ItemSo != null && equipped.ItemSo == clicked.ItemSo)
        {
            // 같은 아이템이면 해제
            WearText.text = "교체하기";
        }
        else
        {
            WearText.text = "교체하기";
        }
    }

    private void TextRuneEqulp(RuneInstance equipped, RuneInstance clicked)
    {
        if (equipped == null)
        {
            WearText.text = "장착하기";
        }
        else if (ReferenceEquals(equipped, clicked) || equipped == clicked)
        {
            WearText.text = "해제하기";
        }
        else if (equipped.RuneSo != null && clicked.RuneSo != null && equipped.RuneSo == clicked.RuneSo)
        {
            // 같은 아이템이면 해제
            WearText.text = "교체하기";
        }
        else
        {
            WearText.text = "교체하기";
        }
    }

    // 아이템 장착, 해제, 교체
    public void HandleEquip(ItemInstance equipped, ItemInstance clicked, Action unwearAction, Action wearAction)
    {
        if (equipped == null)
        {
            wearAction();
            InventoryUIManager.UpdateEquippedSlot(clicked, true);
        }
        else if (equipped == clicked)
        {
            unwearAction();
            InventoryUIManager.UpdateEquippedSlot(equipped, false);
        }
        else
        {
            unwearAction();
            InventoryUIManager.UpdateEquippedSlot(equipped, false);
            wearAction();
            InventoryUIManager.UpdateEquippedSlot(clicked, true);
        }
    }

    private void HandleRuneEquip(RuneInstance equipped, RuneInstance clicked, Action unwearAction, Action wearAction)
    {
        if (equipped == null)
        {
            wearAction();
            InventoryUIManager.UpdateEquippedRuneSlot(clicked, true);
        }
        else if (equipped == clicked)
        {
            unwearAction();
            InventoryUIManager.UpdateEquippedRuneSlot(equipped, false);
        }
        else
        {
            unwearAction();
            InventoryUIManager.UpdateEquippedRuneSlot(equipped, false);
            wearAction();
            InventoryUIManager.UpdateEquippedRuneSlot(clicked, true);
        }
    }

    // 등급별 배경색 지정
    private Dictionary<Grade, Color> _gradeColors = new Dictionary<Grade, Color>()
    {
        { Grade.Legendary, new Color32(237, 171, 95, 255) },
        { Grade.Rare, new Color32(190, 119, 231, 255) },
        { Grade.Uncommon, new Color32(87, 152, 241, 255) },
        { Grade.Common, new Color32(114, 143, 115, 255) }
    };

    public void HideInfo()
    {
        SoundManager.Instance.PlaySFX("Button");

        PopDown.PlayShrink(Panel);
    }
}