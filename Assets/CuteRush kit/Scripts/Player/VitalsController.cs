using System;
using UnityEngine;

public class VitalsController : MonoBehaviour
{
    [Header("[Health Settings]")]
    public int maxHealth;
    public int currentHealth;
    public int medKitHeal = 50;

    public static event Action<int, int> OnHealthChange;

    private AudioPlayerController audioController;

    public void Start()
    {
        currentHealth = maxHealth;

        audioController = GetComponent<AudioPlayerController>();

        OnHealthChange?.Invoke(currentHealth, maxHealth);
    }

    public void UseMedikit()
    {
        Increase(medKitHeal);
        audioController.PlayHealSound();
    }

    private void Increase(int value)
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

        if (audioController != null)
        {
            audioController.PlayDamageSound(isPhysical);
        }
    }
}