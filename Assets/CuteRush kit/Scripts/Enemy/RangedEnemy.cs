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
        lastAttackTime = Time.time;
        isAttacking = false;
    }

    protected override void PerformChaseLogic()
    {
        if (!agent.isOnNavMesh) return;

        float distanceSqr = (transform.position - player.position).sqrMagnitude;
        float rangeAttackdistanceSqr = rangeAttackdistance * rangeAttackdistance;

        if (distanceSqr <= rangeAttackdistanceSqr * 1.1f)
        {
            Vector3 predictedTarget = GetPredictedPlayerPosition(projectileForce, firePoint.position);

            if (HasClearShot(firePoint.position, predictedTarget))
            {
                agent.isStopped = true;
                agent.stoppingDistance = rangeAttackdistance;

                animator.SetFloat(GameConstants.ANIM_SPEED, 0f);
                FaceTarget(predictedTarget);

                Vector3 planarForward = transform.forward.WithY(0);
                Vector3 planarDir = predictedTarget - transform.position; planarDir.y = 0;

                if (Vector3.Angle(planarForward, planarDir) <= aimTolerance)
                {
                    if (Time.time - lastAttackTime > attackCooldown && !isAttacking)
                    {
                        lastAttackTime = Time.time + Random.Range(-0.1f, 0.2f);
                        SetFace(faces.attackFace);
                        animator.SetTrigger(GameConstants.ANIM_ATTACK);
                        isAttacking = true;
                    }
                }
            }
            else
            {
                if (!isAttacking)
                {
                    agent.isStopped = false;
                    agent.stoppingDistance = 0.5f;

                    agent.SetDestination(player.position);
                    SetFace(faces.WalkFace);
                    animator.SetFloat(GameConstants.ANIM_SPEED, agent.velocity.magnitude);
                }
            }
        }
        else if (!isAttacking)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
            SetFace(faces.WalkFace);
            animator.SetFloat(GameConstants.ANIM_SPEED, agent.velocity.magnitude);
        }
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

    protected override void InterruptAttack()
    {
        base.InterruptAttack();
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