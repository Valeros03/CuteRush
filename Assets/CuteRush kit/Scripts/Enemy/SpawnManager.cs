using UnityEngine;
using System.Collections.Generic;

public class SpawnManager : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject enemyPrefab; // Il prefab del nemico da spawnare
    public int numberOfEnemies = 10;
    public float spawnRadius = 5f; // Raggio entro cui spawnare i nemici

    [Header("AI Leash Settings")]
    public float leashRange = 30f; // Distanza massima dal quale i nemici si arrendono

    // Opzionale: per tenere traccia dei nemici (utile per il contatore)
    private List<Enemy> spawnedEnemies = new List<Enemy>();

    void Start()
    {
        SpawnEnemies();
    }

    void SpawnEnemies()
    {
        for (int i = 0; i < numberOfEnemies; i++)
        {
            // Calcola una posizione di spawn casuale attorno allo spawner
            Vector3 randomPos = Random.insideUnitSphere * spawnRadius;
            randomPos += transform.position;

            UnityEngine.AI.NavMeshHit hit;
            if (UnityEngine.AI.NavMesh.SamplePosition(randomPos, out hit, spawnRadius, 1))
            {
                randomPos = hit.position;


                GameObject enemyGO = Instantiate(enemyPrefab, randomPos, Quaternion.identity);
                Enemy enemyScript = enemyGO.GetComponent<Enemy>();

                if (enemyScript != null)
                {
                    enemyScript.Initialize(transform.position, leashRange);
                    spawnedEnemies.Add(enemyScript);
                }

            }
            else
            {
                // Logga un errore per farti sapere che lo spawn è fallito
                Debug.LogError($"Spawn FAILED for enemy {i}! No NavMesh found near {randomPos}.");
            }

            
        }
    }
}