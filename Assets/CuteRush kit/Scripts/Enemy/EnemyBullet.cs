using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyBullet : MonoBehaviour
{
    public float maxLifeTime = 3.0f;
    public int damage;
    public Vector3 shooterPosition; 

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Questa è la nuova funzione chiave: il nemico la chiama per sparare!
    public void Fire(Vector3 direction, int bulletDamage, float bulletSpeed, Vector3 enemyPos)
    {
        damage = bulletDamage;
        shooterPosition = enemyPos; // Memorizziamo la posizione del nemico! (Fase 1 Problema 2)

        // Reset fisica
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.AddForce(direction.normalized * bulletSpeed, ForceMode.VelocityChange);
        Invoke(nameof(Deactivate), maxLifeTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            // Passiamo la posizione del nemico, non quella attuale del proiettile!
            collision.gameObject.GetComponent<VitalsController>()?.Decrease(damage, shooterPosition);
        }

        Deactivate();
    }

    void Deactivate()
    {
        CancelInvoke(nameof(Deactivate));
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        gameObject.SetActive(false);
    }
}