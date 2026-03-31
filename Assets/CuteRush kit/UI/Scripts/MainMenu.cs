using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : UIPanel
{
    public void OnPlayButtonClicked()
    {
        Bootstrapper.Instance.LoadGameLevel("Medium");
    }

    public void OnQuitButtonClicked()
    {
        Application.Quit();
    }
}
