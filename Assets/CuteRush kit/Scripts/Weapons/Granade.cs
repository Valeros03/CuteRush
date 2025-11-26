using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

public class Granade : MonoBehaviour
{
    public float delay = 3f;
    public float radius = 5f;
    public float explosionForce = 700f;
    [SerializeField] private AudioSource audioSource;

    public float maxDamage;
    private GameObject explosionEffect;
    [SerializeField] private LayerMask damageLayer;


    void Start()
    {
        explosionEffect = transform.Find("Explosion").gameObject;
        explosionEffect.SetActive(false);
        StartCoroutine(ExplodeAfterDelay());
    }

    IEnumerator ExplodeAfterDelay()
    {
        yield return new WaitForSeconds(delay);

        if (explosionEffect != null) explosionEffect.SetActive(true);
        if (audioSource != null) audioSource.Play();

        // 2. MODIFICA QUI: Aggiungi "damageLayers" come terzo parametro
        // Ora Unity ignorerà automaticamente i trigger di visione se sono su un altro layer
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius, damageLayer);

        // Lista per evitare di colpire lo stesso nemico più volte (es. colpisco gamba E testa)
        HashSet<Enemy> enemiesHit = new HashSet<Enemy>();

        foreach (Collider nearbyCollider in colliders)
        {
            // Nota: non serve più "if (nearbyCollider.isTrigger) continue;" 
            // se hai impostato bene i layer, ma lasciarlo non fa danni.

            Enemy enemy = nearbyCollider.GetComponentInParent<Enemy>();

            // 3. Aggiungi il controllo "!enemiesHit.Contains(enemy)"
            if (enemy != null && !enemy.isDead && !enemiesHit.Contains(enemy))
            {
                // Aggiungiamo il nemico alla lista "già colpiti"
                enemiesHit.Add(enemy);

                // Calcolo preciso con ClosestPoint
                Vector3 closestPoint = nearbyCollider.ClosestPoint(transform.position);

                float distance = Vector3.Distance(transform.position, closestPoint);

                float damageMultiplier = Mathf.Clamp01(1.0f - (distance / radius));

                float calculatedDamage = maxDamage * damageMultiplier;

                Vector3 direction = (enemy.transform.position - transform.position).normalized;
                direction += Vector3.up * 0.2f;
                direction.Normalize();

                enemy.TakeDamage(Mathf.CeilToInt(calculatedDamage), direction, closestPoint);
            }
        }

        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }
}
