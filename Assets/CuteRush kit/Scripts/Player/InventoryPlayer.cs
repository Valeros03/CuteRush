using System;
using UnityEngine;

public class InventoryPlayer : MonoBehaviour
{
    private int MedkitCount = 0;
    private int GrenadeCount = 0;
    private int coins = 0;

    private static int MaxMedkit = 10;
    private static int MaxGrenade = 10;

    // Il tuo evento è perfetto: passa due int (medikit, granate)
    public static event Action<int, int> OnInventoryChanged;

    private void Start()
    {
        // All'avvio del gioco, "urliamo" all'HUD i valori iniziali (es. 0 e 0)
        OnInventoryChanged?.Invoke(MedkitCount, GrenadeCount);
    }

    public int getMedkitCount()
    {
        return MedkitCount;
    }

    public int getGrenadeCount()
    {
        return GrenadeCount;
    }

    // --- MEDIKIT ---
    public bool addMedkit()
    {
        if (MedkitCount < MaxMedkit)
        {
            MedkitCount++;
            // INVIO L'AGGIORNAMENTO ALL'HUD!
            OnInventoryChanged?.Invoke(MedkitCount, GrenadeCount);
            return true;
        }
        return false;
    }

    public bool removeMedkit()
    {
        if (MedkitCount > 0)
        {
            MedkitCount--;
            // INVIO L'AGGIORNAMENTO ALL'HUD!
            OnInventoryChanged?.Invoke(MedkitCount, GrenadeCount);
            return true;
        }
        return false;
    }

    // --- GRANATE ---
    // TRASFORMATO IN BOOL per farlo funzionare con il Totem!
    public bool addGrenade()
    {
        if (GrenadeCount < MaxGrenade)
        {
            GrenadeCount++;
            // INVIO L'AGGIORNAMENTO ALL'HUD!
            OnInventoryChanged?.Invoke(MedkitCount, GrenadeCount);
            return true;
        }
        return false;
    }

    // TRASFORMATO IN BOOL per sicurezza futura
    public bool removeGrenade()
    {
        if (GrenadeCount > 0)
        {
            GrenadeCount--;
            // INVIO L'AGGIORNAMENTO ALL'HUD!
            OnInventoryChanged?.Invoke(MedkitCount, GrenadeCount);
            return true;
        }
        return false;
    }

    // --- MONETE ---
    public void addCoin()
    {
        coins++;
    }

    public void addCoin(int count)
    {
        coins += count;
    }

    public void removeCoin(int count)
    {
        coins -= count;
    }

    public int getCoins()
    {
        return coins;
    }
}