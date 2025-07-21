using BackEnd;
using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class DrawingPanelManager : Singleton<DrawingPanelManager>
{
    [SerializeField] private Material _sparkleMaterial;

    // 풀링
    private Queue<Material> _sparkleMaterialPool = new Queue<Material>();
    private Queue<GameObject> _drawingSlotPool = new Queue<GameObject>();

    public GachaDatabaseSO GachaDatabaseSO;

    [Header("Script")]
    public FloatingText FloatingText;

    [Header("Obj")]
    public GameObject DrawingPanel;
    public GameObject DrawingSlotPrefab;
    public Transform DrawingContent;
    public List<Button> IgnoreButtons;

    [Header("LV")]
    public Image LvLine;
    public TMP_Text LvLineNum;
    public TMP_Text LvText;
    private int _drawLevel = 1;         // 현재 뽑기 레벨
    private int _currentEXP = 0;        // 현재 필요한 경험치

    private bool _uiTrue = false;

    // 등급별 확률(레벨별로 확률 오름)
    private Dictionary<Grade, float> _gradeChances = new();
    private void Start()
    {
        UpdateChancesByLevel(_drawLevel);

        InitMaterialPool(30);
        InitDrawingSlotPool(30);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && _uiTrue == false)
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                if (IsClickingOnAnyIgnoreButton())

                    return;
            }

            DrawingPanel.SetActive(false);
        }
    }

    public IEnumerator Draw(int count, int Gp, float delay = 0.5f)     // 1, 10, 30 번뽑기
    {
        // 뽑기 시작시 버튼 및 패널 비활성화
        _uiTrue = true;
        SetDrawButton(false);

        if (GameManager.Instance.GpNum < Gp)
        {
            FloatingText.Show("-GP가 부족합니다.-");
            SetDrawButton(false);
            _uiTrue = false;

            yield break;
        }

        foreach (Transform i in DrawingContent)
        {
            ReturnDrawingSlotToPool(i.gameObject);
        }

        DrawingPanel.SetActive(false);
        DrawingPanel.SetActive(true);

        GameManager.Instance.LoseGp(Gp);

        for (int i = 0; i < count; i++)
        {
            Grade resultGrade = RollGrade(); // 확률에 따라 뽑기
            List<ItemBase> items = GachaDatabaseSO.GetItemsByGrade(resultGrade); // 뽑힌 등급에 있는 아이템 리스트

            if (items.Count > 0)
            {
                ItemBase selected = items[Random.Range(0, items.Count)]; // 리스트에서 랜덤

                //GameObject drawingIconObj = Instantiate(DrawingSlotPrefab, DrawingContent);
                GameObject drawingIconObj = GetDrawingSlotFromPool();
                drawingIconObj.transform.SetParent(DrawingContent); // 재부착


                // 아이콘 이미지 설정
                Image iconImage = drawingIconObj.transform.Find("Icon").GetComponent<Image>();
                iconImage.sprite = selected.ItemImage;

                Image MeterialImage = drawingIconObj.transform.Find("Shader Line").GetComponent<Image>();

                // 머데리얼 적용
                Material sparkleMatInstance = GetMaterialFromPool();
                MeterialImage.material = sparkleMatInstance;

                // 시작 시간과 지속 시간 설정
                float startTime = Time.timeSinceLevelLoad;
                float duration = 0.3f; // 반짝이 지속 시간 (1초)
                sparkleMatInstance.SetFloat("_StartTime", startTime);
                sparkleMatInstance.SetFloat("_Duration", duration);

                // 배경 이미지와 색상 설정
                Image background = drawingIconObj.transform.Find("Background").GetComponent<Image>();
                Color bgColor = _gradeColors.ContainsKey(selected.grade) ? _gradeColors[selected.grade] : Color.white;
                background.color = bgColor;

                // 인벤토리에 뽑은 아이템 추가 (아이템, 레벨)
                InventoryManager.Instance.AddItem(selected, 0);

                // 처음 얻는 아이템이면 도감에 해금
                CollectionManager.Instance.UnlockItem(selected);
            }

            AddExp(5); // 뽑기 한번당 5경험치

            yield return new WaitForSeconds(delay);

            UpdateGachaDataToBackend();
        }

        //뽑기 끝날 시 버튼 및 패널 활성화
        _uiTrue = false;
        SetDrawButton(true);
    }

    // 레벨별 뽑기 확률
    private Grade RollGrade()
    {
        float roll = Random.Range(0.0f, 100.0f);
        float total = 0.0f;

        // 현재 레벨에 맞는 확률
        foreach (var pair in _gradeChances)
        {
            total += pair.Value;
            if (roll <= total)
                return pair.Key;
        }

        // 혹시 확률 합이 100이 아니면 기본값
        return Grade.Common;
    }

    private void AddExp(int amount)
    {
        _currentEXP += amount;
        int expRequired = GetExpToNextLevel(_drawLevel);

        while (_currentEXP >= expRequired && _drawLevel < 10)
        {
            _currentEXP -= expRequired; // 초과 경험치 갱신을 위해서
            _drawLevel++;               // 레벨업

            UpdateChancesByLevel(_drawLevel);

            Debug.Log($"레벨업! {_drawLevel}");

            expRequired = GetExpToNextLevel(_drawLevel);    // 다음 레벨의 경험치 요구량 갱신
        }

        LvLine.fillAmount = (float)_currentEXP / GetExpToNextLevel(_drawLevel); // 레벨 바 갱신
        LvText.text = _drawLevel.ToString();    //레벨 업 텍스트
        LvLineNum.text = $"{GetExpToNextLevel(_drawLevel)}/{_currentEXP}";
    }

    // 등급별 배경색 지정
    private Dictionary<Grade, Color> _gradeColors = new Dictionary<Grade, Color>()
    {
        { Grade.Legendary, new Color32(237, 171, 95, 255) },
        { Grade.Rare, new Color32(190, 119, 231, 255) },
        { Grade.Uncommon, new Color32(87, 152, 241, 255) },
        { Grade.Common, new Color32(114, 143, 115, 255) }
    };

    // 레벨업 별 경헙치
    private int GetExpToNextLevel(int level)
    {
        return level switch
        {
            1 => 100,
            2 => 150,
            3 => 200,
            4 => 300,
            5 => 400,
            6 => 500,
            7 => 600,
            8 => 700,
            9 => 850,
            10 => 1000,
            _ => 99999
        };
    }

    // 뽑기 레벨별 당 등급확률 
    public void UpdateChancesByLevel(int level)
    {
        if (level == 1)
            _gradeChances = new() { { Grade.Common, 85f }, { Grade.Uncommon, 15 }, { Grade.Rare, 0f }, { Grade.Legendary, 0f } };
        else if (level == 2)
            _gradeChances = new() { { Grade.Common, 83.4f }, { Grade.Uncommon, 16.5f }, { Grade.Rare, 0.1f }, { Grade.Legendary, 0f } };
        else if (level == 3)
            _gradeChances = new() { { Grade.Common, 80.9f }, { Grade.Uncommon, 18.59f }, { Grade.Rare, 0.5f }, { Grade.Legendary, 0.01f } };
        else if (level == 4)
            _gradeChances = new() { { Grade.Common, 78f }, { Grade.Uncommon, 20.95f }, { Grade.Rare, 1f }, { Grade.Legendary, 0.05f } };
        else if (level == 5)
            _gradeChances = new() { { Grade.Common, 74f }, { Grade.Uncommon, 23.9f }, { Grade.Rare, 2f }, { Grade.Legendary, 0.1f } };
        else if (level == 6)
            _gradeChances = new() { { Grade.Common, 72.5f }, { Grade.Uncommon, 24f }, { Grade.Rare, 3f }, { Grade.Legendary, 0.5f } };
        else if (level == 7)
            _gradeChances = new() { { Grade.Common, 68f }, { Grade.Uncommon, 27f }, { Grade.Rare, 4f }, { Grade.Legendary, 1f } };
        else if (level == 8)
            _gradeChances = new() { { Grade.Common, 64.2f }, { Grade.Uncommon, 27.3f }, { Grade.Rare, 6.5f }, { Grade.Legendary, 2f } };
        else if (level == 9)
            _gradeChances = new() { { Grade.Common, 60.8f }, { Grade.Uncommon, 28.6f }, { Grade.Rare, 7.6f }, { Grade.Legendary, 3f } };
        else if (level == 10)
            _gradeChances = new() { { Grade.Common, 58.5f }, { Grade.Uncommon, 29f }, { Grade.Rare, 8.5f }, { Grade.Legendary, 4f } };
    }

    private bool IsClickingOnAnyIgnoreButton()
    {
        // Raycast로 마우스 위치의 모든 UI 오브젝트 가져옴
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();

        // 첫 번째 버튼 기준으로 부모 Canvas의 Raycaster 사용
        if (IgnoreButtons.Count == 0 || IgnoreButtons[0] == null)
            return false;

        GraphicRaycaster raycaster = IgnoreButtons[0].GetComponentInParent<Canvas>().GetComponent<GraphicRaycaster>();
        raycaster.Raycast(pointerData, results);

        foreach (var result in results)
        {
            foreach (var button in IgnoreButtons)
            {
                if (result.gameObject == button.gameObject)
                    return true;
            }
        }

        return false;
    }

    // 버튼 활성 / 비활성화
    private void SetDrawButton(bool interactable)
    {
        foreach (var btn in IgnoreButtons)
        {
            btn.interactable = interactable;
        }
    }

    // DrawInfor에서 참조용
    public int CurrentDrawLevel => _drawLevel;

    public Dictionary<Grade, float> GetcurrentGradeChances(int level)
    {
        Dictionary<Grade, float> chances = level switch
        {
            1 => new() { { Grade.Common, 85f }, { Grade.Uncommon, 15f }, { Grade.Rare, 0f }, { Grade.Legendary, 0f } },
            2 => new() { { Grade.Common, 83.4f }, { Grade.Uncommon, 16.5f }, { Grade.Rare, 0.1f }, { Grade.Legendary, 0f } },
            3 => new() { { Grade.Common, 80.9f }, { Grade.Uncommon, 18.59f }, { Grade.Rare, 0.5f }, { Grade.Legendary, 0.01f } },
            4 => new() { { Grade.Common, 78f }, { Grade.Uncommon, 20.95f }, { Grade.Rare, 1f }, { Grade.Legendary, 0.05f } },
            5 => new() { { Grade.Common, 74f }, { Grade.Uncommon, 23.9f }, { Grade.Rare, 2f }, { Grade.Legendary, 0.1f } },
            6 => new() { { Grade.Common, 72.5f }, { Grade.Uncommon, 24f }, { Grade.Rare, 3f }, { Grade.Legendary, 0.5f } },
            7 => new() { { Grade.Common, 68f }, { Grade.Uncommon, 27f }, { Grade.Rare, 4f }, { Grade.Legendary, 1f } },
            8 => new() { { Grade.Common, 64.2f }, { Grade.Uncommon, 27.3f }, { Grade.Rare, 6.5f }, { Grade.Legendary, 2f } },
            9 => new() { { Grade.Common, 60.8f }, { Grade.Uncommon, 28.6f }, { Grade.Rare, 7.6f }, { Grade.Legendary, 3f } },
            10 => new() { { Grade.Common, 58.5f }, { Grade.Uncommon, 29f }, { Grade.Rare, 8.5f }, { Grade.Legendary, 4f } },
            _ => new() { { Grade.Common, 85f }, { Grade.Uncommon, 15f }, { Grade.Rare, 0f }, { Grade.Legendary, 0f } },
        };

        return chances;
    }

    // 풀 초기화용
    
    private void InitMaterialPool(int count)
    {
        _sparkleMaterialPool.Clear();

        for(int i = 0; i<count; i++)
        {
            Material mat = Instantiate(_sparkleMaterial);
            _sparkleMaterialPool.Enqueue(mat);
        }
    }

    private void InitDrawingSlotPool(int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject obj = Instantiate(DrawingSlotPrefab);
            obj.transform.SetParent(DrawingContent);
            obj.SetActive(false);
            _drawingSlotPool.Enqueue(obj);
        }
    }


    private Material GetMaterialFromPool()
    {
        if(_sparkleMaterialPool.Count > 0)
            return _sparkleMaterialPool.Dequeue();
        else
            return Instantiate(_sparkleMaterial);
    }
    private GameObject GetDrawingSlotFromPool()
    {
        if (_drawingSlotPool.Count > 0)
        {
            GameObject slot = _drawingSlotPool.Dequeue();
            slot.SetActive(true);
            return slot;
        }

        GameObject newSlot = Instantiate(DrawingSlotPrefab);
        newSlot.transform.SetParent(DrawingContent);
        return newSlot;
    }
    private void ReturnDrawingSlotToPool(GameObject slot)
    {
        // 머테리얼도 반납
        var shaderImg = slot.transform.Find("Shader Line").GetComponent<Image>();
        if (shaderImg.material != null)
        {
            ReturnMaterialToPool(shaderImg.material);
            shaderImg.material = null;
        }

        slot.SetActive(false);
        _drawingSlotPool.Enqueue(slot);
    }

    // 풀에 다시 넣는 메서드 (필요한 경우)
    private void ReturnMaterialToPool(Material mat)
    {
        _sparkleMaterialPool.Enqueue(mat);
    }

    // 뒤끝 저장 및 불러오기
    [System.Serializable]
    public class GachaSaveData
    {
        public int drawLevel;
        public int currentExp;
    }

    // 저장
    private string gachaRowInDate = string.Empty; // inDate 저장용 필드

    public void SaveGachaDataToBackend()
    {
        GachaSaveData saveData = new GachaSaveData
        {
            drawLevel = _drawLevel,
            currentExp = _currentEXP
        };

        string json = JsonUtility.ToJson(saveData);
        Param param = new Param();

        param.Add("StatData", json);

        var bro = Backend.GameData.Insert("USER_GACHA", param); // 테이블명은 USER_GACHA

        if (bro.IsSuccess())
        {
            Debug.Log("가챠 데이터 저장 성공");
            gachaRowInDate = bro.GetInDate(); // inDate 저장
        }
        else
        {
            Debug.LogError("가챠 데이터 저장 실패 : " + bro);
        }
    }

    // 수정
    public void UpdateGachaDataToBackend()
    {
        if (string.IsNullOrEmpty(gachaRowInDate))
        {
            Debug.LogError("먼저 SaveGachaDataToBackend()로 데이터를 삽입해야 합니다.");
            return;
        }

        GachaSaveData saveData = new GachaSaveData
        {
            drawLevel = _drawLevel,
            currentExp = _currentEXP
        };

        string json = JsonUtility.ToJson(saveData);
        Param param = new Param();

        param.Add("StatData", json);

        var bro = Backend.GameData.UpdateV2("USER_GACHA", gachaRowInDate, Backend.UserInDate, param);

        if (bro.IsSuccess())
            Debug.Log("가챠 데이터 수정 성공");
        else
            Debug.LogError("가챠 데이터 수정 실패: " + bro);
    }

    // 불러오기
    public void LoadGachaDataFromBackend()
    {
        var bro = Backend.GameData.GetMyData("USER_GACHA", new Where());

        if (!bro.IsSuccess())
        {
            Debug.LogError("가챠 데이터 로드 실패: " + bro);
            return;
        }

        JsonData rows = bro.FlattenRows();
        if (rows.Count <= 0)
        {
            Debug.Log("가챠 저장 데이터 없음, 신규 생성");
            SaveGachaDataToBackend();
            return;
        }

        gachaRowInDate = rows[0]["inDate"].ToString(); // 수정에 대비해 저장
        string json = rows[0]["StatData"].ToString();
        GachaSaveData saveData = JsonUtility.FromJson<GachaSaveData>(json);

        // 저장된 값 적용
        _drawLevel = saveData.drawLevel;
        _currentEXP = saveData.currentExp;

        // UI 업데이트
        UpdateChancesByLevel(_drawLevel);
        LvLine.fillAmount = (float)_currentEXP / GetExpToNextLevel(_drawLevel);
        LvText.text = _drawLevel.ToString();
        LvLineNum.text = $"{GetExpToNextLevel(_drawLevel)}/{_currentEXP}";
    }
}