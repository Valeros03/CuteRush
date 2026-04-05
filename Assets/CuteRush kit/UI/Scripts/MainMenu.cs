using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : UIPanel
{

    [SerializeField] private GameObject PlayPanel;
    [SerializeField] private GameObject UpgradePanel;

    public void SwitchToUpgrade()
    {
        if (UpgradePanel.activeInHierarchy) return;
        PlayPanel.SetActive(false);
        UpgradePanel.SetActive(true);
    }

    public void SwitchToPlay()
    {
        if (PlayPanel.activeInHierarchy) return;
        UpgradePanel.SetActive(false);
        PlayPanel.SetActive(true);
    }

    public void OnPlayButtonClicked()
    {
        Bootstrapper.Instance.LoadGameLevel("Fallforest");
    }

    public void OnQuitButtonClicked()
    {
        Application.Quit();
    }
}
