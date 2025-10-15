using UnityEngine;

public class TriggerCollider : MonoBehaviour
{
    public Enemy enemy; // riferimento al genitore

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            enemy.HandleTriggerEnter(this, other); // passa anche il collider del player
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            enemy.HandleTriggerExit(this, other); // opzionale
        }
    }
}
