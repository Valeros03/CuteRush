using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LatterboxRequester : MonoBehaviour
{
    public Animator cameraAnimator;
    private bool isSkipped = false;
    private bool isAnimationFinished = false;

    private void Update()
    {
        if (!isSkipped && !isAnimationFinished && Input.GetMouseButtonDown(0))
        {
            SkipAnimation();
        }
    }

    public void StartLatterbox()
    {
        if (isSkipped) return;
        AudioManager.Instance.PlayMusic(GameConstants.AUDIO_MENU_SONG, true);
        UIManager.Instance.ShowLatterbox();
    }

    public void FadeLatterbox()
    {
        if (isSkipped) return;
        UIManager.Instance.FadeLatterbox();
    }

    public void ShowMainMenu()
    {
        if (isSkipped) return;
        isAnimationFinished = true;
        UIManager.Instance.OpenMenu();
        UIManager.Instance.ResetLatterbox();
    }

    private void SkipAnimation()
    {
        isSkipped = true;

        if (cameraAnimator != null)
        {
            cameraAnimator.enabled = false;
        }

        // Ensure the menu song is playing
        AudioManager.Instance.PlayMusic(GameConstants.AUDIO_MENU_SONG, true);

        // Show the main menu directly
        UIManager.Instance.OpenMenu();
        UIManager.Instance.ResetLatterbox();
    }
}
