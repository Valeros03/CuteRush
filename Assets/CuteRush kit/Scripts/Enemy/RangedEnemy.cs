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
    public AnimationCurve projectileForceCurve;
    public float rangeAttackdistance = 10f;
    public float attackCooldown = 2f;

    
    [Header("Pooling")]
    public int bulletPoolSize = 5;
    private List<EnemyBullet> bulletPool;

    private float lastAttackTime;
    private bool isAttacking;

    private int scaledRangedDamage;
    private float projectileForce = 10f;

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
        projectileForce = projectileForceCurve.Evaluate(diffMult);
    }

    protected override void PerformChaseLogic()
    {
        if (!agent.isOnNavMesh) return;

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
                Vector3 planarForward = transform.forward; planarForward.y = 0;
                Vector3 planarDir = predictedTarget - transform.position; planarDir.y = 0;

                if (Vector3.Angle(planarForward, planarDir) <= aimTolerance)
                {
                    lastAttackTime = Time.time + Random.Range(-0.1f, 0.2f);
                    SetFace(faces.attackFace);
                    animator.SetTrigger("Attack");
                    isAttacking = true;
                }
            }
        }
        else if (isAttacking == false)
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

        bullet.gameObject.transform.position = firePoint.position;
        bullet.gameObject.transform.rotation = firePoint.rotation;

        bullet.gameObject.SetActive(true);

        Vector3 perfectTarget = GetPredictedPlayerPosition(projectileForce, firePoint.position);
        Vector3 finalTarget = perfectTarget;

        bool isPerfectShot = Random.value <= perfectShotChance;

        if (!isPerfectShot)
        { 
            float distance = Vector3.Distance(firePoint.position, perfectTarget);
            float distanceFactor = Mathf.Clamp(distance / 15f, 0.2f, 1.0f);
            Vector2 randomCircle = Random.insideUnitCircle * maxAimError * distanceFactor;
            Vector3 errorOffset = new Vector3(randomCircle.x, 0f, randomCircle.y);

            finalTarget += errorOffset;
        }

        Vector3 dir = (finalTarget - firePoint.position).normalized;
        Debug.DrawLine(firePoint.position, perfectTarget, Color.green, 2f);

        if (!isPerfectShot)
        {
            // Se c'è stato un errore, disegna una linea ROSSA per farti vedere dove sbanda il colpo
            Debug.DrawLine(firePoint.position, finalTarget, Color.red, 2f);
        }
        else
        {
            // Se è un tiro perfetto, disegna un raggio BIANCO per confermarlo
            Debug.DrawLine(firePoint.position, finalTarget, Color.white, 2f);
        }
 
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