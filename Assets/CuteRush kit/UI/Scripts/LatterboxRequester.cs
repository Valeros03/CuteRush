using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LatterboxRequester : MonoBehaviour
{
    public Animator cameraAnimator;

    private void Start()
    {
        AudioManager.Instance.PlayMusic(GameConstants.AUDIO_MENU_SONG, true);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SkipAnimation();
        }
    }

    public void StartLatterbox()
    {
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
        this.enabled = false; // Stop checking for input once menu is shown naturally
    }

    private void SkipAnimation()
    {
        if (cameraAnimator != null)
        {
            cameraAnimator.Play("CameraIntroAnim", -1, 1f);
            cameraAnimator.Update(0f);
            cameraAnimator.enabled = false;
        }

        UIManager.Instance.ResetLatterbox();
        UIManager.Instance.OpenMenu();

        this.enabled = false; // Stop checking for input
    }
}
