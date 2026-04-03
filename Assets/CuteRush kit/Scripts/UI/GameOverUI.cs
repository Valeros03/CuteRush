using UnityEngine;
using TMPro;
using System.Text;
using UnityEngine.SceneManagement;
using System.Linq;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI personalLeaderboardText;
    [SerializeField] private TextMeshProUGUI globalLeaderboardText;

    private void OnEnable()
    {
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

        // Fetch personal scores
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
                    personalLeaderboardText.text = "No personal scores yet.";
                }
            }
        }

        // Fetch global scores
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
                    globalLeaderboardText.text = "No global scores yet.";
                }
            }
        }
    }
}
