using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    public static DifficultyManager Instance { get; private set; }

    public float currentMultiplier { get; private set; }
    public float timeStep = 60f;

    private DifficultyProfile profile;

    private void Awake()
    {
        Instance = this;
    }

    public void InitManager()
    {
        profile = GameManager.Instance.currentDifficulty;
        currentMultiplier = profile.startingMultiplier;
        StartCoroutine(DifficultyCurveRoutine());
    }

    private IEnumerator DifficultyCurveRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeStep);
            currentMultiplier += profile.scalingIncrement;
        }
    }
}