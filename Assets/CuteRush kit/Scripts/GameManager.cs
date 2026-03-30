using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Stato del Gioco")]
    public GameState currentState;
    public int currentScore { get; private set; }

    public event Action<int> OnScoreChange;
    public event Action OnGameOver;

    public int killNumber { get; private set; } = 0;
    public float difficultyMultiplier;
    public int difficultyAdder;

    public enum GameState
    {
        Playing,
        Paused,
        GameOver
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        StartGame();
    }

    public void StartGame()
    {
        currentState = GameState.Playing;
        currentScore = 0;
        Time.timeScale = 1f;
        OnScoreChange?.Invoke(currentScore);
    }

    public void AddKillScore(int points)
    {
        if (currentState != GameState.Playing) return;

        killNumber++;
        currentScore += points + difficultyAdder;
        OnScoreChange?.Invoke(currentScore);
    }

    public void GameOver()
    {
        if (currentState == GameState.GameOver) return;

        currentState = GameState.GameOver;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopAmbient();
            AudioManager.Instance.PlayMusic("MenuSong", true);
        }

        Time.timeScale = 0f;
        OnGameOver?.Invoke();
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}