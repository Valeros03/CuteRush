using UnityEngine;

public abstract class BaseSpawner : MonoBehaviour
{
    [Header("Base Settings")]
    [Tooltip("Tempo in secondi per il respawn (nemici) o ricarica (totem)")]
    public float spawnCooldown = 10f;

    public virtual void InitSpawner() { }
}