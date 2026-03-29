using UnityEngine;
using System.Collections.Generic;

public class UiAcidoBorico : MonoBehaviour
{
    [Header("Generazione Dinamica")]
    public GameObject barPrefab;
    public Transform barsContainer;

    [Header("Colori")]
    public Color chargedColor = Color.green;
    public Color chargingColor = Color.blue;
    public Color emptyColor = new Color(0.2f, 0.2f, 0.2f, 0.5f);

    private int currentCharges = 0;
    private List<AcidBarItem> activeBars = new List<AcidBarItem>();


    public void SetupMaxBars(int maxBars)
    {
        foreach (Transform child in barsContainer)
        {
            Destroy(child.gameObject);
        }
        activeBars.Clear();

        for (int i = 0; i < maxBars; i++)
        {
            GameObject newBarObj = Instantiate(barPrefab, barsContainer);
            AcidBarItem barScript = newBarObj.GetComponent<AcidBarItem>();
            activeBars.Add(barScript);
        }
    }

    public void UpdateCharges(int charges)
    {
        currentCharges = charges;

        for (int i = 0; i < activeBars.Count; i++)
        {
            if (i < currentCharges)
            {
                activeBars[i].barImage.fillAmount = 1f;
                activeBars[i].barImage.color = chargedColor;
                if (activeBars[i].percentText != null) activeBars[i].percentText.text = "100%";
            }
            else
            {
                activeBars[i].barImage.fillAmount = 0f;
                activeBars[i].barImage.color = emptyColor;
                if (activeBars[i].percentText != null) activeBars[i].percentText.text = "0%";
            }
        }
    }

    public void UpdateProgress(float progress)
    {
        if (currentCharges < activeBars.Count)
        {
            int chargingIndex = currentCharges;

            activeBars[chargingIndex].barImage.color = chargingColor;
            activeBars[chargingIndex].barImage.fillAmount = progress;

            if (activeBars[chargingIndex].percentText != null)
            {
                int percentValue = Mathf.RoundToInt(progress * 100f);
                activeBars[chargingIndex].percentText.text = percentValue + "%";
            }
        }
    }
}