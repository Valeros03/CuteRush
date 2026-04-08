using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultySetter : MonoBehaviour
{
    [SerializeField] private int difficulty;
    [SerializeField] private MainMenu menu;
    public void OnClick()
    {
        PlayerPrefs.SetInt("DifficultyProfile", difficulty);
        menu.MarkDifficultyAsSelected();
    }

}
