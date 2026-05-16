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
    private Camera loadingCamera;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        GameObject camObj = new GameObject("LoadingCamera");
        camObj.transform.SetParent(transform);
        loadingCamera = camObj.AddComponent<Camera>();
        loadingCamera.clearFlags = CameraClearFlags.SolidColor;
        loadingCamera.backgroundColor = Color.black;
        loadingCamera.depth = -100;
        loadingCamera.gameObject.SetActive(true);

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

    public void ContinueFromLoadingScreen()
    {
        continueLoad = true;
    }

    private IEnumerator TransitionSceneRoutine(string newSceneName)
    {
        if (UIManager.Instance != null)
            UIManager.Instance.ShowLoadingScreen();

        continueLoad = false;

        AudioManager.Instance.StopMusic();
        if (!string.IsNullOrEmpty(currentLoadedEnvironment))
            yield return SceneManager.UnloadSceneAsync(currentLoadedEnvironment);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(newSceneName, LoadSceneMode.Additive);
        asyncLoad.allowSceneActivation = false;

        while (asyncLoad.progress < 0.9f)
        {
            yield return null;
        }
        UIManager.Instance.ShowContinueButton();
        while (!continueLoad)
        {
            yield return null;
        }

        asyncLoad.allowSceneActivation = true;

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