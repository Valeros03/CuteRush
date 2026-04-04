using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LatterboxRequester : MonoBehaviour
{
    public void StartLatterbox()
    {
        AudioManager.Instance.PlayMusic(GameConstants.AUDIO_MENU_SONG, true);
        UIManager.Instance.ShowLatterbox();
    }

    public void FadeLatterbox()
    {
        UIManager.Instance.FadeLatterbox();
    }

    public void ShowMainMenu()
    {
        UIManager.Instance.OpenMenu();
        UIManager.Instance.ResetLatterbox();
    }

}
