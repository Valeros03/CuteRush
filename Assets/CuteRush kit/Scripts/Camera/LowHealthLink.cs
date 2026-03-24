using UnityEngine;
using Leguar.LowHealth;

public class LowHealthLink : MonoBehaviour
{
    private LowHealthController cameraEffect;

    void Awake()
    {
        // Prende in automatico lo script dell'effetto attaccato allo stesso oggetto (la telecamera)
        cameraEffect = GetComponent<LowHealthController>();
    }

    // Quando la telecamera viene attivata, si "abbona" alle notizie sulla salute
    void OnEnable()
    {
        VitalsController.OnHealthChange += UpdateCameraEffect;
    }

    // Quando la telecamera viene disattivata, disdice l'abbonamento per evitare errori
    void OnDisable()
    {
        VitalsController.OnHealthChange -= UpdateCameraEffect;
    }

    // Questo metodo scatta in automatico ogni volta che subisci danno o ti curi!
    private void UpdateCameraEffect(int currentHealth, int maxHealth)
    {
        if (cameraEffect != null)
        {
            // Calcola la percentuale e attiva l'effetto sfumato
            float healthPercent = (float)currentHealth / (float)maxHealth;
            cameraEffect.SetPlayerHealthSmoothly(healthPercent, 0.5f);
        }
    }
}