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
        SaveData curr = SaveManager.Instance.currentSave;
        UpdateInfoUi(curr.grenadeCount, curr.medikitCount, curr.coins);
    }

    public void UpdateInfoUi(int greneade, int medkit, int gold)
    {
        goldText.text = gold.ToString();
        grenadeText.text = greneade.ToString();
        medKitText.text = medkit.ToString();
    }
}
