using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Bootstrapper : MonoBehaviour
{
    public static Bootstrapper Instance { get; private set; }

    public string uiSceneName = GameConstants.SCENE_UI;
    public string mainMenuEnvironment = GameConstants.SCENE_MAIN_MENU;
  
    private string currentLoadedEnvironment = "";
    private bool continueLoad = false;

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

    // [Requires Inspector Setup] Connect the continue/start button's OnClick event in the loading screen
    // to call this ContinueFromLoadingScreen() method on the Bootstrapper.
    public void ContinueFromLoadingScreen()
    {
        continueLoad = true;
    }

    private IEnumerator TransitionSceneRoutine(string newSceneName)
    {
        // Tell UIManager to show the loading screen
        if (UIManager.Instance != null)
            UIManager.Instance.ShowLoadingScreen();

        continueLoad = false;

        AudioManager.Instance.StopMusic();
        if (!string.IsNullOrEmpty(currentLoadedEnvironment))
            yield return SceneManager.UnloadSceneAsync(currentLoadedEnvironment);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(newSceneName, LoadSceneMode.Additive);
        asyncLoad.allowSceneActivation = false;

        // Wait until scene is loaded in memory (stops at 0.9 progress when allowSceneActivation is false)
        while (asyncLoad.progress < 0.9f)
        {
            yield return null;
        }

        // Wait for player to press the continue button on the loading screen
        while (!continueLoad)
        {
            yield return null;
        }

        asyncLoad.allowSceneActivation = true;

        // Wait for scene to finish activating
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        if (UIManager.Instance != null)
            UIManager.Instance.HideLoadingScreen();

        currentLoadedEnvironment = newSceneName;
        Time.timeScale = 1f;
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(newSceneName));
    }
}