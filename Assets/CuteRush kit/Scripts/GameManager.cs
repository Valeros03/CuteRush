using System;
using System.Collections;
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

    public DifficultyProfile currentDifficulty;

    private int totalSecondsElapsed = 0;
    private Coroutine timerCoroutine;
    private float levelStartTime = 0f;

    public event Action<int, int> OnTimeUpdated;

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

        ResetLevelTimer();
        levelStartTime = Time.time;
        timerCoroutine = StartCoroutine(TimerRoutine());
    }

    public void StopLevelTimer()
    {
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
            timerCoroutine = null;
        }
    }

    public void AddKillScore(int points)
    {
        if (currentState != GameState.Playing) return;

        killNumber++;
        currentScore += points + Mathf.RoundToInt(difficultyAdder*difficultyMultiplier);
        OnScoreChange?.Invoke(currentScore);
    }

    public void GameOver()
    {
        if (currentState == GameState.GameOver) return;

        currentState = GameState.GameOver;
        StopLevelTimer();

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopAmbient();
            AudioManager.Instance.PlayMusic("MenuSong", true);
        }

        Time.timeScale = 0f;
        
        UIManager.Instance.EndGameSequence(Time.time-levelStartTime, currentScore, killNumber, difficultyMultiplier);

    }

    public void ResetLevelTimer()
    {
        totalSecondsElapsed = 0;
        OnTimeUpdated?.Invoke(0, 0);
    }

    private IEnumerator TimerRoutine()
    {
        float startTime = Time.time;
        totalSecondsElapsed = 0;

        while (true)
        {
            totalSecondsElapsed++;
            float targetTickTime = startTime + totalSecondsElapsed;

            float exactWaitTime = targetTickTime - Time.time;

            if (exactWaitTime > 0)
            {
                yield return new WaitForSeconds(exactWaitTime);
            }
            else
            {
                yield return null;
            }

            int minutes = totalSecondsElapsed / 60;
            int seconds = totalSecondsElapsed % 60;
            OnTimeUpdated?.Invoke(minutes, seconds);
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}