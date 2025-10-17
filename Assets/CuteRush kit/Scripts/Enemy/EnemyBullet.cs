using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float maxLifeTime = 3.0f;
    public int damage;

    void OnEnable()
    {
        Invoke(nameof(Deactivate), maxLifeTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        
        if (collision.collider.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<VitalsController>().Decrease(damage);
        }
        Deactivate();
    }

    void Deactivate()
    {
        CancelInvoke(nameof(Deactivate));

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        gameObject.SetActive(false);
    }
}