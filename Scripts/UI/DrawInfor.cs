using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.HighDefinition.ScalableSettingLevelParameter;

public class DrawInfor : MonoBehaviour
{
    public DrawingPanelManager DrawingPanelManager;
    public PopDown PopDown;
    public PopUp PopUp;

    public GameObject DrawLevelInformation;

    [Header("Text%")]
    public TMP_Text LText;
    public TMP_Text RText;
    public TMP_Text UText;
    public TMP_Text CText;

    public TMP_Text LvText;

    [Header("Button")]
    public Button DownButton;
    public Button UpButton;

    public Button DrawInforOPButton;
    public Button DrawInforClButton;

    private int _UpLv = 0;

    private void Start()
    {
        DownButton.onClick.AddListener(DownButtonClick);
        UpButton.onClick.AddListener(UpButtonClick);
        DrawInforOPButton.onClick.AddListener(OpButtonClick);
        DrawInforClButton.onClick.AddListener(XButtonClick);
        UpdateChancesUI();
    }

    public void ShowChancesUI(int level)
    {
        var chances = DrawingPanelManager.GetcurrentGradeChances(level);

        LvText.text = $"LV.{level}";
        LText.text = $"{chances[Grade.Legendary]}%";
        RText.text = $"{chances[Grade.Rare]}%";
        UText.text = $"{chances[Grade.Uncommon]}%";
        CText.text = $"{chances[Grade.Common]}%";
    }

    private void DownButtonClick()
    {
        SoundManager.Instance.PlaySFX("Button");

        if (DrawingPanelManager.CurrentDrawLevel + _UpLv > 1)
            _UpLv--;
        UpdateChancesUI();
    }

    private void UpButtonClick()
    {
        SoundManager.Instance.PlaySFX("Button");

        if (DrawingPanelManager.CurrentDrawLevel + _UpLv <10)
            _UpLv++;
        UpdateChancesUI();
    }

    private void UpdateChancesUI()
    {
        int level = Mathf.Clamp(DrawingPanelManager.CurrentDrawLevel + _UpLv, 1, 10);
        ShowChancesUI(level);
    }

    private void XButtonClick()
    {
        SoundManager.Instance.PlaySFX("Button");

        PopDown.PlayShrink(DrawLevelInformation);
        ShowChancesUI(DrawingPanelManager.CurrentDrawLevel);
        _UpLv = 0;
    }

    private void OpButtonClick()
    {
        SoundManager.Instance.PlaySFX("Button");

        ShowChancesUI(DrawingPanelManager.CurrentDrawLevel);
        _UpLv = 0;
        PopUp.PlayGrow(DrawLevelInformation);
    }
}