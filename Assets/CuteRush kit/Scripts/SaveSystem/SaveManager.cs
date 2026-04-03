using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    // Singleton instance
    public static SaveManager Instance { get; private set; }

    // Reference to the active profile
    public SaveData currentSave;

    // Reference to the global leaderboard
    public GlobalLeaderboardData globalLeaderboard;

    // Path to the global leaderboard save file
    public string GlobalSavePath => Path.Combine(Application.persistentDataPath, "GlobalLeaderboard.json");

    private void Awake()
    {
        // Enforce Singleton pattern
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

    /// <summary>
    /// Loads the global leaderboard from JSON or initializes a new one.
    /// </summary>
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

    /// <summary>
    /// Serializes globalLeaderboard to JSON and overwrites the file.
    /// </summary>
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

    /// <summary>
    /// Submits a score, updating both personal and global leaderboards.
    /// </summary>
    public void SubmitScore(string mapName, int finalScore, string difficulty)
    {
        // Update personal leaderboard
        if (currentSave != null)
        {
            MapLeaderboard personalMapData = currentSave.mapLeaderboards.Find(m => m.mapName == mapName);
            if (personalMapData == null)
            {
                personalMapData = new MapLeaderboard(mapName);
                currentSave.mapLeaderboards.Add(personalMapData);
            }

            personalMapData.topScores.Add(new ScoreRecord(finalScore, difficulty));

            // Sort descending and keep top 5
            personalMapData.topScores.Sort((a, b) => b.score.CompareTo(a.score));
            if (personalMapData.topScores.Count > 5)
            {
                personalMapData.topScores.RemoveRange(5, personalMapData.topScores.Count - 5);
            }

            SaveGame();
        }

        // Update global leaderboard
        if (globalLeaderboard != null && currentSave != null && !string.IsNullOrEmpty(currentSave.username))
        {
            GlobalMapLeaderboard globalMapData = globalLeaderboard.maps.Find(m => m.mapName == mapName);
            if (globalMapData == null)
            {
                globalMapData = new GlobalMapLeaderboard(mapName);
                globalLeaderboard.maps.Add(globalMapData);
            }

            globalMapData.topScores.Add(new GlobalScoreRecord(currentSave.username, finalScore, difficulty));

            // Sort descending and keep top 5
            globalMapData.topScores.Sort((a, b) => b.score.CompareTo(a.score));
            if (globalMapData.topScores.Count > 5)
            {
                globalMapData.topScores.RemoveRange(5, globalMapData.topScores.Count - 5);
            }

            SaveGlobalLeaderboard();
        }
    }

    /// <summary>
    /// Creates a new SaveData and saves it as a JSON file in Application.persistentDataPath.
    /// </summary>
    /// <param name="username">The username for the new profile.</param>
    public void CreateNewGame(string username)
    {
        currentSave = new SaveData(username);
        SaveGame();
    }

    /// <summary>
    /// Serializes currentSave to JSON and overwrites the file.
    /// </summary>
    public void SaveGame()
    {
        if (currentSave == null || string.IsNullOrEmpty(currentSave.username))
        {
            Debug.LogError("Cannot save game: currentSave is null or username is empty.");
            return;
        }

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

    /// <summary>
    /// Reads the JSON file and deserializes it into currentSave.
    /// </summary>
    /// <param name="username">The username of the profile to load.</param>
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

    /// <summary>
    /// Scans persistentDataPath for .json files and returns a list of profile names.
    /// </summary>
    /// <returns>A list of profile names (without the .json extension).</returns>
    public List<string> GetAllSavedProfiles()
    {
        List<string> profiles = new List<string>();
        string path = Application.persistentDataPath;

        DirectoryInfo directoryInfo = new DirectoryInfo(path);
        FileInfo[] files = directoryInfo.GetFiles("*.json");

        foreach (FileInfo file in files)
        {
            // Remove the .json extension to get the profile name
            string profileName = Path.GetFileNameWithoutExtension(file.Name);
            profiles.Add(profileName);
        }

        return profiles;
    }

    /// <summary>
    /// Sanitizes the filename to prevent path traversal vulnerabilities or invalid characters.
    /// </summary>
    private string SanitizeFilename(string filename)
    {
        // Simple sanitization: keep only letters, numbers, and basic punctuation,
        // or just rely on GetFileName to strip directory paths.
        // GetFileName strips out any directory information, ensuring only a filename is used.
        string safeName = Path.GetFileName(filename);

        // Additionally, remove invalid file name characters
        foreach (char c in Path.GetInvalidFileNameChars())
        {
            safeName = safeName.Replace(c.ToString(), "");
        }

        return safeName;
    }
}
