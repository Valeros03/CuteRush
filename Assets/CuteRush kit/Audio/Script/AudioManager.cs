using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;
    public AudioSource ambientSource;

    [Header("Audio Mixer")]
    public AudioMixer mixer;
    // I nomi dei parametri esposti nel Mixer
    private const string MUSIC_VOL_PARAM = "MusicVol";
    private const string SFX_VOL_PARAM = "SFXVol";
    private const string AMBIENT_VOL_PARAM = "AmbientVol";

    [Header("Clips")]
    public List<AudioClip> musicTracks;
    public List<AudioClip> sfxClips;
    public List<AudioClip> ambientClips;

    private Dictionary<string, AudioClip> musicDict;
    private Dictionary<string, AudioClip> sfxDict;
    private Dictionary<string, AudioClip> ambientDict;

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            musicDict = new Dictionary<string, AudioClip>();
            sfxDict = new Dictionary<string, AudioClip>();
            ambientDict = new Dictionary<string, AudioClip>();

            foreach (var clip in musicTracks)
                musicDict[clip.name] = clip;

            foreach (var clip in sfxClips)
                sfxDict[clip.name] = clip;

            foreach (var clip in ambientClips)
                ambientDict[clip.name] = clip;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 🎵 Riproduci musica
    public void PlayMusic(string trackName, bool loop = true)
    {
        if (musicDict.TryGetValue(trackName, out AudioClip clip))
        {
            musicSource.clip = clip;
            musicSource.loop = loop;
            musicSource.Play();
        }
        else
        {
            Debug.Log($"[AudioManager] Musica '{trackName}' non trovata!");
        }
    }

    // 🔊 Riproduci effetto sonoro
    public void PlaySFX(string clipName)
    {
        if (sfxDict.TryGetValue(clipName, out AudioClip clip))
        {
            sfxSource.PlayOneShot(clip);
        }
        else
        {
            Debug.Log($"[AudioManager] SFX '{clipName}' non trovato!");
        }
    }

    // 🌫️ Riproduci suono ambientale
    public void PlayAmbient(string ambientName, bool loop = true)
    {
        if (ambientDict.TryGetValue(ambientName, out AudioClip clip))
        {
            ambientSource.clip = clip;
            ambientSource.loop = loop;
            ambientSource.Play();
        }
        else
        {
            Debug.Log($"[AudioManager] Ambient '{ambientName}' non trovato!");
        }
    }

    // 🔈 Controllo volumi tramite AudioMixer
    // (conversione lineare -> decibel)
    public void SetMusicVolume(float linear)
    {
        mixer.SetFloat(MUSIC_VOL_PARAM, LinearToDecibel(linear));
    }

    public void SetSFXVolume(float linear)
    {
        mixer.SetFloat(SFX_VOL_PARAM, LinearToDecibel(linear));
    }

    public void SetAmbientVolume(float linear)
    {
        mixer.SetFloat(AMBIENT_VOL_PARAM, LinearToDecibel(linear));
    }

    // 🔁 Conversione helper
    private float LinearToDecibel(float linear)
    {
        // Evita -∞ quando linear è 0
        return Mathf.Log10(Mathf.Clamp(linear, 0.0001f, 1f)) * 20f;
    }
}
