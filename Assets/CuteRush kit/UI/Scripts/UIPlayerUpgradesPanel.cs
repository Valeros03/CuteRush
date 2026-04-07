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

    [Header("Weapon Upgrade Level Bars")]
    [SerializeField] private Image[] pistolBars;
    [SerializeField] private Image[] smgBars;
    [SerializeField] private Image[] railgunBars;

    [Header("Weapon Upgrade Price Texts")]
    [SerializeField] private TextMeshProUGUI pistolPriceText;
    [SerializeField] private TextMeshProUGUI smgPriceText;
    [SerializeField] private TextMeshProUGUI railgunPriceText;

    [Header("Consumables UI")]
    [SerializeField] private TextMeshProUGUI medkitCountText;
    [SerializeField] private TextMeshProUGUI medkitPriceText;
    [SerializeField] private TextMeshProUGUI grenadeCountText;
    [SerializeField] private TextMeshProUGUI grenadePriceText;

    public Toggle RadioPistol;
    public Toggle RadioSMG;
    public Toggle RadioRailgun;

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

        RefreshPistolUI();
        RefreshSmgUI();
        RefreshRailgunUI();
        RefreshConsumablesUI();
    }

    public void LoadSavedWeaponUI()
    {
        string savedWeapon = PlayerPrefs.GetString("Weapon", GameConstants.WEAPON_PISTOL);

        RadioPistol.isOn = false;
        RadioSMG.isOn = false;
        RadioRailgun.isOn = false;

        switch (savedWeapon)
        {
            case "Pistol":
                RadioPistol.isOn = true;
                break;

            case "SMG":
                RadioSMG.isOn = true;
                break;

            case "Railgun":
                RadioRailgun.isOn = true;
                break;

            default:
                RadioPistol.isOn = true;
                break;
        }
    }

    private void RefreshMaxHealthUI()
    {
        if (SaveManager.Instance == null || SaveManager.Instance.currentSave == null || UpgradeManager.Instance == null) return;
        int level = SaveManager.Instance.currentSave.playerUpgrades.maxHealthLevel;
        UpdateBars(maxHealthBars, level);
        UpdatePrice(maxHealthPriceText, level, UpgradeManager.Instance.maxMaxHealthLevel, UpgradeManager.Instance.baseCostMaxHealth);
    }

    private void RefreshMedkitHealUI()
    {
        if (SaveManager.Instance == null || SaveManager.Instance.currentSave == null || UpgradeManager.Instance == null) return;
        int level = SaveManager.Instance.currentSave.playerUpgrades.medikitHealLevel;
        UpdateBars(medkitHealBars, level);
        UpdatePrice(medkitHealPriceText, level, UpgradeManager.Instance.maxMedkitLevel, UpgradeManager.Instance.baseCostMedkitHeal);
    }

    private void RefreshMovementSpeedUI()
    {
        if (SaveManager.Instance == null || SaveManager.Instance.currentSave == null || UpgradeManager.Instance == null) return;
        int level = SaveManager.Instance.currentSave.playerUpgrades.movementSpeedLevel;
        UpdateBars(movementSpeedBars, level);
        UpdatePrice(movementSpeedPriceText, level, UpgradeManager.Instance.maxSpeedLevel, UpgradeManager.Instance.baseCostSpeed);
    }

    private void RefreshJumpForceUI()
    {
        if (SaveManager.Instance == null || SaveManager.Instance.currentSave == null || UpgradeManager.Instance == null) return;
        int level = SaveManager.Instance.currentSave.playerUpgrades.jumpForceLevel;
        UpdateBars(jumpForceBars, level);
        UpdatePrice(jumpForcePriceText, level, UpgradeManager.Instance.maxJumpLevel, UpgradeManager.Instance.baseCostJump);
    }

    private void RefreshFlitchUI()
    {
        if (SaveManager.Instance == null || SaveManager.Instance.currentSave == null || UpgradeManager.Instance == null) return;
        int level = SaveManager.Instance.currentSave.playerUpgrades.flitchLevel;
        UpdateBars(flitchBars, level);
        UpdatePrice(flitchPriceText, level, UpgradeManager.Instance.maxFlitchLevel, UpgradeManager.Instance.baseCostFlitch);
    }

    private void RefreshBoricAcidUI()
    {
        if (SaveManager.Instance == null || SaveManager.Instance.currentSave == null || UpgradeManager.Instance == null) return;
        int level = SaveManager.Instance.currentSave.playerUpgrades.boricAcidLevel;
        UpdateBars(boricAcidBars, level);
        UpdatePrice(boricAcidPriceText, level, UpgradeManager.Instance.maxBoricAcidLevel, UpgradeManager.Instance.baseCostBoricAcid);
    }

    private void RefreshPistolUI()
    {
        if (SaveManager.Instance == null || SaveManager.Instance.currentSave == null || UpgradeManager.Instance == null) return;
        int level = SaveManager.Instance.currentSave.weaponUpgrades.pistolLevel;
        UpdateBars(pistolBars, level);
        UpdatePrice(pistolPriceText, level, UpgradeManager.Instance.maxPistolLevel, UpgradeManager.Instance.baseCostPistol);
    }

    private void RefreshSmgUI()
    {
        if (SaveManager.Instance == null || SaveManager.Instance.currentSave == null || UpgradeManager.Instance == null) return;
        int level = SaveManager.Instance.currentSave.weaponUpgrades.smgLevel;
        UpdateBars(smgBars, level);
        UpdatePrice(smgPriceText, level, UpgradeManager.Instance.maxSmgLevel, UpgradeManager.Instance.baseCostSmg);
    }

    private void RefreshRailgunUI()
    {
        if (SaveManager.Instance == null || SaveManager.Instance.currentSave == null || UpgradeManager.Instance == null) return;
        int level = SaveManager.Instance.currentSave.weaponUpgrades.railgunLevel;
        UpdateBars(railgunBars, level);
        UpdatePrice(railgunPriceText, level, UpgradeManager.Instance.maxRailgunLevel, UpgradeManager.Instance.baseCostRailgun);
    }

    private void RefreshConsumablesUI()
    {
        if (SaveManager.Instance == null || SaveManager.Instance.currentSave == null || UpgradeManager.Instance == null) return;

        if (medkitCountText != null)
            medkitCountText.text = SaveManager.Instance.currentSave.medikitCount.ToString();

        if (grenadeCountText != null)
            grenadeCountText.text = SaveManager.Instance.currentSave.grenadeCount.ToString();

        if (medkitPriceText != null)
            medkitPriceText.text = UpgradeManager.Instance.costMedikit.ToString();

        if (grenadePriceText != null)
            grenadePriceText.text = UpgradeManager.Instance.costGrenade.ToString();
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

    private void UpdatePrice(TextMeshProUGUI priceText, int currentLevel, int maxLevel, int baseCost)
    {
        if (priceText == null) return;

        if (currentLevel >= maxLevel)
        {
            priceText.text = "MAX";
        }
        else if (UpgradeManager.Instance != null)
        {
            int cost = UpgradeManager.Instance.GetUpgradeCost(baseCost, currentLevel);
            priceText.text = cost.ToString();
        }
    }

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

    public void OnClickUpgradePistol()
    {
        if (UpgradeManager.Instance != null && UpgradeManager.Instance.UpgradePistol())
        {
            RefreshPistolUI();
        }
    }

    public void OnClickUpgradeSmg()
    {
        if (UpgradeManager.Instance != null && UpgradeManager.Instance.UpgradeSmg())
        {
            RefreshSmgUI();
        }
    }

    public void OnClickUpgradeRailgun()
    {
        if (UpgradeManager.Instance != null && UpgradeManager.Instance.UpgradeRailgun())
        {
            RefreshRailgunUI();
        }
    }

    public void OnClickBuyMedikit()
    {
        if (UpgradeManager.Instance != null && UpgradeManager.Instance.BuyMedikit())
        {
            RefreshConsumablesUI();
        }
    }

    public void OnClickBuyGrenade()
    {
        if (UpgradeManager.Instance != null && UpgradeManager.Instance.BuyGrenade())
        {
            RefreshConsumablesUI();
        }
    }

    public void SetPistol()
    {
        PlayerPrefs.SetString("Weapon", GameConstants.WEAPON_PISTOL);
    }
    public void SetSMG()
    {
        PlayerPrefs.SetString("Weapon", GameConstants.WEAPON_SMG);
    }
    public void SetRailgun()
    {
        PlayerPrefs.SetString("Weapon", GameConstants.WEAPON_RAILGUN);
    }
}