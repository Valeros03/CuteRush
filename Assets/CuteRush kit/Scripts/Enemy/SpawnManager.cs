using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI; // Necessario per gestire NavMeshAgent
using System.Collections;

public class SpawnManager : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject enemyPrefab;
    public int numberOfEnemies = 10;
    public float spawnRadius = 5f;
    public float timeSpawn = 1f; // Tempo di attesa per il respawn

    [Header("Area Settings")]
    [Tooltip("Raggio dell'area controllata da questo spawner.")]
    public float spawnerAreaRadius = 30f;

    private Transform playerTransform;
    public bool IsPlayerInArea { get; private set; }

    private List<Enemy> spawnedEnemies = new List<Enemy>();
    private SphereCollider areaTrigger;
    private Coroutine spawnCorutine;

    void Awake()
    {
        areaTrigger = GetComponent<SphereCollider>();
        areaTrigger.isTrigger = true;
        areaTrigger.radius = spawnerAreaRadius;

        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (playerTransform == null) Debug.LogError("SpawnManager: Player non trovato!", this);
    }

    void Start()
    {
        if (playerTransform != null)
        {
            // Spawn iniziale
            for (int i = 0; i < numberOfEnemies; i++)
            {
                CreateNewEnemy();
            }

            CheckInitialPlayerPosition();
            spawnCorutine = StartCoroutine(RespawnRoutine());

        }
    }

    IEnumerator RespawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeSpawn);
            TryRespawnOneEnemy();
         
        }
    }

    public void StopSpawn()
    {
        StopCoroutine(spawnCorutine);
    }

    void TryRespawnOneEnemy()
    {
        // Cerca nella lista un nemico che è stato disattivato (morto)
        foreach (Enemy enemy in spawnedEnemies)
        {
            if (!enemy.gameObject.activeInHierarchy) // Trovato un nemico morto
            {
                // 1. Calcola nuova posizione
                Vector3 newPos;
                if (GetRandomNavMeshPosition(out newPos))
                {
                    // 2. Sposta il nemico
                    NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();
                    if (agent != null)
                    {
                        // IMPORTANTE: Se c'è un NavMeshAgent, devi usare Warp!
                        agent.Warp(newPos);
                    }
                    else
                    {
                        enemy.transform.position = newPos;
                    }

                    // 3. Riattiva l'oggetto
                    enemy.gameObject.SetActive(true);

                  
                    break;
                }
            }
        }
    }

    // Ho estratto la logica della posizione per non ripeterla
    bool GetRandomNavMeshPosition(out Vector3 result)
    {
        Vector3 randomPos = Random.insideUnitSphere * spawnRadius;
        randomPos += transform.position;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPos, out hit, spawnRadius, 1))
        {
            result = hit.position;
            return true;
        }

        result = Vector3.zero;
        return false;
    }

    void CreateNewEnemy()
    {
        Vector3 spawnPos;
        if (GetRandomNavMeshPosition(out spawnPos))
        {
            GameObject enemyGO = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
            Enemy enemyScript = enemyGO.GetComponent<Enemy>();

            if (enemyScript != null)
            {
                enemyScript.Initialize(transform.position, this);
                spawnedEnemies.Add(enemyScript);
            }
        }
    }

    // --- Gestione Trigger (Invariata) ---
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) IsPlayerInArea = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) IsPlayerInArea = false;
    }

    void CheckInitialPlayerPosition()
    {
        if (playerTransform == null) return;
        float distance = Vector3.Distance(playerTransform.position, transform.position);
        IsPlayerInArea = (distance < spawnerAreaRadius);
    }
}