using UnityEngine;
using System.Collections;
using UnityEditor;

public class Granade : MonoBehaviour
{
    public float delay = 3f;             
    public float radius = 5f;            
    public float explosionForce = 700f;
    [SerializeField] private AudioSource audioSource;

    public float maxDamage;
    private GameObject explosionEffect;
    

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

        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);

        foreach (Collider nearbyCollider in colliders)
        {

            Enemy enemy = nearbyCollider.GetComponentInParent<Enemy>();

            if (enemy != null && !enemy.isDead)
            {

                Debug.Log("Enemy colpito");
                float distance = Vector3.Distance(transform.position, enemy.transform.position);

                float damageMultiplier = Mathf.Clamp01(1.0f - (distance / radius));

                float calculatedDamage = maxDamage * damageMultiplier;

                Vector3 direction = (enemy.transform.position - transform.position).normalized;

                direction += Vector3.up * 0.2f;
                direction.Normalize(); 

                Vector3 hitPoint = enemy.transform.position;
              
                enemy.TakeDamage(Mathf.CeilToInt(calculatedDamage), direction, hitPoint);
            }

        }
        
        yield return new WaitForSeconds(2f); 
        Destroy(gameObject);
    }
}
