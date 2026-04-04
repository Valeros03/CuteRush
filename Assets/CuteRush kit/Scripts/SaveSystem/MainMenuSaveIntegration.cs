using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuSaveIntegration : UIPanel
{
    [Header("Pannelli UI")]
    [Tooltip("Il pannello che contiene i bottoni principali (Nuova Partita, Carica, Esci)")]
    public GameObject mainButtonsPanel;

    [Tooltip("Il pannello dove inserisci il nome per la nuova partita")]
    public GameObject newGamePanel;

    [Tooltip("Il pannello che contiene la lista dei profili salvati")]
    public GameObject loadGamePanel;

    [Header("Input Dati")]
    public TMP_InputField usernameInputField;


    [Header("Save Slot Prefab")]
    [SerializeField] private GameObject _saveSlotPrefab;
    [Header("Save scrolview")]
    [SerializeField] private Transform _saveScrolview;
    private string _selectedSaveName;

    private void Start()
    {
        ShowMainPanel();
    }


    public void ShowMainPanel()
    {
        mainButtonsPanel.SetActive(true);
        newGamePanel.SetActive(false);
        loadGamePanel.SetActive(false);
    }

    public void ShowNewGamePanel()
    {
        mainButtonsPanel.SetActive(false);
        newGamePanel.SetActive(true);
        loadGamePanel.SetActive(false);
    }

    public void ShowLoadGamePanel()
    {
        mainButtonsPanel.SetActive(false);
        newGamePanel.SetActive(false);
        loadGamePanel.SetActive(true);
        loadProfileList();
    }

    public void SelectProfile(string name)
    {
        _selectedSaveName = name;
    }

    public void HandleNewGame(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            Debug.LogWarning("Username cannot be empty.");
            return;
        }

        List<string> existingProfiles = SaveManager.Instance.GetAllSavedProfiles();
        if (existingProfiles.Contains(username))
        {
            Debug.LogWarning($"A profile with the username '{username}' already exists. Please choose a different name or load the existing profile.");
            return;
        }

        SaveManager.Instance.CreateNewGame(username);

        PlayerPrefs.SetString("LastUsername", username);
        PlayerPrefs.Save();

        UIManager.Instance.StartGameMenu();

        Bootstrapper.Instance.LoadMainMenuLand();
    }

    public List<string> GetProfilesForUI()
    {
        return SaveManager.Instance.GetAllSavedProfiles();
    }

    public void HandleLoadGame()
    {
        if (string.IsNullOrWhiteSpace(_selectedSaveName))
        {
            Debug.LogWarning("Profile name to load is empty.");
            return;
        }

        SaveManager.Instance.LoadGame(_selectedSaveName);

        PlayerPrefs.SetString("LastUsername", _selectedSaveName);
        PlayerPrefs.Save();

        UIManager.Instance.StartGameMenu();
        Bootstrapper.Instance.LoadMainMenuLand();

    }

    public void SubmitNewGameProfile()
    {
        string typedName = usernameInputField.text;
        HandleNewGame(typedName);
    }

    private void loadProfileList()
    {
        List<string> profileNames = SaveManager.Instance.GetAllSavedProfiles();
        foreach (string name in profileNames)
        {
            GameObject newSlotObj = Instantiate(_saveSlotPrefab, _saveScrolview);
            SaveSlotUI slotScript = newSlotObj.GetComponent<SaveSlotUI>();
            slotScript.SetupSlot(name, this);
        }
    }

}