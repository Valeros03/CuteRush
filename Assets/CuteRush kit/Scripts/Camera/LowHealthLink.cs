using UnityEngine;
using Leguar.LowHealth;

public class LowHealthLink : MonoBehaviour
{
    private LowHealthController cameraEffect;
    private VitalsController vitals;

    void Awake()
    {
        cameraEffect = GetComponent<LowHealthController>();
        vitals = GetComponentInParent<VitalsController>();
    }

    void OnEnable()
    {
        vitals.OnHealthChange += UpdateCameraEffect;
    }

    void OnDisable()
    {
        if (vitals != null)
        {
            vitals.OnHealthChange -= UpdateCameraEffect;
        }
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