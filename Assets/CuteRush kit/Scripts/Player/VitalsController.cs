using System;
using System.Collections;
using System.Collections.Generic;
using Unity.UNetWeaver;
using UnityEngine;
using UnityEngine.UI;

public class VitalsController : MonoBehaviour
{
    [Header("[Health Settings]")]
    public int maxHealth;
    public int currentHealth;

    public static Action<int> OnHealthChange;
    public void Start()
    {
        // Health start settings
        currentHealth = maxHealth;

    }

    private void Update()
    {
        if (currentHealth <= 0)
        {
            currentHealth = 0;
        }
        
        if (currentHealth >= maxHealth)
            currentHealth = maxHealth;

    }

   public void Increase(int value)
    {

       currentHealth += value;
       
    }

    public void Decrease(int value)
    {
       currentHealth -= value;       
    }

  
}
