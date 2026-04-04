using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SaveSlotUI : MonoBehaviour
{
    [Header("Riferimenti Grafici")]
    public TextMeshProUGUI nomePartitaText;

    private string _saveFileName;

    private MainMenuSaveIntegration _menuManager;

    public void SetupSlot(string saveName, MainMenuSaveIntegration menu)
    {
        _menuManager = menu;
        _saveFileName = saveName;
        nomePartitaText.text = saveName;
    }

    public void OnClickSlot()
    {
        _menuManager.SelectProfile(_saveFileName);
    }
}