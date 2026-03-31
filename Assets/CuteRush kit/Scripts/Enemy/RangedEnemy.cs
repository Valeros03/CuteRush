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
    public float rangeAttackdistance = 10f;
    public float attackCooldown = 2f;

    
    [Header("Pooling")]
    public int bulletPoolSize = 5;
    private List<EnemyBullet> bulletPool;

    private float lastAttackTime;
    private bool isAttacking;

    private int scaledRangedDamage;

    protected override void Start()
    {
        base.Start(); 

        InitializeBulletPool();

        if (agent != null)
        {
            agent.stoppingDistance = rangeAttackdistance;
        }
    }

    public override void OnEnable()
    {
        base.OnEnable();

        float diffMult = 1.0f;
        if (DifficultyManager.Instance != null)
        {
            diffMult = DifficultyManager.Instance.currentMultiplier;
        }
        scaledRangedDamage = Mathf.RoundToInt(rangedAttackDamage * diffMult);
    }

    protected override void PerformChaseLogic()
    {
        

        float distanceSqr = (transform.position - player.position).sqrMagnitude;
        float rangeAttackdistanceSqr = rangeAttackdistance * rangeAttackdistance;

        agent.SetDestination(player.position);

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {

            agent.isStopped = true;

            Vector3 predictedTarget = GetPredictedPlayerPosition(projectileForce, firePoint.position);
            FaceTarget(predictedTarget);
            if (Time.time - lastAttackTime > attackCooldown)
            {
                lastAttackTime = Time.time;
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
        bulletPool = new List<EnemyBullet>();
        if (projectilePrefab == null) return;

        for (int i = 0; i < bulletPoolSize; i++)
        {
            GameObject bulletObj = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            bulletObj.SetActive(false);

            EnemyBullet bulletComp = bulletObj.GetComponent<EnemyBullet>();
            if (bulletComp != null)
            {
                bulletPool.Add(bulletComp);
            }
        }
    }

    void FaceTarget(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        direction.y = 0;

        if (direction == Vector3.zero) return;

        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * agent.angularSpeed);
    }

    protected override void PerformAttack()
    {
       
        FireProjectile();
        isAttacking = false;
    }

    void FireProjectile()
    {
        EnemyBullet bullet = GetPooledBullet();
        if (bullet == null) return;

        bullet.transform.position = firePoint.position;
        bullet.transform.rotation = firePoint.rotation;

        bullet.gameObject.SetActive(true);

        Vector3 predictedTarget = GetPredictedPlayerPosition(projectileForce, firePoint.position);
        Vector3 dir = (predictedTarget - firePoint.position).normalized;

        bullet.Fire(dir, scaledRangedDamage, projectileForce, transform.position);
    }

    EnemyBullet GetPooledBullet()
    {
        foreach (EnemyBullet bullet in bulletPool)
        {
            if (!bullet.gameObject.activeInHierarchy)
            {
                return bullet;
            }
        }
        return null;
    }
}