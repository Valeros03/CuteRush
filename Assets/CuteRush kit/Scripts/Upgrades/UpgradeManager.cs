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

    [Header("Max Levels - Weapons")]
    public int maxPistolLevel = 3;
    public int maxSmgLevel = 3;
    public int maxRailgunLevel = 3;

    private const int BASE_COST = 500;
    private const float COST_MULTIPLIER = 2.5f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Calculates the cost of the next upgrade based on the current level.
    /// Increment of +150% each level (e.g. 500, 1250, 3125...).
    /// </summary>
    /// <param name="currentLevel">The current level of the upgrade (0 means base level).</param>
    /// <returns>The cost to upgrade to the next level.</returns>
    public int GetUpgradeCost(int currentLevel)
    {
        return Mathf.RoundToInt(BASE_COST * Mathf.Pow(COST_MULTIPLIER, currentLevel));
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

        int cost = GetUpgradeCost(upgrades.maxHealthLevel);
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

        int cost = GetUpgradeCost(upgrades.medikitHealLevel);
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

        int cost = GetUpgradeCost(upgrades.movementSpeedLevel);
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

        int cost = GetUpgradeCost(upgrades.jumpForceLevel);
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

        int cost = GetUpgradeCost(upgrades.flitchLevel);
        if (!CanAffordUpgrade(cost)) return false;

        DeductCoins(cost);
        upgrades.flitchLevel++;
        SaveManager.Instance.SaveGame();
        return true;
    }

    // --- Weapon Upgrades ---

    public bool UpgradePistol()
    {
        if (SaveManager.Instance == null || SaveManager.Instance.currentSave == null) return false;

        WeaponUpgradesSave upgrades = SaveManager.Instance.currentSave.weaponUpgrades;
        if (upgrades.pistolLevel >= maxPistolLevel) return false;

        int cost = GetUpgradeCost(upgrades.pistolLevel);
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

        int cost = GetUpgradeCost(upgrades.smgLevel);
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

        int cost = GetUpgradeCost(upgrades.railgunLevel);
        if (!CanAffordUpgrade(cost)) return false;

        DeductCoins(cost);
        upgrades.railgunLevel++;
        SaveManager.Instance.SaveGame();
        return true;
    }
}
