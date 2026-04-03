using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// This script provides the core methods for integrating the Save System into the Main Menu UI.
/// It doesn't contain the full UI script (like dealing with buttons visually),
/// but contains the requested integration logic methods.
/// </summary>
public class MainMenuSaveIntegration : MonoBehaviour
{
    [Tooltip("Name of the main game scene to load after creating or loading a profile.")]
    public string gameSceneName = "GameScene";

    /// <summary>
    /// Handles the "New Game" flow.
    /// Checks if the username already exists. If not, creates a new game,
    /// saves the username to PlayerPrefs, and loads the game scene.
    /// </summary>
    /// <param name="username">The new desired username from the UI input field.</param>
    public void HandleNewGame(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            Debug.LogWarning("Username cannot be empty.");
            return;
        }

        // Check if the username already exists
        List<string> existingProfiles = SaveManager.Instance.GetAllSavedProfiles();
        if (existingProfiles.Contains(username))
        {
            Debug.LogWarning($"A profile with the username '{username}' already exists. Please choose a different name or load the existing profile.");
            // Here you would typically show an error message to the player via UI
            return;
        }

        // If not, create new game
        SaveManager.Instance.CreateNewGame(username);

        // Save the username in PlayerPrefs
        PlayerPrefs.SetString("LastUsername", username);
        PlayerPrefs.Save();

        // Load the game scene
        LoadGameScene();
    }

    /// <summary>
    /// Fetches the list of all saved profiles to populate dynamic UI buttons.
    /// </summary>
    /// <returns>A list of profile names.</returns>
    public List<string> GetProfilesForUI()
    {
        return SaveManager.Instance.GetAllSavedProfiles();
    }

    /// <summary>
    /// Handles the "Load Game" flow.
    /// Called when a dynamic profile button is clicked in the UI.
    /// </summary>
    /// <param name="profileName">The name of the profile to load.</param>
    public void HandleLoadGame(string profileName)
    {
        if (string.IsNullOrWhiteSpace(profileName))
        {
            Debug.LogWarning("Profile name to load is empty.");
            return;
        }

        // Load the game data via SaveManager
        SaveManager.Instance.LoadGame(profileName);

        // Update the LastUsername in PlayerPrefs
        PlayerPrefs.SetString("LastUsername", profileName);
        PlayerPrefs.Save();

        // Load the game scene
        LoadGameScene();
    }

    /// <summary>
    /// Loads the main game scene.
    /// </summary>
    private void LoadGameScene()
    {
        // Check if the scene name is valid before trying to load it
        if (!string.IsNullOrEmpty(gameSceneName))
        {
            SceneManager.LoadScene(gameSceneName);
        }
        else
        {
            Debug.LogError("Game scene name is not set in MainMenuSaveIntegration.");
        }
    }
}
