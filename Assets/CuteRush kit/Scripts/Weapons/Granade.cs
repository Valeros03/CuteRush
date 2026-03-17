using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class Granade : MonoBehaviour
{
    public float delay = 3f;
    public float radius = 5f;
    public float explosionForce = 700f; // Utile se vuoi far saltare in aria oggetti fisici, ma non sui nemici per ora
    [SerializeField] private AudioSource audioSource;

    public float maxDamage;
    private GameObject explosionEffect;
    [SerializeField] private LayerMask damageLayer;

    private Rigidbody rb;
    private bool hasExploded = false;

    void Awake()
    {
        // Recuperiamo il Rigidbody
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        explosionEffect = transform.Find("Explosion").gameObject;
        if (explosionEffect != null) explosionEffect.SetActive(false);

        StartCoroutine(ExplodeAfterDelay());
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Se è già esplosa, ignoriamo
        if (rb.isKinematic || hasExploded) return;

        
        if (collision.gameObject.layer == LayerMask.NameToLayer("EnemyBullet"))
        {
            return;
        }

   
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;
    }

    IEnumerator ExplodeAfterDelay()
    {
        yield return new WaitForSeconds(delay);
        hasExploded = true;

        if (explosionEffect != null) explosionEffect.SetActive(true);
        if (audioSource != null) audioSource.Play();

      
        GetComponentInChildren<MeshRenderer>().enabled = false;

        Collider[] colliders = Physics.OverlapSphere(transform.position, radius, damageLayer);

        HashSet<Enemy> enemiesHit = new HashSet<Enemy>();

        foreach (Collider nearbyCollider in colliders)
        {
            Enemy enemy = nearbyCollider.GetComponentInParent<Enemy>();

            if (enemy == null)
            {
            
                continue;
            }

            if (enemy != null && !enemy.isDead && !enemiesHit.Contains(enemy))
            {
                enemiesHit.Add(enemy);

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

        // Aspettiamo che il particle system finisca
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }

    // Un piccolo bonus: disegna la sfera di esplosione nell'editor per aiutarti a visualizzare il raggio!
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}