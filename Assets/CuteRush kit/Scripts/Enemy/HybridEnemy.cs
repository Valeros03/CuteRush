using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

public class HybridEnemy : Enemy
{
    [Header("Hybrid Settings")]
    public float meleeRange = 2f;
    public float rangedRange = 10f;
    public float attackCooldown = 1.5f;
    public GameObject bulletPrefab;
    public Transform firePoint;

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
            agent.stoppingDistance = meleeRange;
        }
    }

    void InitializeBulletPool()
    {
        bulletPool = new List<GameObject>();
        for (int i = 0; i < bulletPoolSize; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            bullet.SetActive(false);
            bulletPool.Add(bullet);
        }
    }

    protected override void PerformChaseLogic()
    {
        if (!agent.isOnNavMesh) return;

        agent.SetDestination(player.position);

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= rangedRange && distance > meleeRange)
        {
            agent.isStopped = true;
            FacePlayer();
            PerformAttack();
        }
        
        else if (distance <= meleeRange && agent.velocity.magnitude < 0.1f)
        {
            agent.isStopped = true; // Rimani fermo
            FacePlayer();
            PerformAttack(); // Chiama il "grilletto"
        }

        else
        {
            agent.isStopped = false; // Continua a muoverti
        }

        // Aggiorna l'animazione
        animator.SetFloat("Speed", agent.velocity.magnitude);
    }

    protected override void PerformAttack()
    {
        if (Time.time - lastAttackTime < attackCooldown) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= meleeRange)
        {
            TryMeleeAttack();
        }
        else if (distance <= rangedRange)
        {
            TryRangedAttack();
        }
    }

    void TryMeleeAttack()
    {
        lastAttackTime = Time.time;
        animator.SetTrigger("Attack");
        SetFace(faces.attackFace);

        player.GetComponent<VitalsController>().Decrease(attackDamage);
    }

    void TryRangedAttack()
    {
       
        GameObject bullet = GetPooledBullet();
        if (bullet == null) return; // Non ci sono proiettili

        lastAttackTime = Time.time;
        animator.SetTrigger("Shoot");
        SetFace(faces.attackFace);

        // Posiziona e spara
        bullet.transform.position = firePoint.position;
        bullet.transform.rotation = firePoint.rotation;
        bullet.SetActive(true);

        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            Vector3 dir = (player.position + Vector3.up * 1.2f - firePoint.position).normalized;
            rb.velocity = dir * 15f;
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

    void FacePlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * agent.angularSpeed);
    }

}