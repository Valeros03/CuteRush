using UnityEngine;
using TMPro;
using System;
using System.Text;
using System.Linq;
using UnityEngine.SceneManagement;

public class GameOverScreen : UIPanel
{
    [Header("Stats References")]
    [SerializeField] private GameObject statsContainer;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI killsText;
    [SerializeField] private TextMeshProUGUI baseScoreText;
    [SerializeField] private TextMeshProUGUI kpmBonusText;
    [SerializeField] private TextMeshProUGUI difficultyText;
    [SerializeField] private TextMeshProUGUI finalScoreText;

    [Header("Leaderboard References")]
    [SerializeField] private GameObject leaderboardContainer;
    [SerializeField] private TextMeshProUGUI personalLeaderboardText;
    [SerializeField] private TextMeshProUGUI globalLeaderboardText;

    [Header("Bilanciamento Gioco")]
    [Tooltip("Quanti punti diamo per ogni Uccisione al Minuto?")]
    public float kpmWeight = 50f;

    public void ShowGameOverStats(float timeSurvivedSeconds, int totalKillPoints, int totalKills, float difficultyMultiplier, float kpmMult)
    {
        kpmWeight = kpmMult;
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

        if (statsContainer != null) statsContainer.SetActive(true);
        if (leaderboardContainer != null) leaderboardContainer.SetActive(false);

        gameObject.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ShowScoreBoard()
    {
        if (statsContainer != null) statsContainer.SetActive(true);
        if (leaderboardContainer != null) leaderboardContainer.SetActive(false);
    }
    public void ShowLeaderboards()
    {
        if (statsContainer != null)
        {
            statsContainer.SetActive(false);
        }

        if (leaderboardContainer != null)
        {
            leaderboardContainer.SetActive(true);
        }

        if (personalLeaderboardText != null) personalLeaderboardText.gameObject.SetActive(true);
        if (globalLeaderboardText != null) globalLeaderboardText.gameObject.SetActive(true);

        RefreshLeaderboards();
    }

    private void RefreshLeaderboards()
    {
        if (SaveManager.Instance == null)
        {
            Debug.LogError("SaveManager.Instance is null. Cannot refresh leaderboards.");
            return;
        }

        string currentMapName = SceneManager.GetActiveScene().name;

        if (SaveManager.Instance.currentSave != null && SaveManager.Instance.currentSave.mapLeaderboards != null)
        {
            var personalMap = SaveManager.Instance.currentSave.mapLeaderboards.FirstOrDefault(m => m.mapName == currentMapName);
            if (personalMap != null && personalMap.topScores != null && personalMap.topScores.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < personalMap.topScores.Count; i++)
                {
                    sb.AppendLine($"{i + 1}. {personalMap.topScores[i].score} ({personalMap.topScores[i].difficulty})");
                }
                if (personalLeaderboardText != null)
                {
                    personalLeaderboardText.text = sb.ToString();
                }
            }
            else
            {
                if (personalLeaderboardText != null)
                {
                    personalLeaderboardText.text = "Nessun punteggio personale ancora.";
                }
            }
        }
        else if (personalLeaderboardText != null)
        {
            personalLeaderboardText.text = "Nessun punteggio personale ancora.";
        }

        if (SaveManager.Instance.globalLeaderboard != null && SaveManager.Instance.globalLeaderboard.maps != null)
        {
            var globalMap = SaveManager.Instance.globalLeaderboard.maps.FirstOrDefault(m => m.mapName == currentMapName);
            if (globalMap != null && globalMap.topScores != null && globalMap.topScores.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < globalMap.topScores.Count; i++)
                {
                    sb.AppendLine($"{i + 1}. {globalMap.topScores[i].playerName} - {globalMap.topScores[i].score} ({globalMap.topScores[i].difficulty})");
                }
                if (globalLeaderboardText != null)
                {
                    globalLeaderboardText.text = sb.ToString();
                }
            }
            else
            {
                if (globalLeaderboardText != null)
                {
                    globalLeaderboardText.text = "Nessun punteggio globale ancora.";
                }
            }
        }
        else if (globalLeaderboardText != null)
        {
            globalLeaderboardText.text = "Nessun punteggio globale ancora.";
        }
    }

    public void backMainMenu()
    {
        gameObject.SetActive(false);
        Bootstrapper.Instance.LoadMainMenuLand();
    }
}