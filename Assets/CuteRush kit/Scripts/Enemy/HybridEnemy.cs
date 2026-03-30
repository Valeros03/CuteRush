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
    public float projectileForce = 15f;
    public int rangedAttackDamage = 1; 


    [Header("Pooling")]
    public int bulletPoolSize = 5;
    private List<GameObject> bulletPool;
    private float lastRangedAttackTime;

    
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
        if (bulletPrefab == null)
        {
            return;
        }
        for (int i = 0; i < bulletPoolSize; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            // Non impostiamo i danni qui
            bullet.SetActive(false);
            bulletPool.Add(bullet);
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
            agent.SetDestination(player.position);
            animator.SetFloat("Speed", agent.velocity.magnitude);

            Vector3 predictedTarget = GetPredictedPlayerPosition(projectileForce, firePoint.position);

            FaceTarget(predictedTarget);

            if (Time.time - lastRangedAttackTime >= rangedAttackCooldown)
            {
                Vector3 directionToTarget = (predictedTarget - transform.position).normalized;
                directionToTarget.y = 0;

                Vector3 currentForward = transform.forward;
                currentForward.y = 0;

                float angleToTarget = Vector3.Angle(currentForward, directionToTarget);

                if (angleToTarget > facingTolerance)
                {
                    return;
                }

                lastRangedAttackTime = Time.time;
                animator.SetTrigger("Shoot");
            }
        }else
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
            SetFace(faces.WalkFace);
            animator.SetFloat("Speed", agent.velocity.magnitude);
        }
    }

    protected override void PerformAttack()
    {
        if (Vector3.Distance(transform.position, player.position) <= meleeRange + 0.4f) 
        {
            player.GetComponent<VitalsController>()?.Decrease(attackDamage, transform.position, true);
        }
    }


    void FireProjectile()
    {
        GameObject bullet = GetPooledBullet();
        if (bullet == null) return;

        bullet.transform.position = firePoint.position;
        bullet.transform.rotation = firePoint.rotation;

        SetFace(faces.attackFace);
        bullet.SetActive(true);

        StartCoroutine(nameof(faceShootAnimate));

        Vector3 predictedTarget = GetPredictedPlayerPosition(projectileForce, firePoint.position);
        Vector3 dir = (predictedTarget - firePoint.position).normalized;

        bullet.GetComponent<EnemyBullet>().Fire(dir, rangedAttackDamage, projectileForce, transform.position);
    }

    IEnumerator faceShootAnimate()
    {
        SetFace(faces.attackFace);
        yield return new WaitForSeconds(0.3f);
        SetFace(faces.WalkFace);
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

   
    public void TriggerMeleeDamage() 
    {

        if (Vector3.Distance(transform.position, player.position) <= meleeRange + 0.5f)
        {
            player.GetComponent<VitalsController>()?.Decrease(attackDamage, transform.position);
        }
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