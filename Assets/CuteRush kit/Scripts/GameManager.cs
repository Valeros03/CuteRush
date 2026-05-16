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

    [SerializeField] private GameObject Player;
    [SerializeField] private DifficultyProfile[] difficulties;

    public string mapName;

    public int killNumber { get; private set; } = 0;
    public int difficultyAdder;

    public DifficultyProfile currentDifficulty;
    

    private int totalSecondsElapsed = 0;
    private Coroutine timerCoroutine;
    private float levelStartTime = 0f;
    private int finalScore = 0;

    public event Action<int, int> OnTimeUpdated;
    [SerializeField] private string backgroundSound;

    public enum GameState
    {
        Playing,
        Paused,
        GameOver
    }

    private void Awake()
    {
        currentDifficulty = difficulties[PlayerPrefs.GetInt("DifficultyProfile")];
        Debug.Log(currentDifficulty);
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        AudioManager.Instance.PlayAmbient(backgroundSound, true);
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


        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
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
        currentScore += points + Mathf.RoundToInt(difficultyAdder*currentDifficulty.scoreMultiplier);
        OnScoreChange?.Invoke(currentScore);
    }

    public void GameOver()
    {
        if (currentState == GameState.GameOver) return;

        currentState = GameState.GameOver;
        StopLevelTimer();

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopAmbient();
            AudioManager.Instance.PlayMusic(GameConstants.AUDIO_IN_GAME_SONG, true);
        }

        Time.timeScale = 0f;
        finalScore = Mathf.FloorToInt((currentScore + Mathf.FloorToInt((killNumber / Mathf.Max((Time.time - levelStartTime) / 60f, 0.01f)) * currentDifficulty.kpmMultiplier)) * currentDifficulty.scoreMultiplier);
        SaveLastMatch();
        UIManager.Instance.EndGameSequence(Time.time-levelStartTime, currentScore, killNumber, currentDifficulty.startingMultiplier, currentDifficulty.kpmMultiplier);

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

    private void SaveLastMatch()
    {
        InventoryPlayer inventory = Player.GetComponentInChildren<InventoryPlayer>();
        if (inventory != null)
        {
            SaveManager.Instance.currentSave.coins += inventory.getGold();
            SaveManager.Instance.currentSave.medikitCount = inventory.getSavedMedkitsToKeep();
            SaveManager.Instance.currentSave.grenadeCount = inventory.getSavedGrenadesToKeep();
        }
        SaveManager.Instance.SubmitScore(mapName, finalScore, currentDifficulty.difficultyName);

    }
}