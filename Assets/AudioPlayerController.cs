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

}

