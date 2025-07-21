using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SyntheticManager : MonoBehaviour
{
    List<ItemSlot> slots = new List<ItemSlot>();

    public SyntheticUIManager SyntheticUIManager;

    [Header("SyntheticUI")]
    public Image SyntheticItemIconOne;
    public Image SyntheticItemIconTwo;

    public Image SyntheticBackGroundOne;
    public Image SyntheticBackGroundTwo;

    public TMP_Text SyntheticItemTextOne;
    public TMP_Text SyntheticItemTextTwo;

    public GameObject InventorySlotPrefab;
    public Sprite ItemIcon;

    public Button SyntheticButton;
    public TMP_Text SyntheticButtonText;

    public TMP_Text PlayerMoneyText;
    public TMP_Text ProbabilityText;
    public TMP_Text CostMoneyText;
    public TMP_Text WarningText;

    // 각 강화 데이터 저장 변수
    private ItemInstance _selectedItemOne;
    private ItemInstance _selectedItemTwo;

    private void Start()
    {
        SyntheticItemIconOne.GetComponent<Button>().onClick.AddListener(ClearSynthesisUI);
        SyntheticButton.onClick.AddListener(DoSynthesis);
    }

    private void Update()
    {
        PlayerMoneyText.text = GameManager.Instance.GoldNum.ToString();
    }

    public void StartSynthat(ItemInstance itemInstance)
    {
        SyntheticUIManager.ClearContents();

        var item = itemInstance.ItemSo;

        // 처음 강화칸 업로드
        SyntheticItemIconOne.sprite = item.ItemImage;

        SyntheticItemTextOne.text = $"LV {itemInstance.EnhanceLevel}";

        SoundManager.Instance.PlaySFX("Button");

        // 첫번째 강화 장비 저장
        _selectedItemOne = itemInstance;

        if (_gradeColors.TryGetValue(item.grade, out var color))
            SyntheticBackGroundOne.color = color;

        if (!int.TryParse(itemInstance.ItemSo.ID, out int baseId)) return;

        // 현재 아이템과 같은 ID, 같은 레벨을 가진 아이템만 가져옴
        var sameItems = InventoryManager.Instance.InventoryItems
            .FindAll(item =>
            {
                return int.TryParse(item.ItemSo.ID, out int idNum) && idNum == baseId && item.EnhanceLevel == itemInstance.EnhanceLevel && item != itemInstance && item.IsEquipped == false;
            });

        // 같은 슬롯 타입 찾기
        Transform targetParent = null;

        if (baseId >= 0 && baseId < 100)
            targetParent = SyntheticUIManager.WeaponContent;
        else if (baseId >= 101 && baseId < 200)
            targetParent = SyntheticUIManager.TopContent;
        else if (baseId >= 201 && baseId < 300)
            targetParent = SyntheticUIManager.BottomContent;
        else if (baseId >= 301 && baseId < 400)
            targetParent = SyntheticUIManager.GlovesContent;
        else if (baseId >= 401 && baseId < 500)
            targetParent = SyntheticUIManager.ShoesContent;

        if (targetParent == null) return;

        foreach (var items in sameItems)
        {
            GameObject iconObj = Instantiate(InventorySlotPrefab, targetParent);
            ItemSlot slot = iconObj.GetComponent<ItemSlot>();
            Button btn = iconObj.GetComponent<Button>();

            slot.Setup(items);
            slot.SetUpEquipped(false);

            // 이 아이템을 클릭하면 SyntheticItemIconTwo에 이미지 넣기
            btn.onClick.AddListener(() =>
            {
                SoundManager.Instance.PlaySFX("Button");

                foreach (var s in slots)
                {
                    s.SetUpEquipped(false);
                }

                slot.SetUpEquipped(true);

                // 두번째 강화 장비 저장
                _selectedItemTwo = items;

                SyntheticItemIconTwo.sprite = items.ItemSo.ItemImage;
                SyntheticItemTextTwo.text = $"LV {items.EnhanceLevel}";

                if (_gradeColors.TryGetValue(item.grade, out var color))
                    SyntheticBackGroundTwo.color = color;

                // 확률 텍스트 표시
                if(_selectedItemOne.EnhanceLevel == _selectedItemTwo.EnhanceLevel)
                {
                    ProbabilityTexts(_selectedItemOne.EnhanceLevel);
                    CostMoneyTexts(_selectedItemOne.EnhanceLevel);
                }
                else
                {
                    CostMoneyText.text = "비용 : 0";
                    ProbabilityText.text = "확률 : 0";
                }
            });

            slots.Add(slot);
        }
    }
    
    // 첫번째 강화 아이콘 누르면 초기화
    private void ClearSynthesisUI()
    {
        SoundManager.Instance.PlaySFX("Button");

        // 아이콘 및 배경 초기화
        SyntheticItemIconOne.sprite = ItemIcon;
        SyntheticItemIconTwo.sprite = ItemIcon;
        SyntheticBackGroundOne.color = Color.clear;
        SyntheticBackGroundTwo.color = Color.clear;

        // 기존 슬롯 제거
        foreach (var slot in slots)
        {
            if (slot != null)
                Destroy(slot.gameObject);
        }
        slots.Clear();

        CostMoneyText.text = "비용 : 0";
        ProbabilityText.text = "확률 : 0";
        SyntheticUIManager.RefreshInventory();
    }

    // 강화 버튼 동작
    private void DoSynthesis()
    {
        SoundManager.Instance.PlaySFX("Button");

        // 합성 아이템 선택이 안될떄
        if (_selectedItemOne == null || _selectedItemTwo == null)
        {
            SyntheticButtonText.color = Color.red;
            Invoke(nameof(SyntheticButtonTextColor), 0.3f);

            return;
        }

        // 합성 아이텝이 다를때
        if(_selectedItemOne.ItemSo.ID != _selectedItemTwo.ItemSo.ID)
        {
            SyntheticButtonText.color = Color.red;
            Invoke(nameof(SyntheticButtonTextColor), 0.3f);

            return;
        }

        // 합성 아이템 레벨이 다를때(확인할떄는 !=)
        if(_selectedItemOne.EnhanceLevel != _selectedItemTwo.EnhanceLevel)
        {
            SyntheticButtonText.color = Color.red;
            Invoke(nameof(SyntheticButtonTextColor), 0.3f);

            return;
        }

        // 비용 지불
        if(GameManager.Instance.GoldNum <= (Cost(_selectedItemOne.EnhanceLevel)))
        {
            SyntheticButtonText.color = Color.red;
            Invoke(nameof(SyntheticButtonTextColor), 0.3f);
            return;
        }
        GameManager.Instance.LoseMoney(Cost(_selectedItemOne.EnhanceLevel));

        // 강화 확률
        int itemProbability = Random.Range(1, 101);

        if(itemProbability != Probability(_selectedItemOne.EnhanceLevel))
        {
            WarningText.color = Color.green;
            WarningText.text = "합성에 성공하였습니다.";
            InventoryManager.Instance.AddItem(_selectedItemOne.ItemSo, _selectedItemOne.EnhanceLevel+1);

            InventoryManager.Instance.InventoryItems.Remove(_selectedItemOne);
            InventoryManager.Instance.InventoryItems.Remove(_selectedItemTwo);

            _selectedItemOne = null;
            _selectedItemTwo = null;

            InventoryUIManager.Instance.RefreshInventory();
        }
        else
        {
            Debug.Log("ㅋㅋㅋㅋㅋ 터짐요 ㄹㅈㄷ~~");
            WarningText.color = Color.red;
            WarningText.text = "합성에 실패하였습니다.";
            InventoryManager.Instance.InventoryItems.Remove(_selectedItemOne);
            InventoryManager.Instance.InventoryItems.Remove(_selectedItemTwo);
        }

        InventoryManager.Instance.InventoryItems.Remove(_selectedItemOne);
        InventoryManager.Instance.InventoryItems.Remove(_selectedItemTwo);

        ClearSynthesisUI();
    }

    // 레벨당 강화 확률
    private int Probability (int EnhanceLevel)
    {
        return EnhanceLevel switch
        {
            0 => 100,
            1 => 70,
            2 => 65,
            3 => 55,
            4 => 45,
            5 => 35,
            6 => 25,
            7 => 15,
            8 => 10,
            9 => 5,
            10 => 1,
            _ => 0
        };
    }

    // 레벨당 강화 비용
    private int Cost(int EnhanceLevel)
    {
        return EnhanceLevel switch
        {
            0 => 500,
            1 => 1000,
            2 => 2500,
            3 => 3500,
            4 => 4500,
            5 => 5500,
            6 => 6500,
            7 => 7500,
            8 => 8500,
            9 => 9500,
            10 => 10000,
            _ => 0
        };
    }

    // 등급별 배경색 지정
    private Dictionary<Grade, Color> _gradeColors = new Dictionary<Grade, Color>()
    {
        { Grade.Legendary, new Color32(237, 171, 95, 255) },
        { Grade.Rare, new Color32(190, 119, 231, 255) },
        { Grade.Uncommon, new Color32(87, 152, 241, 255) },
        { Grade.Common, new Color32(114, 143, 115, 255) }
    };

    private void SyntheticButtonTextColor()
    {
        SyntheticButtonText.color = new Color32(49, 98, 137, 255);
    }

    private void ProbabilityTexts(int EnhanceLevel)
    {
        ProbabilityText.text = $"확률 : {Probability(EnhanceLevel)}";
    }

    private void CostMoneyTexts(int EnhanceLevel)
    {
        CostMoneyText.text = $"비용 : {Cost(EnhanceLevel)}";
    }
}