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

    private const string MUSIC_VOL_PARAM = GameConstants.AUDIO_MIXER_MUSIC;
    private const string SFX_VOL_PARAM = GameConstants.AUDIO_MIXER_SFX;
    private const string AMBIENT_VOL_PARAM = GameConstants.AUDIO_MIXER_AMBIENT;

    [Header("Clips")]
    public List<AudioClip> musicTracks;
    public List<AudioClip> sfxClips;
    public List<AudioClip> ambientClips;

    private Dictionary<string, AudioClip> musicDict;
    private Dictionary<string, AudioClip> sfxDict;
    private Dictionary<string, AudioClip> ambientDict;

    void Awake()
    {
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

    public void StopAmbient()
    {
        ambientSource.Stop();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void PlayMusic(string trackName, bool loop = true)
    {
        if (musicDict.TryGetValue(trackName, out AudioClip clip))
        {
            musicSource.clip = clip;
            musicSource.loop = loop;
            musicSource.Play();
        }
    }

    public void PlaySFX(string clipName)
    {
        if (sfxDict.TryGetValue(clipName, out AudioClip clip))
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    public void PlayAmbient(string ambientName, bool loop = true)
    {
        if (ambientDict.TryGetValue(ambientName, out AudioClip clip))
        {
            ambientSource.clip = clip;
            ambientSource.loop = loop;
            ambientSource.Play();
        }
    }

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

    private float LinearToDecibel(float linear)
    {
        return Mathf.Log10(Mathf.Clamp(linear, 0.0001f, 1f)) * 20f;
    }
}
