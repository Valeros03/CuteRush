using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Bootstrapper : MonoBehaviour
{
    public static Bootstrapper Instance { get; private set; }

    public string uiSceneName = "UI_scene";
    public string mainMenuEnvironment = "MainMenuLand";

    private string currentLoadedEnvironment = "";

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.LoadScene(uiSceneName, LoadSceneMode.Additive);
    }

    void Start()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayMusic(GameConstants.AUDIO_IN_GAME_SONG);

    }

    public void LoadMainMenuLand()
    {
        StartCoroutine(TransitionSceneRoutine(mainMenuEnvironment));
    }
    public void LoadGameLevel(string level)
    {
        StartCoroutine(TransitionSceneRoutine(level));
    }

    private IEnumerator TransitionSceneRoutine(string newSceneName)
    {
        if (!string.IsNullOrEmpty(currentLoadedEnvironment))
            yield return SceneManager.UnloadSceneAsync(currentLoadedEnvironment);

        yield return SceneManager.LoadSceneAsync(newSceneName, LoadSceneMode.Additive);
        currentLoadedEnvironment = newSceneName;
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(newSceneName));
    }
}