using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioListenerManager : MonoBehaviour
{
    [SerializeField] private AudioListener _uiAudioListener;

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void Init()
    {
        if(_uiAudioListener == null)
        {
            _uiAudioListener = UIManager.Instance.GetComponentInChildren<AudioListener>();
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenuLand")
        {
            if (_uiAudioListener != null)
                _uiAudioListener.enabled = false;
        }

        else if (scene.name == "UI_Scene" || scene.name == "Main")
        { 
            if (_uiAudioListener != null)
                _uiAudioListener.enabled = true;
        }
    }
}