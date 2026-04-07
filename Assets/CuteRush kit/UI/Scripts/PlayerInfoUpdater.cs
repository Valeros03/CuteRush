using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerInfoUpdater : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private TextMeshProUGUI grenadeText;
    [SerializeField] private TextMeshProUGUI medKitText;
    void OnEnable()
    {
        UIEvents.OnCoinChanged += UpdateCoinUI;
        SaveData curr = SaveManager.Instance.currentSave;
        if (curr != null)
        {
            UpdateInfoUi(curr.grenadeCount, curr.medikitCount, curr.coins);
        }
    }

    void OnDisable()
    {
        UIEvents.OnCoinChanged -= UpdateCoinUI;
    }

    private void UpdateCoinUI()
    {
        if (SaveManager.Instance != null && SaveManager.Instance.currentSave != null)
        {
            goldText.text = SaveManager.Instance.currentSave.coins.ToString();
        }
    }

    public void UpdateInfoUi(int greneade, int medkit, int gold)
    {
        goldText.text = gold.ToString();
        grenadeText.text = greneade.ToString();
        medKitText.text = medkit.ToString();
    }
}
