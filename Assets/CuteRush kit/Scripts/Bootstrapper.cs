using UnityEngine;
using UnityEngine.SceneManagement;

public class Bootstrapper : MonoBehaviour
{
    void Awake()
    {
        if (SceneManager.GetSceneByName("UI_Scene").isLoaded == false)
        {
            SceneManager.LoadScene("UI_Scene", LoadSceneMode.Additive);
        }
    }

    void Start()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayAmbient("Spaceship Engine Light");
        }
    }
}