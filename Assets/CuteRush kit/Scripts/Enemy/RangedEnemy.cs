using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI; // Assicurati di avere questo!

public class RangedEnemy : Enemy
{
    [Header("Ranged Settings")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float projectileForce = 10f;

    [Header("AI Behavior")]
    public float minRetreatDistance = 5f; // Distanza minima: se il player è più vicino, scappa
    public float idealAttackRange = 10f;  // Distanza ideale: l'agente si fermerà qui
    public float attackCooldown = 2f;

    
    [Header("Pooling")]
    public int bulletPoolSize = 5;
    private List<GameObject> bulletPool;

    private float lastAttackTime;

    protected override void Start()
    {
        base.Start();

        InitializeBulletPool();

      
        if (agent != null)
        {
            agent.stoppingDistance = idealAttackRange;
        }
    }

    void InitializeBulletPool()
    {
        bulletPool = new List<GameObject>();
        for (int i = 0; i < bulletPoolSize; i++)
        {
            GameObject bullet = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            bullet.SetActive(false);
            bulletPool.Add(bullet);
        }
    }

    protected override void Update()
    {
        base.Update();
        if (isDead || player == null)
        {
            if (agent != null && agent.isOnNavMesh) agent.isStopped = true;
            return;
        }

        HandleAI();
    }

    void HandleAI()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance < minRetreatDistance)
        {
            // 1. Player troppo vicino: RITIRATI
            Retreat();
        }
        else
        {
            // 2. Player a distanza di sicurezza: INSEGUI e ATTACCA
            ChaseAndAttack(distance);
        }

        animator.SetFloat("Speed", agent.velocity.magnitude);
    }

    void Retreat()
    {
        if (!agent.isOnNavMesh) return;

        Vector3 dirAwayFromPlayer = (transform.position - player.position).normalized;

        Vector3 retreatPosition = transform.position + dirAwayFromPlayer * 10f;

        agent.SetDestination(retreatPosition);
        agent.isStopped = false;
    }

    void ChaseAndAttack(float distance)
    {
        if (!agent.isOnNavMesh) return;

        agent.SetDestination(player.position);
        agent.isStopped = false;

       
        if (distance <= idealAttackRange && agent.velocity.magnitude < 0.1f)
        {
            // Siamo fermi e pronti a sparare
            agent.isStopped = true; // Fermati del tutto per sparare
            FacePlayer();
            PerformAttack();
        }
    }

    void FacePlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * agent.angularSpeed);
    }

    protected override void PerformAttack()
    {
        if (Time.time - lastAttackTime < attackCooldown) return;

        lastAttackTime = Time.time;

        animator.SetTrigger("Attack"); 
        SetFace(faces.attackFace); 

        FireProjectile();
    }

    void FireProjectile()
    {
        GameObject bullet = GetPooledBullet();
        if (bullet == null) return; 

        bullet.transform.position = firePoint.position;
        bullet.transform.rotation = firePoint.rotation;
        bullet.SetActive(true);

        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero; 
            Vector3 dir = (player.position + Vector3.up * 1.2f - firePoint.position).normalized;

            rb.AddForce(dir * projectileForce, ForceMode.VelocityChange);
        }
    }
    GameObject GetPooledBullet()
    {
        foreach (GameObject bullet in bulletPool)
        {
            if (!bullet.activeInHierarchy)
            {
                return bullet;
            }
        }
        return null;
    }
}