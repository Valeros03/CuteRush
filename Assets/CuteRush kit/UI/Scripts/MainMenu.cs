using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : UIPanel
{

    [SerializeField] private GameObject PlayPanel;
    [SerializeField] private UIPlayerUpgradesPanel UpgradePanel;


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
        Bootstrapper.Instance.LoadGameLevel("Fallforest");
    }

    public void OnQuitButtonClicked()
    {
        Application.Quit();
    }
}
