using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioGunController : MonoBehaviour
{
    public AudioSource GunSource;
    public List<AudioClip> ShootClips;
    public AudioClip Recharge;
    public AudioClip GunCharging;

    public void PlayShoot()
    {
        AudioClip clip = ShootClips[Random.Range(0, ShootClips.Count)];
        GunSource.PlayOneShot(clip);
    }

    public void PlayRecharge()
    {
        if(Recharge != null)
        {
            GunSource.clip = Recharge;
            GunSource.Play();
        }
            
    }

    public void PlayCharge()
    {
        if (GunCharging != null)
        {
            GunSource.clip = GunCharging;
            GunSource.Play();
        }
            
    }

}
