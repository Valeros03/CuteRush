using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : UIPanel
{
    [SerializeField] private Image healthImage;
    [SerializeField] private Text healthText;
    [SerializeField] private Color emptyHealthColor;
    [SerializeField] private Color fullHealthColor;

    [SerializeField] private Text medikitText;
    [SerializeField] private Text grenadeText;

    //PUO' ESSERE USATO COME SETTER
    public void UpdateHealth(int val)
   {
        healthImage.fillAmount += val;
        healthImage.color = Color.Lerp(emptyHealthColor, fullHealthColor, healthImage.fillAmount);
        healthText.text = val.ToString();
    }

    //PUO' ESSERE USATO COME SETTER
    public void UpdateInventory(int medikitCount, int grenadeCount)
    {
        if (medikitText != null)
            medikitText.text = medikitCount.ToString();
        if (grenadeText != null)
            grenadeText.text = grenadeCount.ToString();
    }
}
