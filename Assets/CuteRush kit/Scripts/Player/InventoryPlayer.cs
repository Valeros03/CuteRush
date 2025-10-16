using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryPlayer : MonoBehaviour
{
    private int MedkitCount = 0;
    private int GrenadeCount = 0;

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
}
