using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class HologramMedkitTotem : MonoBehaviour
{
    [Header("Totem Settings")]
    [Tooltip("Tempo in secondi prima che l'ologramma ricompaia")]
    public float cooldownTime = 10f;
    public int medkitsToGive = 1;

    [Header("Visuals & Audio")]
    [Tooltip("L'oggetto figlio che contiene l'ologramma da spegnere/accendere")]
    public GameObject hologramVisual;
    public AudioSource audioSource;
    public AudioClip pickupSound;

    private bool isReady = true;

    private void OnTriggerStay(Collider other)
    {
        // Se il totem si sta ricaricando, ignora la collisione
        if (!isReady) return;

        if (other.CompareTag("Player"))
        {
            // Cerchiamo direttamente il PlayerController invece del GunBase
            PlayerController player = other.GetComponent<PlayerController>();

            if (player != null)
            {
                bool medkitTaken = false;

                // Proviamo a dare i medikit
                for (int i = 0; i < medkitsToGive; i++)
                {
                    // Ora addMedkit() ci risponde vero o falso!
                    if (player.addMedkit())
                    {
                        medkitTaken = true;
                    }
                }

                // Se abbiamo effettivamente preso almeno un medikit, consumiamo il totem
                if (medkitTaken)
                {
                    GiveMedkitAndStartCooldown();
                }
            }
        }
    }

    private void GiveMedkitAndStartCooldown()
    {
        // SPEGNIMENTO IMMEDIATO per evitare bug di trigger multipli
        isReady = false;
        if (hologramVisual != null) hologramVisual.SetActive(false);

        if (audioSource != null && pickupSound != null)
        {
            audioSource.PlayOneShot(pickupSound);
        }

        StartCoroutine(CooldownRoutine());
    }

    private IEnumerator CooldownRoutine()
    {
        // Aspettiamo i secondi di ricarica
        yield return new WaitForSeconds(cooldownTime);

        // Riaccendiamo tutto
        isReady = true;
        if (hologramVisual != null) hologramVisual.SetActive(true);
    }
}