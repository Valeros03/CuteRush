using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VitalsController : MonoBehaviour
{
    [Header("[Health Settings]")]
    public int maxHealth;
    public int currentHealth;
    public Image healthImage;
    public Text healthTextQty;
    public Color fullHealthColor;
    public Color emptyHealthColor;

    public void Start()
    {
        // Health start settings
        currentHealth = maxHealth;
        healthImage.fillAmount = maxHealth / 100;
        healthImage.color = fullHealthColor;
        healthTextQty.text = maxHealth.ToString();

    }

    private void Update()
    {
        if (currentHealth <= 0)
            currentHealth = 0;
        if (currentHealth >= maxHealth)
            currentHealth = maxHealth;

    }

   public void Increase(int value, string type)
    {
        if(type == "health")
        {
            ChangeSliderValue(value / 100f, "health");
            currentHealth += value;
        }
    }

    public void Decrease(int value, string type)
    {
        if (type == "health")
        {
            ChangeSliderValue(-value / 100f, "health");
            currentHealth -= value;
        }
    }

    public void ChangeSliderValue(float value, string type)
    {     
        if(type == "health")
        {
            healthImage.fillAmount += value;
            healthImage.color = Color.Lerp(emptyHealthColor, fullHealthColor, healthImage.fillAmount);
            healthTextQty.text = currentHealth.ToString();
        } 
    }
}
