using UnityEngine;
using UnityEngine.AI;

public class MeleeEnemy : Enemy
{
    [Header("Melee Settings")]
    public float meleeRange = 1.5f;


    protected override void Start()
    {
        base.Start();

        if (agent != null)
        {
            agent.stoppingDistance = meleeRange;
        }
    }

    // In MeleeEnemy.cs
    protected override void PerformChaseLogic()
    {
        if (!agent.isOnNavMesh) return;

 
        float distanceSqr = (transform.position - player.position).sqrMagnitude;
        float meleeRangeSqr = meleeRange * meleeRange;

        if (distanceSqr <= meleeRangeSqr * 1.1f)
        {
         
            agent.isStopped = true;
            FacePlayer();

            animator.SetFloat("Speed", 0f);
            SetFace(faces.attackFace);
            animator.SetTrigger("Attack");
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
        
        if (Vector3.Distance(transform.position, player.position) <= meleeRange + 0.5f) // Tolleranza
        {
            player.GetComponent<VitalsController>()?.Decrease(attackDamage);
        }
    }


    void FacePlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * agent.angularSpeed);
    }
}