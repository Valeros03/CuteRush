using System;
using UnityEngine;

public class InventoryPlayer : MonoBehaviour
{
    private int MedkitCount = 0;
    private int GrenadeCount = 0;
    private int coins = 0;

    private static int MaxMedkit = 10;
    private static int MaxGrenade = 10;

    public static event Action<int, int> OnInventoryChanged;

    private void Start()
    {
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

    public bool addMedkit()
    {
        if (MedkitCount < MaxMedkit)
        {
            MedkitCount++;
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
            OnInventoryChanged?.Invoke(MedkitCount, GrenadeCount);
            return true;
        }
        return false;
    }

    public bool addGrenade()
    {
        if (GrenadeCount < MaxGrenade)
        {
            GrenadeCount++;
            OnInventoryChanged?.Invoke(MedkitCount, GrenadeCount);
            return true;
        }
        return false;
    }

    public bool removeGrenade()
    {
        if (GrenadeCount > 0)
        {
            GrenadeCount--;
            OnInventoryChanged?.Invoke(MedkitCount, GrenadeCount);
            return true;
        }
        return false;
    }

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