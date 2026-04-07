using System;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance { get; private set; }

    [Header("Max Levels - Player")]
    public int maxMaxHealthLevel = 4;
    public int maxSpeedLevel = 3;
    public int maxJumpLevel = 3;
    public int maxMedkitLevel = 4;
    public int maxFlitchLevel = 3;
    public int maxBoricAcidLevel = 3;

    [Header("Max Levels - Weapons")]
    public int maxPistolLevel = 3;
    public int maxSmgLevel = 3;
    public int maxRailgunLevel = 3;

    [Header("Base Costs - Player Upgrades")]
    public int baseCostMaxHealth = 500;
    public int baseCostSpeed = 500;
    public int baseCostJump = 500;
    public int baseCostMedkitHeal = 500;
    public int baseCostFlitch = 500;
    public int baseCostBoricAcid = 500;

    [Header("Base Costs - Weapons")]
    public int baseCostPistol = 500;
    public int baseCostSmg = 500;
    public int baseCostRailgun = 500;

    [Header("Costs - Consumables (Fixed Price)")]
    public int costMedikit = 1000;
    public int costGrenade = 400;

    [Header("Upgrade Settings")]
    [Tooltip("Moltiplicatore che definisce quanto aumenta il costo ad ogni livello")]
    public float costMultiplier = 2.5f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        UIEvents.OnLoadMenu?.Invoke();
    }

    public int GetUpgradeCost(int baseCost, int currentLevel)
    {
        return Mathf.RoundToInt(baseCost * Mathf.Pow(costMultiplier, currentLevel));
    }

    private bool CanAffordUpgrade(int cost)
    {
        if (SaveManager.Instance == null || SaveManager.Instance.currentSave == null) return false;
        return SaveManager.Instance.currentSave.coins >= cost;
    }

    private void DeductCoins(int cost)
    {
        if (SaveManager.Instance != null && SaveManager.Instance.currentSave != null)
        {
            SaveManager.Instance.currentSave.coins -= cost;
        }
    }

    // --- Player Upgrades ---

    public bool UpgradeMaxHealth()
    {
        if (SaveManager.Instance == null || SaveManager.Instance.currentSave == null) return false;

        PlayerUpgradesSave upgrades = SaveManager.Instance.currentSave.playerUpgrades;
        if (upgrades.maxHealthLevel >= maxMaxHealthLevel) return false;

        int cost = GetUpgradeCost(baseCostMaxHealth, upgrades.maxHealthLevel);
        if (!CanAffordUpgrade(cost)) return false;

        DeductCoins(cost);
        upgrades.maxHealthLevel++;
        SaveManager.Instance.SaveGame();
        return true;
    }

    public bool UpgradeMedkitHeal()
    {
        if (SaveManager.Instance == null || SaveManager.Instance.currentSave == null) return false;

        PlayerUpgradesSave upgrades = SaveManager.Instance.currentSave.playerUpgrades;
        if (upgrades.medikitHealLevel >= maxMedkitLevel) return false;

        int cost = GetUpgradeCost(baseCostMedkitHeal, upgrades.medikitHealLevel);
        if (!CanAffordUpgrade(cost)) return false;

        DeductCoins(cost);
        upgrades.medikitHealLevel++;
        SaveManager.Instance.SaveGame();
        return true;
    }

    public bool UpgradeMovementSpeed()
    {
        if (SaveManager.Instance == null || SaveManager.Instance.currentSave == null) return false;

        PlayerUpgradesSave upgrades = SaveManager.Instance.currentSave.playerUpgrades;
        if (upgrades.movementSpeedLevel >= maxSpeedLevel) return false;

        int cost = GetUpgradeCost(baseCostSpeed, upgrades.movementSpeedLevel);
        if (!CanAffordUpgrade(cost)) return false;

        DeductCoins(cost);
        upgrades.movementSpeedLevel++;
        SaveManager.Instance.SaveGame();
        return true;
    }

    public bool UpgradeJumpForce()
    {
        if (SaveManager.Instance == null || SaveManager.Instance.currentSave == null) return false;

        PlayerUpgradesSave upgrades = SaveManager.Instance.currentSave.playerUpgrades;
        if (upgrades.jumpForceLevel >= maxJumpLevel) return false;

        int cost = GetUpgradeCost(baseCostJump, upgrades.jumpForceLevel);
        if (!CanAffordUpgrade(cost)) return false;

        DeductCoins(cost);
        upgrades.jumpForceLevel++;
        SaveManager.Instance.SaveGame();
        return true;
    }

    public bool UpgradeFlitch()
    {
        if (SaveManager.Instance == null || SaveManager.Instance.currentSave == null) return false;

        PlayerUpgradesSave upgrades = SaveManager.Instance.currentSave.playerUpgrades;
        if (upgrades.flitchLevel >= maxFlitchLevel) return false;

        int cost = GetUpgradeCost(baseCostFlitch, upgrades.flitchLevel);
        if (!CanAffordUpgrade(cost)) return false;

        DeductCoins(cost);
        upgrades.flitchLevel++;
        SaveManager.Instance.SaveGame();
        return true;
    }

    public bool UpgradeBoricAcid()
    {
        if (SaveManager.Instance == null || SaveManager.Instance.currentSave == null) return false;

        PlayerUpgradesSave upgrades = SaveManager.Instance.currentSave.playerUpgrades;
        if (upgrades.boricAcidLevel >= maxBoricAcidLevel) return false;

        int cost = GetUpgradeCost(baseCostBoricAcid, upgrades.boricAcidLevel);
        if (!CanAffordUpgrade(cost)) return false;

        DeductCoins(cost);
        upgrades.boricAcidLevel++;
        SaveManager.Instance.SaveGame();
        return true;
    }

    // --- Consumables ---

    public bool BuyMedikit()
    {
        if (SaveManager.Instance == null || SaveManager.Instance.currentSave == null) return false;

        if (!CanAffordUpgrade(costMedikit)) return false;

        DeductCoins(costMedikit);
        SaveManager.Instance.currentSave.medikitCount++;
        SaveManager.Instance.SaveGame();
        return true;
    }

    public bool BuyGrenade()
    {
        if (SaveManager.Instance == null || SaveManager.Instance.currentSave == null) return false;

        if (!CanAffordUpgrade(costGrenade)) return false;

        DeductCoins(costGrenade);
        SaveManager.Instance.currentSave.grenadeCount++;
        SaveManager.Instance.SaveGame();
        return true;
    }

    // --- Weapon Upgrades ---

    public bool UpgradePistol()
    {
        if (SaveManager.Instance == null || SaveManager.Instance.currentSave == null) return false;

        WeaponUpgradesSave upgrades = SaveManager.Instance.currentSave.weaponUpgrades;
        if (upgrades.pistolLevel >= maxPistolLevel) return false;

        int cost = GetUpgradeCost(baseCostPistol, upgrades.pistolLevel);
        if (!CanAffordUpgrade(cost)) return false;

        DeductCoins(cost);
        upgrades.pistolLevel++;
        SaveManager.Instance.SaveGame();
        return true;
    }

    public bool UpgradeSmg()
    {
        if (SaveManager.Instance == null || SaveManager.Instance.currentSave == null) return false;

        WeaponUpgradesSave upgrades = SaveManager.Instance.currentSave.weaponUpgrades;
        if (upgrades.smgLevel >= maxSmgLevel) return false;

        int cost = GetUpgradeCost(baseCostSmg, upgrades.smgLevel);
        if (!CanAffordUpgrade(cost)) return false;

        DeductCoins(cost);
        upgrades.smgLevel++;
        SaveManager.Instance.SaveGame();
        return true;
    }

    public bool UpgradeRailgun()
    {
        if (SaveManager.Instance == null || SaveManager.Instance.currentSave == null) return false;

        WeaponUpgradesSave upgrades = SaveManager.Instance.currentSave.weaponUpgrades;
        if (upgrades.railgunLevel >= maxRailgunLevel) return false;

        int cost = GetUpgradeCost(baseCostRailgun, upgrades.railgunLevel);
        if (!CanAffordUpgrade(cost)) return false;

        DeductCoins(cost);
        upgrades.railgunLevel++;
        SaveManager.Instance.SaveGame();
        return true;
    }
}