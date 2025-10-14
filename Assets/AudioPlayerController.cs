using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayerController : MonoBehaviour
{
    public AudioSource footstepSource;
    public AudioClip runingLoop;


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

}

