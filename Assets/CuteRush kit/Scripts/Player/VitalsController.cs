using System;
using UnityEngine;

public class VitalsController : MonoBehaviour
{
    [Header("[Health Settings]")]
    public int maxHealth;
    public int currentHealth;

    public static event Action<int, int> OnHealthChange;

    // Riferimento al nostro nuovo manager dell'audio
    private AudioPlayerController audioController;

    public void Start()
    {
        currentHealth = maxHealth;

        // Cerca in automatico lo script AudioPlayerController attaccato allo stesso oggetto
        audioController = GetComponent<AudioPlayerController>();

        OnHealthChange?.Invoke(currentHealth, maxHealth);
    }

    public void Increase(int value)
    {
        currentHealth += value;
        if (currentHealth > maxHealth) currentHealth = maxHealth;

        OnHealthChange?.Invoke(currentHealth, maxHealth);
    }

    public void Decrease(int value, Vector3 damageSourcePosition, bool isPhysical = false)
    {
        currentHealth -= value;
        if (currentHealth < 0) currentHealth = 0;

        OnHealthChange?.Invoke(currentHealth, maxHealth);

        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowDamageIndicator(damageSourcePosition);
        }

        // Diciamo all'AudioController di fare il suo lavoro!
        if (audioController != null)
        {
            audioController.PlayDamageSound(isPhysical);
        }
    }
}