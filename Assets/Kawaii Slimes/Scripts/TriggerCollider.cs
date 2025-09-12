using UnityEngine;

public class TriggerCollider : MonoBehaviour
{
    public EnemyAi enemyAi; // riferimento al genitore

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            enemyAi.HandleTriggerEnter(this, other); // passa anche il collider del player
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            enemyAi.HandleTriggerExit(this, other); // opzionale
        }
    }
}
