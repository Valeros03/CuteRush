using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;
using System.Collections;


[System.Serializable]
public struct LootDrop
{
    public GameObject itemPrefab;
    [Tooltip("Peso o probabilità (es: Moneta=70, Medikit=20)")]
    public float dropWeight;
}

public class EnemySpawner : BaseSpawner
{
    [Header("Spawn Settings")]
    public GameObject enemyPrefab;
    public int numberOfEnemies = 10;
    public float spawnRadius = 5f;

    [Header("Area Settings")]
    [Tooltip("Raggio dell'area controllata da questo spawner.")]
    public float spawnerAreaRadius = 30f;

    [Header("Loot Settings (First Spawn Only)")]
    [Tooltip("Inserisci qui i Prefab dei PickableItem (monete, medikit, colpi). Verranno distribuiti a caso.")]
    public List<LootDrop> possibleDrops;

    private Transform playerTransform;
    public bool IsPlayerInArea { get; private set; }

    private List<Enemy> spawnedEnemies = new List<Enemy>();
    private SphereCollider areaTrigger;
    private Coroutine spawnCoroutine;

    void Awake()
    {
        areaTrigger = GetComponent<SphereCollider>();
        areaTrigger.isTrigger = true;
        areaTrigger.radius = spawnerAreaRadius;

        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (playerTransform == null) Debug.LogError("EnemySpawner: Player non trovato!", this);
    }

    void Start()
    {
        if (playerTransform != null)
        {
            for (int i = 0; i < numberOfEnemies; i++)
            {
                CreateNewEnemy();
            }

            CheckInitialPlayerPosition();
            spawnCoroutine = StartCoroutine(RespawnRoutine());
        }
    }

    IEnumerator RespawnRoutine()
    {
        while (true)
        {
            // Usiamo il valore ereditato dal BaseSpawner
            yield return new WaitForSeconds(spawnCooldown);
            TryRespawnOneEnemy();
        }
    }

    public void StopSpawn()
    {
        StopCoroutine(spawnCoroutine);
    }

    void TryRespawnOneEnemy()
    {
        foreach (Enemy enemy in spawnedEnemies)
        {
            if (!enemy.gameObject.activeInHierarchy)
            {
                Vector3 newPos;
                if (GetRandomNavMeshPosition(out newPos))
                {
                    NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();
                    if (agent != null) agent.Warp(newPos);
                    else enemy.transform.position = newPos;

                    enemy.gameObject.SetActive(true);
                    break;
                }
            }
        }
    }

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

                GameObject drop = GetRandomWeightedDrop();
                if (drop != null)
                {
                    enemyScript.SetDropItem(drop);
                }

                spawnedEnemies.Add(enemyScript);
            }
        }
    }

    private GameObject GetRandomWeightedDrop()
    {
        if (possibleDrops == null || possibleDrops.Count == 0) return null;

        float totalWeight = 0f;
        foreach (LootDrop drop in possibleDrops)
        {
            totalWeight += drop.dropWeight;
        }

        float randomValue = Random.Range(0f, totalWeight);
        float currentSum = 0f;

        foreach (LootDrop drop in possibleDrops)
        {
            currentSum += drop.dropWeight;
            if (randomValue <= currentSum)
            {
                return drop.itemPrefab;
            }
        }

        return null; 
    }

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