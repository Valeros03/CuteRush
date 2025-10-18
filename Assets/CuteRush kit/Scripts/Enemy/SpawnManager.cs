using UnityEngine;
using System.Collections.Generic;

public class SpawnManager : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject enemyPrefab;
    public int numberOfEnemies = 10;
    public float spawnRadius = 5f;

    [Header("Area Settings")]
    [Tooltip("Raggio dell'area controllata da questo spawner.")]
    public float spawnerAreaRadius = 30f;

    
    private Transform playerTransform;

    public bool IsPlayerInArea { get; private set; }
    

    private List<Enemy> spawnedEnemies = new List<Enemy>();

    private SphereCollider areaTrigger; 

    void Awake() 
    {
        // Ottieni il collider e impostalo come trigger
        areaTrigger = GetComponent<SphereCollider>();
        areaTrigger.isTrigger = true;
        areaTrigger.radius = spawnerAreaRadius;

        // Trova il player
        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (playerTransform == null)
        {
            Debug.LogError("SpawnManager: Player non trovato!", this);
        }
    }

    void Start()
    {
        if (playerTransform != null) 
        {
            SpawnEnemies();
            CheckInitialPlayerPosition();
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            IsPlayerInArea = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            IsPlayerInArea = false;
        }
    }

    void CheckInitialPlayerPosition()
    {
        if (playerTransform == null) return;
        float distance = Vector3.Distance(playerTransform.position, transform.position);
        IsPlayerInArea = (distance < spawnerAreaRadius);
    }

    void SpawnEnemies()
    {
        for (int i = 0; i < numberOfEnemies; i++)
        {
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
                   
                    enemyScript.Initialize(transform.position, this);
                    spawnedEnemies.Add(enemyScript);
                }
            }
           
        }
    }

    
}