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

[System.Serializable]
public struct EnemySpawnWeight
{
    public GameObject enemyPrefab;
    [Tooltip("Peso o probabilità (es: Slime=80, Hybrid=15, Sniper=5)")]
    public float spawnWeight;
}

public class EnemySpawner : BaseSpawner
{
    [Header("Spawn Settings")]
    [Tooltip("Lista dei nemici che possono nascere e le loro probabilità")]
    public List<EnemySpawnWeight> enemyTypes;

    [Tooltip("Quanti nemici MASSIMI possono essere attivi contemporaneamente da questo spawner")]
    public int maxConcurrentEnemies = 10;

    [Tooltip("Quanti nemici far nascere subito all'avvio della partita? (Gli altri arriveranno col tempo)")]
    public int initialSpawnCount = 3;

    public float spawnRadius = 5f;

    [Header("Area Settings")]
    public float spawnerAreaRadius = 30f;

    [Header("Shutdown Settings")]
    public float shutdownDuration = 15f;

    public bool isShutDown { get; private set; }

    [Header("Loot Settings")]
    public List<LootDrop> possibleDrops;

    private Transform playerTransform;
    public bool IsPlayerInArea { get; private set; }

    private List<Enemy> spawnedEnemies = new List<Enemy>();
    private SphereCollider areaTrigger;

    void Awake()
    {
        areaTrigger = GetComponent<SphereCollider>();
        areaTrigger.isTrigger = true;
        areaTrigger.radius = spawnerAreaRadius;

        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Start()
    {
        if (playerTransform != null)
        {
            int startCount = Mathf.Min(initialSpawnCount, maxConcurrentEnemies);
            for (int i = 0; i < startCount; i++)
            {
                TrySpawnEnemy();
            }

            CheckInitialPlayerPosition();
            StartCoroutine(RespawnRoutine());
        }
    }

    IEnumerator RespawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnCooldown);

            if (!isShutDown)
            {
                TrySpawnEnemy();
            }
        }
    }

    public void StopSpawn()
    {
        if (isShutDown) return;

        isShutDown = true;
        StartCoroutine(LogicCooldownRoutine());
    }

    private IEnumerator LogicCooldownRoutine()
    {
        yield return new WaitForSeconds(shutdownDuration);
        isShutDown = false;
    }

    void TrySpawnEnemy()
    {
        int activeCount = 0;
        foreach (Enemy e in spawnedEnemies)
        {
            if (e.gameObject.activeInHierarchy) activeCount++;
        }

        if (activeCount >= maxConcurrentEnemies) return;

        GameObject selectedPrefab = GetRandomWeightedEnemy();
        if (selectedPrefab == null) return;

        Enemy pooledEnemy = GetInactiveEnemyOfPrefab(selectedPrefab);

        if (pooledEnemy != null)
        {
            Vector3 newPos;
            if (GetRandomNavMeshPosition(out newPos))
            {
                NavMeshAgent agent = pooledEnemy.GetComponent<NavMeshAgent>();
                if (agent != null) agent.Warp(newPos);
                else pooledEnemy.transform.position = newPos;

                GameObject drop = GetRandomWeightedDrop();
                if (drop != null) pooledEnemy.SetDropItem(drop);

                pooledEnemy.gameObject.SetActive(true);
            }
        }
        else
        {
            CreateNewEnemy(selectedPrefab);
        }
    }

    Enemy GetInactiveEnemyOfPrefab(GameObject prefab)
    {
        foreach (Enemy e in spawnedEnemies)
        {
            if (!e.gameObject.activeInHierarchy && e.gameObject.name == prefab.name)
            {
                return e;
            }
        }
        return null;
    }

    void CreateNewEnemy(GameObject prefab)
    {
        Vector3 spawnPos;
        if (GetRandomNavMeshPosition(out spawnPos))
        {
            GameObject enemyGO = Instantiate(prefab, spawnPos, Quaternion.identity);

            enemyGO.name = prefab.name;

            Enemy enemyScript = enemyGO.GetComponent<Enemy>();

            if (enemyScript != null)
            {
                enemyScript.Initialize(transform.position, this);

                GameObject drop = GetRandomWeightedDrop();
                if (drop != null) enemyScript.SetDropItem(drop);

                spawnedEnemies.Add(enemyScript);
            }
        }
    }

    private GameObject GetRandomWeightedEnemy()
    {
        if (enemyTypes == null || enemyTypes.Count == 0) return null;
        EnemySpawnWeight selectedEnemy = enemyTypes.GetRandomWeightedItem(enemy => enemy.spawnWeight);
        return selectedEnemy.enemyPrefab;
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

    private GameObject GetRandomWeightedDrop()
    {
        if (possibleDrops == null || possibleDrops.Count == 0) return null;

        float baseDropChance = 1.0f;
        float currentDropChance = baseDropChance;

        if (GameManager.Instance != null && GameManager.Instance.currentDifficulty != null)
        {
            currentDropChance = baseDropChance * GameManager.Instance.currentDifficulty.dropRateMultiplier;
        }

        if (Random.value > currentDropChance)
        {
            return null;
        }
        LootDrop selectedDrop = possibleDrops.GetRandomWeightedItem(drop => drop.dropWeight);

        return selectedDrop.itemPrefab;
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