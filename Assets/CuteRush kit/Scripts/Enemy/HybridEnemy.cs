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

        if (distanceSqr <= meleeRangeSqr * 1.1f)
        {
            agent.isStopped = true;
            FaceTarget(player.position);
            animator.SetFloat("Speed", 0f);

            if (!animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack"))
            {
                SetFace(faces.attackFace);
                animator.SetTrigger("Attack");
            }
        }
        else if (distanceSqr <= rangedRangeSqr * 1.1f)
        {
            agent.isStopped = false;
            agent.stoppingDistance = meleeRange;
            agent.SetDestination(player.position);
            animator.SetFloat("Speed", agent.velocity.magnitude);

            Vector3 predictedTarget = GetPredictedPlayerPosition(projectileForce, firePoint.position);

            if (HasClearShot(firePoint.position, predictedTarget))
            {
                FaceTarget(predictedTarget);

                Vector3 planarForward = transform.forward.WithY(0);
                Vector3 planarDir = predictedTarget - transform.position; planarDir.y = 0;

                if (Vector3.Angle(planarForward, planarDir) <= aimTolerance)
                {
                    if (Time.time - lastRangedAttackTime >= rangedAttackCooldown)
                    {
                        lastRangedAttackTime = Time.time + Random.Range(-0.1f, 0.2f);
                        animator.SetTrigger("Shoot");
                        StartCoroutine(faceShootAnimate());
                    }
                }
            }
            else
            {
                SetFace(faces.WalkFace);
            }
        }else
        {

            agent.isStopped = false;
            agent.stoppingDistance = meleeRange;

            agent.SetDestination(player.position);
            animator.SetFloat("Speed", agent.velocity.magnitude);
            SetFace(faces.WalkFace);
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