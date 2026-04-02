using UnityEngine;

[CreateAssetMenu(fileName = "NewDifficulty", menuName = "Game/Difficulty Profile")]
public class DifficultyProfile : ScriptableObject
{
    public string difficultyName;

    [Header("Scaling System")]
    [Tooltip("1.0 = normale, 0.8 = facile, 1.2 = difficile")]
    public float startingMultiplier = 1.0f;
    [Tooltip("Di quanto aumenta il moltiplicatore ogni step (es. minuto)")]
    public float scalingIncrement = 0.1f;

    [Header("Economy & Resources")]
    [Tooltip("Moltiplicatore per le probabilità di Drop")]
    public float dropRateMultiplier = 1.0f;
    [Tooltip("Moltiplicatore per l'oro guadagnato")]
    public float goldEarnedMultiplier = 1.0f;
    [Tooltip("Moltiplicatore per punti")]
    public float scoreMultiplyer = 1.0f;

}