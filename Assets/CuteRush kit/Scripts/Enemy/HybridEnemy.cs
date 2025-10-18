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
            Debug.LogError($"Bullet Prefab not assigned on {name}!", this);
            return;
        }
        for (int i = 0; i < bulletPoolSize; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            EnemyBullet bulletScript = bullet.GetComponent<EnemyBullet>();
            if (bulletScript != null)
            {
                bulletScript.damage = rangedAttackDamage;
            }
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
            FacePlayer();
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

            if (Time.time - lastRangedAttackTime >= rangedAttackCooldown)
            {
                Vector3 directionToPlayer = (player.position - transform.position).normalized;
                float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

                if (angleToPlayer > facingTolerance)
                {
                    return; 
                }
                lastRangedAttackTime = Time.time;
                animator.SetTrigger("Shoot");
            }

        }
        else
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
            player.GetComponent<VitalsController>()?.Decrease(attackDamage);
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

        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            Vector3 targetPoint = player.position + Vector3.up * 0.5f;
            Vector3 dir = (targetPoint - firePoint.position).normalized;
            rb.AddForce(dir * projectileForce, ForceMode.VelocityChange);
        }
        
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
            player.GetComponent<VitalsController>()?.Decrease(attackDamage);
        }
    }

    void FacePlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * agent.angularSpeed * 0.1f); 
    }
}