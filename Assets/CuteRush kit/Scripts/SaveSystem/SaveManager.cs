using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    // Singleton instance
    public static SaveManager Instance { get; private set; }

    // Reference to the active profile
    public SaveData currentSave;

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
