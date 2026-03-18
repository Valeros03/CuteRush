using System;
using UnityEngine;

public class VitalsController : MonoBehaviour
{
    [Header("[Health Settings]")]
    public int maxHealth;
    public int currentHealth;

    public static event Action<int> OnHealthChange;

    public void Start()
    {
        // Impostiamo la salute iniziale e aggiorniamo subito l'interfaccia
        currentHealth = maxHealth;
        OnHealthChange?.Invoke(currentHealth);
    }

    public void Increase(int value)
    {
        currentHealth += value;

        // Blocchiamo la vita al tetto massimo
        if (currentHealth > maxHealth) currentHealth = maxHealth;

        OnHealthChange?.Invoke(currentHealth);
    }

    public void Decrease(int value, Vector3 damageSourcePosition)
    {
        currentHealth -= value;

        // Evitiamo che la vita vada in negativo
        if (currentHealth < 0) currentHealth = 0;

        OnHealthChange?.Invoke(currentHealth);

        // Richiamiamo l'UI Manager per far apparire l'arco rosso!
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowDamageIndicator(damageSourcePosition);
        }
    }
}