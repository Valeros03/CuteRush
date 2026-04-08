using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : UIPanel
{
    [Header("Panels")]
    [SerializeField] private GameObject PlayPanel;
    [SerializeField] private UIPlayerUpgradesPanel UpgradePanel;

    [Header("Map Selection")]
    public Toggle[] mapToggles;
    public string selectedMapName = "";

    [Header("UI Feedback")]
    [SerializeField] private GameObject errorMissingSelectionText;

    private bool isMapExplicitlySelected = false;
    private bool isDifficultyExplicitlySelected = false;

    
    public void OnEnable()
    {
        if (errorMissingSelectionText != null)
            errorMissingSelectionText.SetActive(false);

        foreach (Toggle toggle in mapToggles)
        {
            toggle.onValueChanged.AddListener((bool isOn) =>
            {
                if (isOn)
                {
                    MapToggleData data = toggle.GetComponent<MapToggleData>();
                    if (data != null)
                    {
                        selectedMapName = data.mapNameDefinition;

                        isMapExplicitlySelected = true;

                        if (errorMissingSelectionText != null)
                            errorMissingSelectionText.SetActive(false);
                    }
                }
            });
        }
    }

    public void MarkDifficultyAsSelected()
    {
        isDifficultyExplicitlySelected = true;

        if (errorMissingSelectionText != null)
            errorMissingSelectionText.SetActive(false);
    }

    public void SwitchToUpgrade()
    {
        if (UpgradePanel.gameObject.activeInHierarchy) return;
        PlayPanel.SetActive(false);
        UpgradePanel.Show();
    }

    public void SwitchToPlay()
    {
        if (PlayPanel.activeInHierarchy) return;
        UpgradePanel.Hide();
        PlayPanel.SetActive(true);
    }

    public void OnPlayButtonClicked()
    {
        Debug.Log("");
        //if (!isMapExplicitlySelected || !isDifficultyExplicitlySelected)
        if (!isDifficultyExplicitlySelected)
        {
            if (errorMissingSelectionText != null)
            {
                errorMissingSelectionText.SetActive(true);
            }
            Debug.Log("difficoltà non selezionata");
            return;
        }

        Hide();
        Bootstrapper.Instance.LoadGameLevel(selectedMapName);
    }

    public void OnQuitButtonClicked()
    {
        Application.Quit();
    }

}