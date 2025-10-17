using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI; 

public class RangedEnemy : Enemy
{
    [Header("Ranged Settings")]
    public int rangedAttackDamage;
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float projectileForce = 10f;
    public float rangeAttackdistance = 10f; //distanza 
    public float attackCooldown = 2f; //allunga l'attesa prima del successivo colpo esclude il tempo dell'animazione

    
    [Header("Pooling")]
    public int bulletPoolSize = 5;
    private List<GameObject> bulletPool;

    private float lastAttackTime;
    private bool isAttacking;

    protected override void Start()
    {
        base.Start(); 

        InitializeBulletPool();

        if (agent != null)
        {
            agent.stoppingDistance = rangeAttackdistance;
        }
    }

    protected override void PerformChaseLogic()
    {
        

        float distanceSqr = (transform.position - player.position).sqrMagnitude;
        float rangeAttackdistanceSqr = rangeAttackdistance * rangeAttackdistance;

        agent.SetDestination(player.position);

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            
            if (Time.time - lastAttackTime > attackCooldown)
            {
                lastAttackTime = Time.time;
                agent.isStopped = true;
                FacePlayer();
                SetFace(faces.attackFace);
                animator.SetTrigger("Attack");
                isAttacking = true;
            }
        }
        else if(isAttacking == false)
        {
            agent.isStopped = false;
            SetFace(faces.WalkFace);
        }

        animator.SetFloat("Speed", agent.velocity.magnitude);
    }
    void InitializeBulletPool()
    {
        bulletPool = new List<GameObject>();
        for (int i = 0; i < bulletPoolSize; i++)
        {
            GameObject bullet = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            bullet.GetComponent<EnemyBullet>().damage = rangedAttackDamage;
            bullet.SetActive(false);
            bulletPool.Add(bullet);
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
       
        FireProjectile();
        isAttacking = false;
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
            Vector3 dir = (player.position + Vector3.up * 0.5f - firePoint.position).normalized;

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