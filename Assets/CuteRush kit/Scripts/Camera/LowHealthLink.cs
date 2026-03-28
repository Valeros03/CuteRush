using UnityEngine;
using Leguar.LowHealth;

public class LowHealthLink : MonoBehaviour
{
    private LowHealthController cameraEffect;

    void Awake()
    {
        cameraEffect = GetComponent<LowHealthController>();
    }

    void OnEnable()
    {
        VitalsController.OnHealthChange += UpdateCameraEffect;
    }

    void OnDisable()
    {
        VitalsController.OnHealthChange -= UpdateCameraEffect;
    }

    private void UpdateCameraEffect(int currentHealth, int maxHealth)
    {
        if (cameraEffect != null)
        {
            float healthPercent = (float)currentHealth / (float)maxHealth;
            cameraEffect.SetPlayerHealthSmoothly(healthPercent, 0.5f);
        }
    }
}