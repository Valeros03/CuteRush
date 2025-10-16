using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

    protected override void Update()
    {
        base.Update();
        if (isDead || player == null) return;

        PerformAttack();
        ChasePlayer();
    }

    protected override void PerformAttack()
    {
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
        if (Time.time - lastAttackTime < attackCooldown) return;

        lastAttackTime = Time.time;
        animator.SetTrigger("Attack");
        SetFace(faces.attackFace);

        if (Vector3.Distance(transform.position, player.position) <= meleeRange)
        {
            player.GetComponent<VitalsController>().Decrease(attackDamage);
        }
    }

    void TryRangedAttack()
    {
        if (Time.time - lastAttackTime < attackCooldown) return;

        GameObject bullet = GetPooledBullet();
        if (bullet == null) return;

        lastAttackTime = Time.time;
        animator.SetTrigger("Shoot");
        SetFace(faces.attackFace);

        
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
        // Se arrivi qui, tutti i proiettili sono già attivi (in volo)
        return null;
    }
    // ---------------------------

    void ChasePlayer()
    {
        agent.SetDestination(player.position);
        animator.SetFloat("Speed", agent.velocity.magnitude);
    }
}