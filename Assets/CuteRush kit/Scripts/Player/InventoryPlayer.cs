using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryPlayer : MonoBehaviour
{
    private int MedkitCount = 0;
    private int GrenadeCount = 0;
    private int coins = 0;


    public static event Action<int, int> OnInventoryChanged;
    public int getMedkitCount()
    {
        return MedkitCount;
    }

    public int getGrenadeCount()
    {
         return GrenadeCount;
    }

    public void addMedkit()
    {
        MedkitCount++;
    }

    public void addGrenade()
    {
        GrenadeCount++;
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
