using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultySetter : MonoBehaviour
{
    [SerializeField] private int difficulty;
    void OnClick()
    {
        PlayerPrefs.SetInt("DifficultyProfile", difficulty);
    }

}
