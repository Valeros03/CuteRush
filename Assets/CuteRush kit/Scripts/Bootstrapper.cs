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

        if (SceneManager.GetSceneByName("UI_Scene").isLoaded == false)
        {
            SceneManager.LoadScene("UI_Scene", LoadSceneMode.Additive);
        }
    }

    void Start()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayMusic("InGameSong");
        }
    }

    public void LoadGameLevel(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}