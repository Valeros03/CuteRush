using UnityEngine;
using UnityEngine.SceneManagement;

public class Bootstrapper : MonoBehaviour
{
    void Awake()
    {
        // ✅ Controlla se la scena della UI ("UI_Scene") è già caricata
        if (SceneManager.GetSceneByName("UI_Scene").isLoaded == false)
        {
            // 🔄 Se non è caricata, la carica in modo "Additive"
            SceneManager.LoadScene("UI_Scene", LoadSceneMode.Additive);
        }

    }
}
