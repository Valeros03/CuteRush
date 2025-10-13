using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFootsteps : MonoBehaviour
{
    public AudioSource footstepSource;
    public List<AudioClip> footstepClips;

    public void PlayFootstep()
    {
        AudioClip clip = footstepClips[Random.Range(0, footstepClips.Count)];
        footstepSource.PlayOneShot(clip);
    }
}

