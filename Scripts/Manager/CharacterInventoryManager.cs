using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterInventoryManager : Singleton<CharacterInventoryManager>
{
    public ItemInfoPanelManager itemInfoPanelManager;

    [Header("Default Image")]
    public Sprite NormalImage;

    // 장비
    [Header("Inventory Slots")]
    public ItemSlot WeaponSlot;
    public ItemSlot TopSlot;
    public ItemSlot BottomSlot;
    public ItemSlot ShoesSlot;
    public ItemSlot GlovesSlot;
    public ItemSlot RuneSlot;

    [Header("ItemWearNull")]
    public ItemInstance _equippedWeapon;
    public ItemInstance _equippedTop;
    public ItemInstance _equippedBottom;
    public ItemInstance _equippedShoes;
    public ItemInstance _equippedGloves;
    public RuneInstance _equippedRunes;

    [Header("Image")]
    public Image WeaponImage;
    public Image TopImage;
    public Image BottomImage;
    public Image ShoesImage;
    public Image GlovesImage;
    public Image RuneImage;

    [Header("ImageBackGround")]
    public Image WeaponBackGround;
    public Image TopBackGround;
    public Image BottomBackGround;
    public Image ShoesBackGround;
    public Image GlovesBackGround;
    public Image RuneBackGround;

    [Header("Weapon")]
    public float WeaponAtkBonus = 0f;
    public float WeaponSkillBonus = 0f;

    [Header("Top")]
    public float TopDefenseBonus = 0;
    public float TopHpRegenBonus = 0;
    public float TopMpRegenBonus = 0;

    [Header("Shoes")]
    public float ShoesMoveBonus = 0;

    [Header("Gloves")]
    public float GlovesAttackSpeedBonus = 0f;

    [Header("Bottom")]
    public float BottomDefenseBonus = 0f;
    public float BottomHpBonus = 0f;
    public float BottomMpBonus = 0f;

    // 룬
    [Header("Rune of Shadows")]
    public float CriChanceBonus = 0f;
    public float CriDamageBonus = 0f;

    [Header("Rune of the Earth")]
    public float DefenseBonus = 0f;
    public float HealthBouns = 0f;
    public float ManaBouns = 0f;

    [Header("Rune of Fortune")]
    public float GoldBonus = 0f;

    [Header("Rune of Rage")]
    public float AtkDamageBonus = 0f;
    public float SkillDamageBonus = 0f;

    [Header("Rune of Life")]
    public float HealthRepBonus = 0f;
    public float ManaRepBonus = 0f;

    [Header("Rune of Swiftness")]
    public float AtkSpeedBonus = 0f;
    public float MoveSpeedBonus = 0f;


    #region GetFinalStat

    public float GetFinalStat(StatType type)
    {
        float baseStat = StatUpgradeManager.Instance.GetBaseStat(type);
        float upgradeStat = StatUpgradeManager.Instance.GetUpgradeValue(type);
        float equipmentBonus = GetEquipmentBonus(type);
        float runeBonus = GetRuneBonus(type);

        return baseStat + upgradeStat + equipmentBonus + runeBonus;
    }

    public float GetEquipmentBonus(StatType type)
    {
        float bonusPercent = 0f;

        switch (type)
        {
            case StatType.Attack:
                bonusPercent = WeaponAtkBonus;
                break;
            case StatType.SkillDamage:
                bonusPercent = WeaponSkillBonus;
                break;
            case StatType.Defense:
                bonusPercent = TopDefenseBonus + BottomDefenseBonus;
                break;
            case StatType.HpRegen:
                bonusPercent = TopHpRegenBonus;
                break;
            case StatType.MpRegen:
                bonusPercent = TopMpRegenBonus;
                break;
            case StatType.AttackSpeed:
                bonusPercent = GlovesAttackSpeedBonus;
                break;
            case StatType.MoveSpeed:
                bonusPercent = ShoesMoveBonus;
                break;
            case StatType.MaxHp:
                bonusPercent = BottomHpBonus;
                break;
            case StatType.MaxMp:
                bonusPercent = BottomMpBonus;
                break;
        }

        // 장비 보정은 "업그레이드 포함값"에 % 적용
        float upgradedStat = StatUpgradeManager.Instance.GetBaseStat(type) +
                             StatUpgradeManager.Instance.GetUpgradeValue(type);

        return upgradedStat * (bonusPercent * 0.01f);
    }

    public float GetRuneBonus(StatType type)
    {
        float bonusPercent = 0f;

        switch (type)
        {
            case StatType.Attack:
                bonusPercent = AtkDamageBonus;
                break;
            case StatType.SkillDamage:
                bonusPercent = SkillDamageBonus;
                break;
            case StatType.Defense:
                bonusPercent = DefenseBonus;
                break;
            case StatType.HpRegen:
                bonusPercent = HealthRepBonus;
                break;
            case StatType.MpRegen:
                bonusPercent = ManaRepBonus;
                break;
            case StatType.AttackSpeed:
                bonusPercent = AtkSpeedBonus;
                break;
            case StatType.MoveSpeed:
                bonusPercent = MoveSpeedBonus;
                break;
            case StatType.MaxHp:
                bonusPercent = HealthBouns;
                break;
            case StatType.MaxMp:
                bonusPercent = ManaBouns;
                break;
            case StatType.CritChance:
                bonusPercent = CriChanceBonus;
                break;
            case StatType.CritDamage:
                bonusPercent = CriDamageBonus;
                break;
            case StatType.GoldGain:
                bonusPercent = GoldBonus;
                break;

        }

        // 장비 보정은 "업그레이드 포함값"에 % 적용
        float upgradedStat = StatUpgradeManager.Instance.GetBaseStat(type) +
                             StatUpgradeManager.Instance.GetUpgradeValue(type);

        return upgradedStat * (bonusPercent * 0.01f);
    }
    #endregion

    // 장비창 장착 아이템 UI 반환 및 스텟 반환
    public void CharacterItemWear(ItemInstance itemInstance)
    {
        var item = itemInstance.ItemSo;

        if (item is WaponDataSo wapon)
        {
            // 이미지
            WeaponImage.sprite = wapon.ItemImage;
            // 이미지 배경
            if (_gradeColors.TryGetValue(item.grade, out var color))
                WeaponBackGround.color = color;
            // 옵션
            WeaponAtkBonus = wapon.AtkDamage * (itemInstance.EnhanceLevel + 1);
            WeaponSkillBonus = wapon.SkillDamage * (itemInstance.EnhanceLevel + 1);

            Button btn = WeaponImage.GetComponent<Button>();
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => OnSlotButtonClick(itemInstance));

            _equippedWeapon = itemInstance;
            itemInstance.IsEquipped = true;

            PlayerStats.Instance.ApplyUpgradedStats(StatUpgradeManager.Instance, this);
            StatUpgradeManager.Instance.UpdateUI();
        }
        else if (item is ArmorTopDataSo Top)
        {
            TopImage.sprite = Top.ItemImage;

            if (_gradeColors.TryGetValue(item.grade, out var color))
                TopBackGround.color = color;

            TopDefenseBonus = Top.Defense * (itemInstance.EnhanceLevel + 1);
            TopHpRegenBonus = Top.HpRegen * (itemInstance.EnhanceLevel + 1);
            TopMpRegenBonus = Top.MpRegen * (itemInstance.EnhanceLevel + 1);

            Button btn = TopImage.GetComponent<Button>();
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => OnSlotButtonClick(itemInstance));

            _equippedTop = itemInstance;
            itemInstance.IsEquipped = true;

            PlayerStats.Instance.ApplyUpgradedStats(StatUpgradeManager.Instance, this);
            StatUpgradeManager.Instance.UpdateUI();
        }
        else if (item is ArmorShoesDataSo Shoes)
        {
            ShoesImage.sprite = Shoes.ItemImage;

            if (_gradeColors.TryGetValue(item.grade, out var color))
                ShoesBackGround.color = color;

            ShoesMoveBonus = Shoes.MoveSpeed * (itemInstance.EnhanceLevel + 1);

            Button btn = ShoesImage.GetComponent<Button>();
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => OnSlotButtonClick(itemInstance));

            _equippedShoes = itemInstance;
            itemInstance.IsEquipped = true;

            PlayerStats.Instance.ApplyUpgradedStats(StatUpgradeManager.Instance, this);
            StatUpgradeManager.Instance.UpdateUI();
        }
        else if (item is ArmorGlovesDataSo Glove)
        {
            GlovesImage.sprite = Glove.ItemImage;

            if (_gradeColors.TryGetValue(item.grade, out var color))
                GlovesBackGround.color = color;

            GlovesAttackSpeedBonus = Glove.AttackSpeede * (itemInstance.EnhanceLevel + 1);

            Button btn = GlovesImage.GetComponent<Button>();
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => OnSlotButtonClick(itemInstance));

            _equippedGloves = itemInstance;
            itemInstance.IsEquipped = true;

            PlayerStats.Instance.ApplyUpgradedStats(StatUpgradeManager.Instance, this);
            StatUpgradeManager.Instance.UpdateUI();
        }
        else if (item is ArmorBottomDataSo Bottom)
        {
            BottomImage.sprite = Bottom.ItemImage;

            if (_gradeColors.TryGetValue(item.grade, out var color))
                BottomBackGround.color = color;

            BottomDefenseBonus = Bottom.Defense * (itemInstance.EnhanceLevel + 1);
            BottomHpBonus = Bottom.Hp * (itemInstance.EnhanceLevel + 1);
            BottomMpBonus = Bottom.Mp * (itemInstance.EnhanceLevel + 1);

            Button btn = BottomImage.GetComponent<Button>();
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => OnSlotButtonClick(itemInstance));

            _equippedBottom = itemInstance;
            itemInstance.IsEquipped = true;

            PlayerStats.Instance.ApplyUpgradedStats(StatUpgradeManager.Instance, this);
            StatUpgradeManager.Instance.UpdateUI();
        }


        InventoryManager.Instance.UpdateInventoryToBackend();
        // 인벤토리 UI 및 합성 UI 갱신
        InventoryUIManager.Instance.RefreshInventory();
        SyntheticUIManager.Instance.RefreshInventory();
    }

    #region CharacterRuneWear

    public void CharacterRuneWear(RuneInstance runeInstance)
    {
        RuneImage.sprite = runeInstance.RuneSo.ItemImage;
        RuneBackGround.color = new Color32(221, 160, 221, 255);

        if (runeInstance.RuneSo is IRuneEffect effect)
            effect.ApplyEffect(this);

        Button btn = RuneImage.GetComponent<Button>();
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() => OnRuneSlotButtonClick(runeInstance));

        _equippedRunes = runeInstance;
        runeInstance.IsEquipped = true;

        PlayerStats.Instance.ApplyUpgradedStats(StatUpgradeManager.Instance, this);
        InventoryManager.Instance.UpdateInventoryToBackend();
        StatUpgradeManager.Instance.UpdateUI();

    }

    #endregion

    #region ItemUnWear

    // 장착 아이템 해제(무기, 상의, 하의, 신발, 장갑)
    // 무기
    public void CharacterWaponItemUnwear()
    {
        // 이미지 제거 또는 투명 이미지 설정
        WeaponImage.sprite = NormalImage;   // DefaultWeaponSprite는 기본 이미지 (null 가능)
        WeaponBackGround.color = Color.clear;

        // 능력치 초기화
        WeaponAtkBonus = 0;
        WeaponSkillBonus = 0;

        // 버튼 이벤트 제거
        Button btn = WeaponImage.GetComponent<Button>();
        btn.onClick.RemoveAllListeners();

        _equippedWeapon.IsEquipped = false;
        _equippedWeapon = null;

        PlayerStats.Instance.ApplyUpgradedStats(StatUpgradeManager.Instance, this);
        StatUpgradeManager.Instance.UpdateUI();
        InventoryUIManager.Instance.RefreshInventory();

        InventoryManager.Instance.UpdateInventoryToBackend();

    }

    // 상의
    public void CharacterTopItemUnwear()
    {
        TopImage.sprite = NormalImage;
        TopBackGround.color = Color.clear;

        TopDefenseBonus = 0;
        TopHpRegenBonus = 0;
        TopMpRegenBonus = 0;

        Button btn = TopImage.GetComponent<Button>();
        btn.onClick.RemoveAllListeners();

        _equippedTop.IsEquipped = false;
        _equippedTop = null;

        PlayerStats.Instance.ApplyUpgradedStats(StatUpgradeManager.Instance, this);
        StatUpgradeManager.Instance.UpdateUI();
        InventoryUIManager.Instance.RefreshInventory();

        InventoryManager.Instance.UpdateInventoryToBackend();

    }

    // 하의
    public void CharacterBottomItemUnwear()
    {
        BottomImage.sprite = NormalImage;
        BottomBackGround.color = Color.clear;

        BottomDefenseBonus = 0;
        BottomHpBonus = 0;
        BottomMpBonus = 0;

        Button btn = BottomImage.GetComponent<Button>();
        btn.onClick.RemoveAllListeners();

        _equippedBottom.IsEquipped = false;
        _equippedBottom = null;

        PlayerStats.Instance.ApplyUpgradedStats(StatUpgradeManager.Instance, this);
        StatUpgradeManager.Instance.UpdateUI();
        InventoryUIManager.Instance.RefreshInventory();

        InventoryManager.Instance.UpdateInventoryToBackend();

    }

    // 신발
    public void CharacterShoesItemUnwear()
    {
        ShoesImage.sprite = NormalImage;
        ShoesBackGround.color = Color.clear;

        ShoesMoveBonus = 0;

        Button btn = ShoesImage.GetComponent<Button>();
        btn.onClick.RemoveAllListeners();

        _equippedShoes.IsEquipped = false;
        _equippedShoes = null;

        PlayerStats.Instance.ApplyUpgradedStats(StatUpgradeManager.Instance, this);
        StatUpgradeManager.Instance.UpdateUI();
        InventoryUIManager.Instance.RefreshInventory();

        InventoryManager.Instance.UpdateInventoryToBackend();

    }

    // 장갑
    public void CharacterGlovesItemUnwear()
    {
        GlovesImage.sprite = NormalImage;
        GlovesBackGround.color = Color.clear;

        GlovesAttackSpeedBonus = 0;

        Button btn = GlovesImage.GetComponent<Button>();
        btn.onClick.RemoveAllListeners();

        _equippedGloves.IsEquipped = false;
        _equippedGloves = null;

        PlayerStats.Instance.ApplyUpgradedStats(StatUpgradeManager.Instance, this);
        StatUpgradeManager.Instance.UpdateUI();
        InventoryUIManager.Instance.RefreshInventory();

        InventoryManager.Instance.UpdateInventoryToBackend();

    }

    #endregion

    #region RuneUnwear

    // 룬 장착 해제(그림자, 대지, 행운, 분노, 생명, 신속)
    public void CharacterRuneUnwear()
    {
        RuneImage.sprite = NormalImage;
        RuneBackGround.color = Color.clear;

        if (_equippedRunes?.RuneSo is IRuneEffect effect)
            effect.RemoveEffect(this);

        Button btn = RuneImage.GetComponent<Button>();
        btn.onClick.RemoveAllListeners();

        _equippedRunes.IsEquipped = false;
        _equippedRunes = null;

        PlayerStats.Instance.ApplyUpgradedStats(StatUpgradeManager.Instance, this);
        StatUpgradeManager.Instance.UpdateUI();
        InventoryUIManager.Instance.RefreshInventory();

        InventoryManager.Instance.UpdateInventoryToBackend();

    }

    #endregion

    // 인벤토리 제거
    public void RemoveItem(RuneInstance item)
    {
        if (_equippedRunes == item)
            CharacterRuneUnwear();
        InventoryManager.Instance.UpdateInventoryToBackend();

    }

    // 등급별 배경색 지정
    private Dictionary<Grade, Color> _gradeColors = new Dictionary<Grade, Color>()
    {
        { Grade.Legendary, new Color32(237, 171, 95, 255) },
        { Grade.Rare, new Color32(190, 119, 231, 255) },
        { Grade.Uncommon, new Color32(87, 152, 241, 255) },
        { Grade.Common, new Color32(114, 143, 115, 255) }
    };

    private void OnSlotButtonClick(ItemInstance itemInstance)
    {
        SoundManager.Instance.PlaySFX("Button");

        itemInfoPanelManager.ShowInfo(itemInstance);
    }

    private void OnRuneSlotButtonClick(RuneInstance runeInstance)
    {
        SoundManager.Instance.PlaySFX("Button");

        itemInfoPanelManager.RuneShowInfo(runeInstance);
    }
}