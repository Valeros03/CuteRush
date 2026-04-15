using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AudioLabelModifier : MonoBehaviour
{

    [Header("Audio Label")]
    [SerializeField] private TextMeshProUGUI Master;
    [SerializeField] private TextMeshProUGUI Music;
    [SerializeField] private TextMeshProUGUI SFX;
    [SerializeField] private TextMeshProUGUI Ambient;

    [Header("Slider")]
    [SerializeField] private Slider SMaster;
    [SerializeField] private Slider SMusic;
    [SerializeField] private Slider SSFX;
    [SerializeField] private Slider SAmbient;

    public void OnEnable()
    {
        AudioSettings a = SaveManager.Instance.currentSave.audioSettings;

        Master.SetText(Mathf.RoundToInt(a.MasterVolume * 100).ToString());
        Music.SetText(Mathf.RoundToInt(a.MusicVolume * 100).ToString());
        SFX.SetText(Mathf.RoundToInt(a.SFXVolume * 100).ToString());
        Ambient.SetText(Mathf.RoundToInt(a.AmbientVolume * 100).ToString());

        SMaster.value = a.MasterVolume;
        SMusic.value = a.MusicVolume;
        SSFX.value = a.SFXVolume;
        SAmbient.value = a.AmbientVolume;
    }

    public void SetMasterVolume(float linear)
    {
        AudioManager.Instance.SetMasterVolume(linear);
        Master.SetText(Mathf.RoundToInt(linear * 100).ToString());
    }
    public void SetMusicVolume(float linear)
    {
        AudioManager.Instance.SetMusicVolume(linear);
        Music.SetText(Mathf.RoundToInt(linear * 100).ToString());
    }

    public void SetSFXVolume(float linear)
    {
        AudioManager.Instance.SetSFXVolume(linear);
        SFX.SetText(Mathf.RoundToInt(linear * 100).ToString());
    }

    public void SetAmbientVolume(float linear)
    {
        AudioManager.Instance.SetAmbientVolume(linear);
        Ambient.SetText(Mathf.RoundToInt(linear * 100).ToString());
    }
}
