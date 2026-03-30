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
        bulletPool = new List<GameObject>();
        for (int i = 0; i < bulletPoolSize; i++)
        {
            GameObject bullet = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            
            bullet.SetActive(false);
            bulletPool.Add(bullet);
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
        GameObject bullet = GetPooledBullet();
        if (bullet == null) return;

        bullet.transform.position = firePoint.position;
        bullet.transform.rotation = firePoint.rotation;
        bullet.SetActive(true);

        Vector3 predictedTarget = GetPredictedPlayerPosition(projectileForce, firePoint.position);
        Vector3 dir = (predictedTarget - firePoint.position).normalized;

        bullet.GetComponent<EnemyBullet>().Fire(dir, rangedAttackDamage, projectileForce, transform.position);
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