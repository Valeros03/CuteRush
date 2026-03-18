using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class HologramAmmoTotem : MonoBehaviour
{
    [Header("Totem Settings")]
    [Tooltip("Tempo in secondi prima che l'ologramma ricompaia")]
    public float cooldownTime = 10f;
    public int magsToGive = 1;

    [Header("Visuals & Audio")]
    [Tooltip("L'oggetto figlio che contiene l'ologramma da spegnere/accendere")]
    public GameObject hologramVisual;
    public AudioSource audioSource;
    public AudioClip pickupSound; // Suono quando prendi le munizioni

    private bool isReady = true;

    private void OnTriggerStay(Collider other)
    {
        if (!isReady) return;

        if (other.CompareTag("Player"))
        {
            GunBase playerGun = other.GetComponentInChildren<GunBase>();

            if (playerGun != null)
            {
                bool ammoTaken = false;

                for (int i = 0; i < magsToGive; i++)
                {
                    if (playerGun.addMag())
                    {
                        ammoTaken = true;
                    }
                }

                if (ammoTaken)
                {
                    GiveAmmoAndStartCooldown();
                }
            }
        }
    }

    private void GiveAmmoAndStartCooldown()
    {
        if (audioSource != null && pickupSound != null)
        {
            audioSource.PlayOneShot(pickupSound);
        }

        StartCoroutine(CooldownRoutine());
    }

    private IEnumerator CooldownRoutine()
    {
        isReady = false;

        if (hologramVisual != null) hologramVisual.SetActive(false);

        yield return new WaitForSeconds(cooldownTime);

        isReady = true;
        if (hologramVisual != null) hologramVisual.SetActive(true);

    }
}