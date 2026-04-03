using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public string username;

    // Inventory fields
    public int coins;
    public int medikitCount;
    public int grenadeCount;

    // Upgrades
    public PlayerUpgrades playerUpgrades;
    public WeaponUpgrades weaponUpgrades;

    // High scores
    public List<MapLeaderboard> mapLeaderboards;

    public SaveData(string name)
    {
        username = name;
        coins = 0;
        medikitCount = 0;
        grenadeCount = 0;

        playerUpgrades = new PlayerUpgrades();
        weaponUpgrades = new WeaponUpgrades();
        mapLeaderboards = new List<MapLeaderboard>();
    }
}

[System.Serializable]
public class PlayerUpgrades
{
    public int maxHealthLevel;
    public int medikitHealLevel;
    public int movementSpeedLevel;
    public int jumpForceLevel;
    public int flinchResistLevel;

    public PlayerUpgrades()
    {
        maxHealthLevel = 1;
        medikitHealLevel = 1;
        movementSpeedLevel = 1;
        jumpForceLevel = 1;
        flinchResistLevel = 1;
    }
}

[System.Serializable]
public class WeaponUpgrades
{
    public WeaponStats pistolStats;
    public WeaponStats smgStats;
    public WeaponStats railgunStats;

    public WeaponUpgrades()
    {
        pistolStats = new WeaponStats();
        smgStats = new WeaponStats();
        railgunStats = new WeaponStats();
    }
}

[System.Serializable]
public class WeaponStats
{
    public int damageLevel;
    public int magSizeLevel;
    public int fireRateLevel;
    public int accuracyLevel;
    public int beamWidthLevel;
    public int pierceLevel;

    public WeaponStats()
    {
        damageLevel = 1;
        magSizeLevel = 1;
        fireRateLevel = 1;
        accuracyLevel = 1;
        beamWidthLevel = 1;
        pierceLevel = 1;
    }
}

[System.Serializable]
public class MapLeaderboard
{
    public string mapName;
    public List<ScoreRecord> topScores;

    public MapLeaderboard(string name)
    {
        mapName = name;
        topScores = new List<ScoreRecord>();
    }
}

[System.Serializable]
public class ScoreRecord
{
    public int score;
    public string difficulty;

    public ScoreRecord(int s, string diff)
    {
        score = s;
        difficulty = diff;
    }
}
