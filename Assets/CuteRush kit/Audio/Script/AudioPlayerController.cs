using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayerController : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource footstepSource;
    private AudioSource playerSource;

    [Header("Movement & Actions")]
    public AudioClip runingLoop;
    public List<AudioClip> sfxClips;
    private Dictionary<string, AudioClip> sfxDict;

    [Header("Damage Sounds")]
    public AudioClip[] physicalHitSounds;
    public AudioClip fireHitSound;
    public float soundCooldown = 0.08f;
    private float lastSoundTime;

    void Awake()
    {
        sfxDict = new Dictionary<string, AudioClip>();
        foreach (AudioClip clip in sfxClips)
        {
            sfxDict[clip.name] = clip;
        }
        playerSource = GetComponent<AudioSource>();
    }

    // --- METODI ESISTENTI ---
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
        if (sfxDict.ContainsKey("throwGranade"))
        {
            playerSource.clip = sfxDict["throwGranade"];
            playerSource.Play();
        }
    }

    // --- NUOVO METODO PER I DANNI ---
    public void PlayDamageSound(bool isPhysical)
    {
        // Controllo Anti-Spam (Cooldown)
        if (Time.time - lastSoundTime >= soundCooldown)
        {
            lastSoundTime = Time.time;

            if (playerSource != null)
            {
                if (isPhysical && physicalHitSounds != null && physicalHitSounds.Length > 0)
                {
                    // Danno Fisico: Variazione casuale del suono + Pitch normale
                    int randomIndex = Random.Range(0, physicalHitSounds.Length);
                    playerSource.pitch = 1f;
                    playerSource.PlayOneShot(physicalHitSounds[randomIndex]);
                }
                else if (!isPhysical && fireHitSound != null)
                {
                    // Danno Fuoco: Suono fisso + Variazione casuale del pitch
                    playerSource.pitch = Random.Range(0.85f, 1.15f);
                    playerSource.PlayOneShot(fireHitSound);
                }
            }
        }
    }
}