using UnityEngine;
using UnityEngine.AI;

public class MeleeEnemy : Enemy
{
    [Header("Melee Settings")]
    public float meleeRange = 1.5f;

    [Header("Smart Pathfinding")]
    public float pathUpdateDelay = 0.25f;
    private float lastPathUpdateTime;

    protected override void Start()
    {
        base.Start();

        if (agent != null)
        {
            agent.stoppingDistance = meleeRange;
            agent.autoBraking = false;
        }
        SetFace(faces.attackFace);
    }

    public override void OnEnable()
    {
        base.OnEnable();
        lastPathUpdateTime = 0f;
    }

    protected override void PerformChaseLogic()
    {
        if (!agent.isOnNavMesh) return;

        float distanceSqr = (transform.position - player.position).sqrMagnitude;
        float meleeRangeSqr = meleeRange * meleeRange;

        float heightDifference = Mathf.Abs(transform.position.y - player.position.y);
        bool canReachPlayer = agent.pathStatus == NavMeshPathStatus.PathComplete || agent.pathPending;

        bool trulyReachable = canReachPlayer && heightDifference < 1.5f;

        if (trulyReachable && distanceSqr <= meleeRangeSqr * 1.1f)
        {
            agent.isStopped = true;
            FacePlayer();

            animator.SetFloat(GameConstants.ANIM_SPEED, 0f);

            if (!animator.GetCurrentAnimatorStateInfo(0).IsTag(GameConstants.ANIM_ATTACK))
            {
                SetFace(faces.attackFace);
                animator.SetTrigger(GameConstants.ANIM_ATTACK);
            }
        }else
        {
            agent.isStopped = false;
            agent.stoppingDistance = trulyReachable ? meleeRange : 0.5f;

            UpdatePathSmart();

            SetFace(faces.WalkFace);
            animator.SetFloat(GameConstants.ANIM_SPEED, agent.velocity.magnitude);
        }
    }

    private void UpdatePathSmart()
    {
        if (Time.time - lastPathUpdateTime > pathUpdateDelay)
        {
            lastPathUpdateTime = Time.time;
            NavMeshPath path = new NavMeshPath();

            if (NavMesh.CalculatePath(transform.position, player.position, NavMesh.AllAreas, path) && path.status != NavMeshPathStatus.PathInvalid)
            {
                agent.SetPath(path);
            }
            else
            {
                NavMeshHit hit;
                if (NavMesh.SamplePosition(player.position, out hit, 15f, NavMesh.AllAreas))
                {
                    if (NavMesh.CalculatePath(transform.position, hit.position, NavMesh.AllAreas, path))
                    {
                        agent.SetPath(path);
                    }
                }
            }
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
        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * agent.angularSpeed);
        }
    }
}