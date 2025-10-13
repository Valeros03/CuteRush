using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioGunController : MonoBehaviour
{
    public AudioSource GunSource;
    public List<AudioClip> ShootClips;
    public AudioClip Recharge;

    public void PlayShoot()
    {
        AudioClip clip = ShootClips[Random.Range(0, ShootClips.Count)];
        GunSource.PlayOneShot(clip);
    }


}
