using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private GameObject[] Pagine;
    [SerializeField] private MainMenuSaveIntegration menu;
    private static int i = 0;
    [SerializeField] private TextMeshProUGUI button;

    public void Next()
    {
        Pagine[i].SetActive(false);
        i++;
        switch(i)
        {
            case 2:
                button.SetText("Esci");
                break;
            case 3:
                i = 0;
                Pagine[i].SetActive(true);
                button.SetText("Avanti");
                menu.BackToMenu();
                return;
        }
        Pagine[i].SetActive(true);
    }
}
