using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }
    public SaveData currentSave;
    public GlobalLeaderboardData globalLeaderboard;
    public string GlobalSavePath => Path.Combine(Application.persistentDataPath, "Global\\GlobalLeaderboard.json");

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

        LoadGlobalLeaderboard();
    }
    public void LoadGlobalLeaderboard()
    {
        if (File.Exists(GlobalSavePath))
        {
            try
            {
                string json = File.ReadAllText(GlobalSavePath);
                globalLeaderboard = JsonUtility.FromJson<GlobalLeaderboardData>(json);
                if (globalLeaderboard == null)
                {
                    globalLeaderboard = new GlobalLeaderboardData();
                }
                Debug.Log($"Global leaderboard loaded successfully from: {GlobalSavePath}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to load global leaderboard from {GlobalSavePath}: {e.Message}");
                globalLeaderboard = new GlobalLeaderboardData();
            }
        }
        else
        {
            globalLeaderboard = new GlobalLeaderboardData();
            Debug.Log("No global leaderboard found, initialized a new one.");
        }
    }
    public void SaveGlobalLeaderboard()
    {
        if (globalLeaderboard == null)
        {
            Debug.LogError("Cannot save global leaderboard: globalLeaderboard is null.");
            return;
        }

        string json = JsonUtility.ToJson(globalLeaderboard, true);

        try
        {
            File.WriteAllText(GlobalSavePath, json);
            Debug.Log($"Global leaderboard saved successfully to: {GlobalSavePath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to save global leaderboard to {GlobalSavePath}: {e.Message}");
        }
    }
    public void SubmitScore(string mapName, int finalScore, string difficulty)
    {
        if (currentSave != null)
        {
            MapLeaderboard personalMapData = currentSave.mapLeaderboards.Find(m => m.mapName == mapName);
            if (personalMapData == null)
            {
                personalMapData = new MapLeaderboard(mapName);
                currentSave.mapLeaderboards.Add(personalMapData);
            }

            personalMapData.topScores.Add(new ScoreRecord(finalScore, difficulty));

            personalMapData.topScores.Sort((a, b) => b.score.CompareTo(a.score));
            if (personalMapData.topScores.Count > 5)
            {
                personalMapData.topScores.RemoveRange(5, personalMapData.topScores.Count - 5);
            }

            SaveGame();
        }

        if (globalLeaderboard != null && currentSave != null && !string.IsNullOrEmpty(currentSave.username))
        {
            GlobalMapLeaderboard globalMapData = globalLeaderboard.maps.Find(m => m.mapName == mapName);
            if (globalMapData == null)
            {
                globalMapData = new GlobalMapLeaderboard(mapName);
                globalLeaderboard.maps.Add(globalMapData);
            }

            globalMapData.topScores.Add(new GlobalScoreRecord(currentSave.username, finalScore, difficulty));

            globalMapData.topScores.Sort((a, b) => b.score.CompareTo(a.score));
            if (globalMapData.topScores.Count > 5)
            {
                globalMapData.topScores.RemoveRange(5, globalMapData.topScores.Count - 5);
            }

            SaveGlobalLeaderboard();
        }
    }
    public void CreateNewGame(string username)
    {
        PlayerPrefs.SetString("Weapon", GameConstants.WEAPON_PISTOL);
        PlayerPrefs.Save();
        currentSave = new SaveData(username);
        SaveGame();
    }

    public void SaveGame()
    {
        if (currentSave == null || string.IsNullOrEmpty(currentSave.username))
        {
            Debug.LogError("Cannot save game: currentSave is null or username is empty.");
            return;
        }

        currentSave.selectedWeapon = PlayerPrefs.GetString("Weapon", GameConstants.WEAPON_PISTOL);
        currentSave.audioSettings.MasterVolume = AudioManager.Instance.MasterVolume;
        currentSave.audioSettings.SFXVolume = AudioManager.Instance.SFXVolume;
        currentSave.audioSettings.AmbientVolume = AudioManager.Instance.AmbientVolume;
        currentSave.audioSettings.MusicVolume = AudioManager.Instance.MusicVolume;
        

        string json = JsonUtility.ToJson(currentSave, true);
        string safeUsername = SanitizeFilename(currentSave.username);
        string filePath = Path.Combine(Application.persistentDataPath, safeUsername + ".json");

        try
        {
            File.WriteAllText(filePath, json);
            Debug.Log($"Game saved successfully to: {filePath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to save game to {filePath}: {e.Message}");
        }
    }

    public void LoadGame(string username)
    {
        string safeUsername = SanitizeFilename(username);
        string filePath = Path.Combine(Application.persistentDataPath, safeUsername + ".json");

        if (File.Exists(filePath))
        {
            try
            {
                string json = File.ReadAllText(filePath);
                currentSave = JsonUtility.FromJson<SaveData>(json);

                PlayerPrefs.SetString("Weapon", string.IsNullOrEmpty(currentSave.selectedWeapon) ? GameConstants.WEAPON_PISTOL : currentSave.selectedWeapon);
                AudioManager.Instance.SetMusicVolume(currentSave.audioSettings.MusicVolume);
                AudioManager.Instance.SetAmbientVolume(currentSave.audioSettings.AmbientVolume);
                AudioManager.Instance.SetSFXVolume(currentSave.audioSettings.SFXVolume);
                AudioManager.Instance.SetMasterVolume(currentSave.audioSettings.MasterVolume);
                PlayerPrefs.Save();

                Debug.Log($"Game loaded successfully from: {filePath}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to load game from {filePath}: {e.Message}");
            }
        }
        else
        {
            Debug.LogError($"Save file not found at: {filePath}");
        }
    }

    public List<string> GetAllSavedProfiles()
    {
        List<string> profiles = new List<string>();
        string path = Application.persistentDataPath;

        DirectoryInfo directoryInfo = new DirectoryInfo(path);
        FileInfo[] files = directoryInfo.GetFiles("*.json");

        foreach (FileInfo file in files)
        {
            string profileName = Path.GetFileNameWithoutExtension(file.Name);
            profiles.Add(profileName);
        }

        return profiles;
    }

    private string SanitizeFilename(string filename)
    {
        string safeName = Path.GetFileName(filename);

        foreach (char c in Path.GetInvalidFileNameChars())
        {
            safeName = safeName.Replace(c.ToString(), "");
        }

        return safeName;
    }
}
