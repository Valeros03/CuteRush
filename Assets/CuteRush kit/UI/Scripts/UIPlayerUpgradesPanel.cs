using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerUpgradesPanel : UIPanel
{
    [Header("Shop Reference")]
    [Tooltip("Reference to the UpgradeManager acting as the Shop.")]
    [SerializeField] private UpgradeManager shop;

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

    public override void Show()
    {
        base.Show();
        RefreshAllBars();
    }

    private void RefreshAllBars()
    {
        if (SaveManager.Instance == null || SaveManager.Instance.currentSave == null) return;

        PlayerUpgradesSave upgrades = SaveManager.Instance.currentSave.playerUpgrades;

        UpdateBars(maxHealthBars, upgrades.maxHealthLevel);
        UpdateBars(medkitHealBars, upgrades.medikitHealLevel);
        UpdateBars(movementSpeedBars, upgrades.movementSpeedLevel);
        UpdateBars(jumpForceBars, upgrades.jumpForceLevel);
        UpdateBars(flitchBars, upgrades.flitchLevel);
        UpdateBars(boricAcidBars, upgrades.boricAcidLevel);
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

    // --- OnClick Functions ---

    public void OnClickUpgradeMaxHealth()
    {
        if (shop != null && shop.UpgradeMaxHealth())
        {
            RefreshAllBars();
        }
    }

    public void OnClickUpgradeMedkitHeal()
    {
        if (shop != null && shop.UpgradeMedkitHeal())
        {
            RefreshAllBars();
        }
    }

    public void OnClickUpgradeMovementSpeed()
    {
        if (shop != null && shop.UpgradeMovementSpeed())
        {
            RefreshAllBars();
        }
    }

    public void OnClickUpgradeJumpForce()
    {
        if (shop != null && shop.UpgradeJumpForce())
        {
            RefreshAllBars();
        }
    }

    public void OnClickUpgradeFlitch()
    {
        if (shop != null && shop.UpgradeFlitch())
        {
            RefreshAllBars();
        }
    }

    public void OnClickUpgradeBoricAcid()
    {
        if (shop != null && shop.UpgradeBoricAcid())
        {
            RefreshAllBars();
        }
    }
}
