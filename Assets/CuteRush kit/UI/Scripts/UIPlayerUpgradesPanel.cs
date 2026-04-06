using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIPlayerUpgradesPanel : UIPanel
{
    [Header("Bar Colors")]
    [SerializeField] private Color filledColor = Color.green;
    [SerializeField] private Color emptyColor = Color.gray;

    [Header("Upgrade Level Bars (Images)")]
    [SerializeField] private Image[] maxHealthBars;
    [SerializeField] private Image[] medkitHealBars;
    [SerializeField] private Image[] movementSpeedBars;
    [SerializeField] private Image[] jumpForceBars;
    [SerializeField] private Image[] flitchBars;
    [SerializeField] private Image[] boricAcidBars;

    [Header("Upgrade Price Texts")]
    [SerializeField] private TextMeshProUGUI maxHealthPriceText;
    [SerializeField] private TextMeshProUGUI medkitHealPriceText;
    [SerializeField] private TextMeshProUGUI movementSpeedPriceText;
    [SerializeField] private TextMeshProUGUI jumpForcePriceText;
    [SerializeField] private TextMeshProUGUI flitchPriceText;
    [SerializeField] private TextMeshProUGUI boricAcidPriceText;

    private void Awake()
    {
        UIEvents.OnLoadMenu += RefreshAll;
    }

    private void OnDestroy()
    {
        UIEvents.OnLoadMenu -= RefreshAll;
    }

    public override void Show()
    {
        base.Show();
        RefreshAll();
    }

    private void RefreshAll()
    {
        RefreshMaxHealthUI();
        RefreshMedkitHealUI();
        RefreshMovementSpeedUI();
        RefreshJumpForceUI();
        RefreshFlitchUI();
        RefreshBoricAcidUI();
    }

    private void RefreshMaxHealthUI()
    {
        if (SaveManager.Instance == null || SaveManager.Instance.currentSave == null) return;
        int level = SaveManager.Instance.currentSave.playerUpgrades.maxHealthLevel;
        UpdateBars(maxHealthBars, level);
        UpdatePrice(maxHealthPriceText, level, UpgradeManager.Instance?.maxMaxHealthLevel ?? 0);
    }

    private void RefreshMedkitHealUI()
    {
        if (SaveManager.Instance == null || SaveManager.Instance.currentSave == null) return;
        int level = SaveManager.Instance.currentSave.playerUpgrades.medikitHealLevel;
        UpdateBars(medkitHealBars, level);
        UpdatePrice(medkitHealPriceText, level, UpgradeManager.Instance?.maxMedkitLevel ?? 0);
    }

    private void RefreshMovementSpeedUI()
    {
        if (SaveManager.Instance == null || SaveManager.Instance.currentSave == null) return;
        int level = SaveManager.Instance.currentSave.playerUpgrades.movementSpeedLevel;
        UpdateBars(movementSpeedBars, level);
        UpdatePrice(movementSpeedPriceText, level, UpgradeManager.Instance?.maxSpeedLevel ?? 0);
    }

    private void RefreshJumpForceUI()
    {
        if (SaveManager.Instance == null || SaveManager.Instance.currentSave == null) return;
        int level = SaveManager.Instance.currentSave.playerUpgrades.jumpForceLevel;
        UpdateBars(jumpForceBars, level);
        UpdatePrice(jumpForcePriceText, level, UpgradeManager.Instance?.maxJumpLevel ?? 0);
    }

    private void RefreshFlitchUI()
    {
        if (SaveManager.Instance == null || SaveManager.Instance.currentSave == null) return;
        int level = SaveManager.Instance.currentSave.playerUpgrades.flitchLevel;
        UpdateBars(flitchBars, level);
        UpdatePrice(flitchPriceText, level, UpgradeManager.Instance?.maxFlitchLevel ?? 0);
    }

    private void RefreshBoricAcidUI()
    {
        if (SaveManager.Instance == null || SaveManager.Instance.currentSave == null) return;
        int level = SaveManager.Instance.currentSave.playerUpgrades.boricAcidLevel;
        UpdateBars(boricAcidBars, level);
        UpdatePrice(boricAcidPriceText, level, UpgradeManager.Instance?.maxBoricAcidLevel ?? 0);
    }

    private void UpdateBars(Image[] bars, int currentLevel)
    {
        if (bars == null) return;

        for (int i = 0; i < bars.Length; i++)
        {
            if (bars[i] != null)
            {
                if (i < currentLevel)
                {
                    bars[i].color = filledColor;
                }
                else
                {
                    bars[i].color = emptyColor;
                }
            }
        }
    }

    private void UpdatePrice(TextMeshProUGUI priceText, int currentLevel, int maxLevel)
    {
        if (priceText == null) return;

        if (currentLevel >= maxLevel)
        {
            priceText.text = "MAX";
        }
        else if (UpgradeManager.Instance != null)
        {
            int cost = UpgradeManager.Instance.GetUpgradeCost(currentLevel);
            priceText.text = cost.ToString();
        }
    }

    // --- OnClick Functions ---

    public void OnClickUpgradeMaxHealth()
    {
        if (UpgradeManager.Instance != null && UpgradeManager.Instance.UpgradeMaxHealth())
        {
            RefreshMaxHealthUI();
        }
    }

    public void OnClickUpgradeMedkitHeal()
    {
        if (UpgradeManager.Instance != null && UpgradeManager.Instance.UpgradeMedkitHeal())
        {
            RefreshMedkitHealUI();
        }
    }

    public void OnClickUpgradeMovementSpeed()
    {
        if (UpgradeManager.Instance != null && UpgradeManager.Instance.UpgradeMovementSpeed())
        {
            RefreshMovementSpeedUI();
        }
    }

    public void OnClickUpgradeJumpForce()
    {
        if (UpgradeManager.Instance != null && UpgradeManager.Instance.UpgradeJumpForce())
        {
            RefreshJumpForceUI();
        }
    }

    public void OnClickUpgradeFlitch()
    {
        if (UpgradeManager.Instance != null && UpgradeManager.Instance.UpgradeFlitch())
        {
            RefreshFlitchUI();
        }
    }

    public void OnClickUpgradeBoricAcid()
    {
        if (UpgradeManager.Instance != null && UpgradeManager.Instance.UpgradeBoricAcid())
        {
            RefreshBoricAcidUI();
        }
    }
}
