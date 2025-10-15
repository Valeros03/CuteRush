using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayerController : MonoBehaviour
{
    public AudioSource footstepSource;
    public AudioClip runingLoop;
    public List<AudioClip> sfxClips;

    private AudioSource playerSource;
    private Dictionary<string, AudioClip> sfxDict;

    void Awake()
    {
        sfxDict = new Dictionary<string, AudioClip>();
        foreach(AudioClip clip in sfxClips)
        {
            sfxDict[clip.name] = clip;
        }
        playerSource = GetComponent<AudioSource>();
    }

    public void PlayFootstep()
    {
        footstepSource.clip = runingLoop;
        footstepSource.Play();
    }

    public void StopFootstep()
    {
        footstepSource.Stop();
    }

    public void runMode()
    {
        footstepSource.pitch = 1.2f;
    }
    public void walkMode()
    {
        footstepSource.pitch = 1f;
    }

    public void playThrow()
    {
        playerSource.clip = sfxDict["throwGranade"];
        playerSource.Play();

    }

}

