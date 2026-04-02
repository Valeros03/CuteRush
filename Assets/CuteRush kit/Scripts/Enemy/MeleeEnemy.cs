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

    protected override void PerformChaseLogic()
    {
        if (!agent.isOnNavMesh) return;

 
        float distanceSqr = (transform.position - player.position).sqrMagnitude;
        float meleeRangeSqr = meleeRange * meleeRange;

        if (distanceSqr <= meleeRangeSqr * 1.1f)
        {
         
            agent.isStopped = true;
            FacePlayer();

            animator.SetFloat(GameConstants.ANIM_SPEED, 0f);
            SetFace(faces.attackFace);
            animator.SetTrigger(GameConstants.ANIM_ATTACK);
        }
        else
        {
         
            agent.isStopped = false;
            agent.SetDestination(player.position);
            SetFace(faces.WalkFace);

            
            animator.SetFloat(GameConstants.ANIM_SPEED, agent.velocity.magnitude);
        }
    }

    protected override void PerformAttack()
    {
        
        if (Vector3.Distance(transform.position, player.position) <= meleeRange + 0.5f)
        {
            _targetVitals.Decrease(scaledAttackDamage, transform.position, true);
        }
    }


    void FacePlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * agent.angularSpeed);
    }
}