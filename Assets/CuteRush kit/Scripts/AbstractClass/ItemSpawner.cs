using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public abstract class ItemSpawner : BaseSpawner
{
    [Header("Item Settings")]
    public int amountToGive = 1;

    [Header("Visuals & Audio")]
    public GameObject hologramVisual;
    public AudioSource audioSource;
    public AudioClip pickupSound;

    protected bool isReady = true;

    private void OnTriggerStay(Collider other)
    {
        if (!isReady) return;

        if (other.CompareTag("Player"))
        {
            // Chiamiamo il metodo che ogni figlio implementerà a modo suo
            if (TryGiveItem(other.gameObject))
            {
                ConsumeTotem();
            }
        }
    }

    // Metodo astratto: lascia ai figli la responsabilità di dare l'oggetto
    protected abstract bool TryGiveItem(GameObject player);

    private void ConsumeTotem()
    {
        isReady = false;
        if (hologramVisual != null) hologramVisual.SetActive(false);
        if (audioSource != null && pickupSound != null) audioSource.PlayOneShot(pickupSound);

        StartCoroutine(CooldownRoutine());
    }

    private IEnumerator CooldownRoutine()
    {
        // Aspettiamo usando la variabile ereditata dal BaseSpawner
        yield return new WaitForSeconds(spawnCooldown);

        isReady = true;
        if (hologramVisual != null) hologramVisual.SetActive(true);
    }
}