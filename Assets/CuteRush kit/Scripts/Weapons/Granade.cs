using UnityEngine;
using System.Collections;
using UnityEditor;

public class Granade : MonoBehaviour
{
    public float delay = 3f;             // secondi prima di esplodere
    public float radius = 5f;            // raggio esplosione
    public float explosionForce = 700f;  // forza dell’esplosione

    private GameObject explosionEffect;  // riferimento al figlio

    void Start()
    {
        // Recupera il figlio (l'esplosione)
        explosionEffect = transform.Find("Explosion").gameObject;

        // Assicuriamoci che sia spento all'inizio
        explosionEffect.SetActive(false);

        // Avvia la coroutine che gestisce il timer
        StartCoroutine(ExplodeAfterDelay());
    }

    IEnumerator ExplodeAfterDelay()
    {
        // Attendi il tempo di delay
        yield return new WaitForSeconds(delay);

        // Attiva l'effetto dell'esplosione
        explosionEffect.SetActive(true);
        GetComponent<AudioSource>().Play();

        // Trova oggetti colpiti nell’area
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);

        foreach (Collider nearby in colliders)
        {

            // Se è un nemico/apply damage
            EnemyAi enemy = nearby.GetComponentInParent<EnemyAi>();
            
            if (enemy != null)
            {
                enemy.TakeDamage(50, Vector3.zero, Vector3.zero); //andrebbe take damage, bisogna calcolare il danno in base alla distanza, la direzione e verso del danno
            }
        }

        // Aspetta un attimo per lasciare visibile l'effetto
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);

    }
}
