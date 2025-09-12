using UnityEngine;
using UnityEngine.AI;

public enum SlimeAnimationState { Idle, Walk, Attack, Damage }

public class EnemyAi : MonoBehaviour
{
    public Face faces;
    public GameObject SmileBody;
    public SlimeAnimationState currentState;

    public Animator animator;
    public NavMeshAgent agent;
    public int damType;

    private Material faceMaterial;
    private Transform player;
    private bool isChasing = false;

    void Start()
    {
        faceMaterial = SmileBody.GetComponent<Renderer>().materials[1];
        currentState = SlimeAnimationState.Idle;
        SetFace(faces.Idleface);
        agent.isStopped = true;
    }

    void SetFace(Texture tex)
    {
        faceMaterial.SetTexture("_MainTex", tex);
    }

    // Trigger dei figli
    public void HandleTriggerEnter(TriggerCollider trigger, Collider playerCollider)
    {
        player = playerCollider.transform;

        if (trigger.gameObject.name == "AttackTrigger")
        {
            currentState = SlimeAnimationState.Attack;
            agent.isStopped = true;
            SetFace(faces.attackFace);
            animator.SetTrigger("Attack");
        }
        else if (trigger.gameObject.name == "DetectionTrigger")
        {
            isChasing = true;
            currentState = SlimeAnimationState.Walk;
            agent.isStopped = false;
            SetFace(faces.WalkFace);
        }
    }

    public void HandleTriggerExit(TriggerCollider trigger, Collider playerCollider)
    {
        if (trigger.gameObject.name == "AttackTrigger")
        {
            // torna a chase se esce dall'area attack
            currentState = SlimeAnimationState.Walk;
            agent.isStopped = false;
            SetFace(faces.WalkFace);
        }
    }

    void Update()
    {
        if (isChasing && player != null && currentState == SlimeAnimationState.Walk)
        {
            agent.SetDestination(player.position);
            animator.SetFloat("Speed", agent.velocity.magnitude);

            // ruota verso il player
            Vector3 direction = (player.position - transform.position).normalized;
            if (direction != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
            }
        }
        else if (currentState == SlimeAnimationState.Idle)
        {
            animator.SetFloat("Speed", 0);
        }
    }
}
