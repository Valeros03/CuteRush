using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DamageIndicator : MonoBehaviour
{
    public float fadeTime = 2f; // Tempo di dissolvenza

    private Vector3 currentEnemyPosition;
    private Image indicatorImage;
    private Transform camTransform;
    private Coroutine fadeRoutine;

    public void Initialize(Vector3 enemyPos)
    {
        currentEnemyPosition = enemyPos;
        indicatorImage = GetComponentInChildren<Image>();
        camTransform = Camera.main.transform;

        // Avviamo la prima dissolvenza
        fadeRoutine = StartCoroutine(FadeOut());
    }

    // NUOVO METODO: Viene chiamato dall'HUD per resettare l'indicatore (unire colpi vicini)
    public void Refresh(Vector3 newEnemyPos)
    {
        currentEnemyPosition = newEnemyPos; // Aggiorniamo posizione nemico se si è mosso

        if (fadeRoutine != null) StopCoroutine(fadeRoutine);

        // Resettiamo visivamente l'alpha dell'immagine (tutto splendente rosso)
        Color c = indicatorImage.color;
        c.a = 1f;
        indicatorImage.color = c;

        // Ripartiamo con la dissolvenza
        fadeRoutine = StartCoroutine(FadeOut());
    }

    // Helper function per far capire all'HUD la nostra direzione
    public float GetCurrentZRotation()
    {
        return transform.localEulerAngles.z;
    }

    void Update()
    {
        if (camTransform == null || indicatorImage == null) return;

     
        Vector3 relDir = currentEnemyPosition - camTransform.position;
        float xView = Vector3.Dot(relDir, camTransform.right);
        float yView = Vector3.Dot(relDir, camTransform.forward);

        
        float angleDeg = Mathf.Atan2(xView, yView) * Mathf.Rad2Deg;
        float finalZ = -angleDeg;

        finalZ = -angleDeg - 90f; 

        transform.localEulerAngles = new Vector3(0f, 0f, finalZ);
    }

    private IEnumerator FadeOut()
    {
        float timer = 0f;
        Color startColor = indicatorImage.color;

        while (timer < fadeTime)
        {
            timer += Time.deltaTime;
            startColor.a = Mathf.Lerp(1f, 0f, timer / fadeTime);
            indicatorImage.color = startColor;
            yield return null;
        }

        Destroy(gameObject);
    }
}