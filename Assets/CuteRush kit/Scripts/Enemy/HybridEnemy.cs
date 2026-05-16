using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Collections;

public class HybridEnemy : Enemy
{
    [Header("Hybrid Settings")]
    public float meleeRange = 2f;
    public float rangedRange = 15f;
    public float facingTolerance = 15.0f;

    [Header("Ranged Attack Specifics")]
    public float rangedAttackCooldown = 2.0f;
    public GameObject bulletPrefab;
    public Transform firePoint;
    public AnimationCurve projectileForceCurve;
    public int rangedAttackDamage = 1;

    [Header("Smart Pathfinding")]
    public float pathUpdateDelay = 0.25f;
    private float lastPathUpdateTime;

    [Header("Pooling")]
    public int bulletPoolSize = 5;
    private List<EnemyBullet> bulletPool;
    private float lastRangedAttackTime;

    protected int scaledRangedDamage;
    protected float projectileForce = 15f;

    protected override void Start()
    {
        base.Start();
        InitializeBulletPool();

        if (agent != null)
        {
            agent.stoppingDistance = meleeRange;
            agent.autoBraking = false;
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
        lastRangedAttackTime = Time.time;
        SetFace(faces.attackFace);

    }

    void InitializeBulletPool()
    {
        bulletPool = new List<EnemyBullet>();
        if (bulletPrefab == null) return;

        for (int i = 0; i < bulletPoolSize; i++)
        {
            GameObject bulletObj = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            bulletObj.SetActive(false);

            EnemyBullet bulletComp = bulletObj.GetComponent<EnemyBullet>();
            if (bulletComp != null)
            {
                bulletPool.Add(bulletComp);
            }
        }
    }

    protected override void PerformChaseLogic()
    {
        if (!agent.isOnNavMesh) return;

        float distanceSqr = (transform.position - player.position).sqrMagnitude;
        float meleeRangeSqr = meleeRange * meleeRange;
        float rangedRangeSqr = rangedRange * rangedRange;

        float heightDifference = Mathf.Abs(transform.position.y - player.position.y);
        bool canReachPlayer = agent.pathStatus == NavMeshPathStatus.PathComplete || agent.pathPending;
        bool trulyReachable = canReachPlayer && heightDifference < 1.5f;

        Vector3 predictedTarget = GetPredictedPlayerPosition(projectileForce, firePoint.position);
        bool hasClearShot = HasClearShot(firePoint.position, predictedTarget);

        if (trulyReachable && distanceSqr <= meleeRangeSqr * 1.1f)
        {
            agent.isStopped = true;
            FaceTarget(player.position);
            animator.SetFloat(GameConstants.ANIM_SPEED, 0f);

            if (!animator.GetCurrentAnimatorStateInfo(0).IsTag(GameConstants.ANIM_ATTACK))
            {
                SetFace(faces.attackFace);
                animator.SetTrigger(GameConstants.ANIM_ATTACK);
            }
        }
        else if (distanceSqr <= rangedRangeSqr * 1.1f && hasClearShot)
        {
            agent.isStopped = true;
            agent.stoppingDistance = meleeRange;
            animator.SetFloat(GameConstants.ANIM_SPEED, 0f);

            FaceTarget(predictedTarget);

            Vector3 planarForward = transform.forward.WithY(0);
            Vector3 planarDir = predictedTarget - transform.position; planarDir.y = 0;

            if (Vector3.Angle(planarForward, planarDir) <= aimTolerance)
            {
                if (Time.time - lastRangedAttackTime >= rangedAttackCooldown)
                {
                    lastRangedAttackTime = Time.time + Random.Range(-0.1f, 0.2f);
                    animator.SetTrigger(GameConstants.ANIM_SHOOT);
                    StartCoroutine(faceShootAnimate());
                }
            }
        }
        else
        {
            agent.isStopped = false;
            agent.stoppingDistance = 0.5f;

            if (Time.time - lastPathUpdateTime > pathUpdateDelay)
            {
                lastPathUpdateTime = Time.time;

                NavMeshPath path = new NavMeshPath();
                if (NavMesh.CalculatePath(transform.position, player.position, NavMesh.AllAreas, path))
                {
                    if (path.status != NavMeshPathStatus.PathInvalid)
                    {
                        agent.SetPath(path);
                    }
                }
            }

            SetFace(faces.WalkFace);
            animator.SetFloat(GameConstants.ANIM_SPEED, agent.velocity.magnitude);
        }
    }

    protected override void PerformAttack()
    {
        if (Vector3.Distance(transform.position, player.position) <= meleeRange + 0.4f) 
        {
            _targetVitals.Decrease(scaledAttackDamage, transform.position, true);
        }
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

    private void TryShoot(Vector3 target)
    {
        Vector3 planarForward = transform.forward.WithY(0);
        Vector3 planarDir = target - transform.position; planarDir.y = 0;

        if (Vector3.Angle(planarForward, planarDir) <= aimTolerance)
        {
            if (Time.time - lastRangedAttackTime >= rangedAttackCooldown)
            {
                lastRangedAttackTime = Time.time + Random.Range(-0.1f, 0.2f);
                animator.SetTrigger(GameConstants.ANIM_SHOOT);
                StartCoroutine(faceShootAnimate());
            }
        }
    }

    IEnumerator faceShootAnimate()
    {
        SetFace(faces.attackFace);
        yield return new WaitForSeconds(0.3f);
        SetFace(faces.WalkFace);
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

    void FaceTarget(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        direction.y = 0;

        if (direction == Vector3.zero) return;

        Quaternion lookRotation = Quaternion.LookRotation(direction);
        float smoothAimSpeed = 5.0f;
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * smoothAimSpeed);
    }
}