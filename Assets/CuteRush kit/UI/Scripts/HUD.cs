using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
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

    [SerializeField] private Text InteractText;
    [SerializeField] private Text PickUpText;
    [SerializeField] private Text NotOpenable;

    private static string default_interact = "Premi F per ";
    private static string default_pickup = "Premi F per raccogliere ";

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

    public void ShowInteract(string item)
    {
        InteractText.text += item;
        InteractText.gameObject.SetActive(true);
    }

    public void HideInteract()
    {
        InteractText.text = default_interact;
        InteractText.gameObject.SetActive(false);
    }

    public void ShowPickUp(string item)
    {
        PickUpText.text += item;
        PickUpText.gameObject.SetActive(true);
    }

    public void HidePickUp()
    {
        PickUpText.text = default_pickup;
        PickUpText.gameObject.SetActive(false);
    }

    public void ShowNotOpenable()
    {
        StartCoroutine(nameof(ShowOpen));
    }

    IEnumerator ShowOpen()
    {
        NotOpenable.gameObject.SetActive(true);
        yield return new WaitForSeconds(3);
        NotOpenable.gameObject.SetActive(false);
    }

}
