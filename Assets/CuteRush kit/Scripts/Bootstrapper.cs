using UnityEngine;
using UnityEngine.SceneManagement;

public class Bootstrapper : MonoBehaviour
{
    public static Bootstrapper Instance { get; private set; }
    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (SceneManager.GetSceneByName(GameConstants.SCENE_UI).isLoaded == false)
        {
            SceneManager.LoadScene(GameConstants.SCENE_UI, LoadSceneMode.Additive);
        }
    }

    void Start()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayMusic(GameConstants.AUDIO_IN_GAME_SONG);
        }
    }

    public void LoadGameLevel(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}