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
    public PlayerUpgradesSave playerUpgrades;
    public WeaponUpgradesSave weaponUpgrades;

    public AudioSettings audioSettings;

    // Selected Weapon
    public string selectedWeapon;

    // High scores
    public List<MapLeaderboard> mapLeaderboards;

    public SaveData(string name)
    {
        username = name;
        coins = 0;
        medikitCount = 0;
        grenadeCount = 0;

        playerUpgrades = new PlayerUpgradesSave();
        weaponUpgrades = new WeaponUpgradesSave();
        selectedWeapon = GameConstants.WEAPON_PISTOL;
        mapLeaderboards = new List<MapLeaderboard>();
    }
}

[System.Serializable]
public class AudioSettings
{
    public float MasterVolume;
    public float SFXVolume;
    public float MusicVolume;
    public float AmbientVolume;

    public AudioSettings()
    {
        MasterVolume = 1.0f;
        SFXVolume = 1.0f;
        MusicVolume = 1.0f;
        AmbientVolume = 1.0f;
    }
}

[System.Serializable]
public class PlayerUpgradesSave
{
    public int maxHealthLevel;
    public int medikitHealLevel;
    public int movementSpeedLevel;
    public int jumpForceLevel;
    public int flitchLevel;
    public int boricAcidLevel;

    public PlayerUpgradesSave()
    {
        maxHealthLevel = 0;
        medikitHealLevel = 0;
        movementSpeedLevel = 0;
        jumpForceLevel = 0;
        flitchLevel = 0;
        boricAcidLevel = 0;
    }
}

[System.Serializable]
public class WeaponUpgradesSave
{
    public int pistolLevel;
    public int smgLevel;
    public int railgunLevel;

    public WeaponUpgradesSave()
    {
        pistolLevel = 0;
        smgLevel = 0;
        railgunLevel = 0;
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
