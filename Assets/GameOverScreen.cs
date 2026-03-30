using UnityEngine;
using TMPro;
using System;

public class GameOverScreen : MonoBehaviour
{
    [Header("UI Text References")]
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI killsText;
    [SerializeField] private TextMeshProUGUI baseScoreText;
    [SerializeField] private TextMeshProUGUI kpmBonusText;
    [SerializeField] private TextMeshProUGUI difficultyText;
    [SerializeField] private TextMeshProUGUI finalScoreText;

    [Header("Bilanciamento Gioco")]
    [Tooltip("Quanti punti diamo per ogni Uccisione al Minuto?")]
    public float kpmWeight = 50f;

    public void ShowGameOverStats(float timeSurvivedSeconds, int totalKillPoints, int totalKills, float difficultyMultiplier)
    {
        float timeSurvivedMinutes = timeSurvivedSeconds / 60f;
        if (timeSurvivedMinutes <= 0.01f) timeSurvivedMinutes = 0.01f;

        float kpm = totalKills / timeSurvivedMinutes;

        int kpmBonus = Mathf.FloorToInt(kpm * kpmWeight);

        int subtotal = totalKillPoints + kpmBonus;
        int finalScore = Mathf.FloorToInt(subtotal * difficultyMultiplier);

        TimeSpan timeSpan = TimeSpan.FromSeconds(timeSurvivedSeconds);
        string formattedTime = string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);

        timeText.text = "Tempo Sopravvissuto: " + formattedTime;
        killsText.text = "Nemici Uccisi: " + totalKills + " (" + kpm.ToString("F1") + " kill/min)";
        baseScoreText.text = "Punti Combattimento: " + totalKillPoints;
        kpmBonusText.text = "Bonus Aggressività: +" + kpmBonus;

        string diffName = difficultyMultiplier >= 1.8f ? "Difficile" : (difficultyMultiplier <= 0.5f ? "Facile" : "Media");
        difficultyText.text = $"Moltiplicatore Difficoltà: {diffName} (x{difficultyMultiplier})";

        finalScoreText.text = "PUNTEGGIO FINALE: " + finalScore;

        gameObject.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}