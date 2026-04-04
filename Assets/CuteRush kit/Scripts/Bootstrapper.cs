using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Bootstrapper : MonoBehaviour
{
    public static Bootstrapper Instance { get; private set; }

    public string uiSceneName = GameConstants.SCENE_UI;
    public string mainMenuEnvironment = GameConstants.SCENE_MAIN_MENU;
  
    private string currentLoadedEnvironment = "";

    [SerializeField] private AudioListenerManager listenerManager;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.LoadScene(uiSceneName, LoadSceneMode.Additive);
    }

    void Start()
    {
        listenerManager.Init();
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
        AudioManager.Instance.StopMusic();
        if (!string.IsNullOrEmpty(currentLoadedEnvironment))
            yield return SceneManager.UnloadSceneAsync(currentLoadedEnvironment);

        yield return SceneManager.LoadSceneAsync(newSceneName, LoadSceneMode.Additive);
        currentLoadedEnvironment = newSceneName;
        Time.timeScale = 1f;
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(newSceneName));
    }
}