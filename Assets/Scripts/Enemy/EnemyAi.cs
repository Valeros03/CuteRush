using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public enum SlimeAnimationState { Idle, Walk, Attack, Damage }

public class EnemyAi : MonoBehaviour
{
    public Face faces;
    public GameObject SmileBody;
    public SlimeAnimationState currentState;
    public int Attack;
    public float attackSpeed;

    public Animator animator;
    public NavMeshAgent agent;
    public int damType;

    private Material faceMaterial;
    private Transform player;
    private bool isChasing = false;
    private bool isPlayerInAttackArea = false;

    // Aggiungi questa variabile per sapere se sta facendo animazione damage
    private bool isTakingDamage = false;

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
            isPlayerInAttackArea = true;
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
            isPlayerInAttackArea = false;
            currentState = SlimeAnimationState.Walk;
            agent.isStopped = false;
            SetFace(faces.WalkFace);
        }
    }

    void Update()
    {
        // Se sta facendo animazione damage, blocca il movimento
        if (isTakingDamage)
        {
            animator.SetFloat("Speed", 0);
            agent.isStopped = true;
            return;
        }

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

    // Funzione da chiamare quando il nemico subisce danno
    public void TakeDamage(int damageAmount)
    {
        if (isTakingDamage) return; // evita sovrapposizioni

        isTakingDamage = true;
        currentState = SlimeAnimationState.Damage;
        animator.SetTrigger("Damage");
        SetFace(faces.damageFace);
        gameObject.GetComponent<EnemyController>().Damage(damageAmount); //take damage

        // Dopo 0.5 secondi (durata animazione), torna a chase/attack
        Invoke(nameof(ResumeChaseOrAttack), 0.5f);
    }

    private void ResumeChaseOrAttack()
    {
        isTakingDamage = false;

        if (player != null)
        {
            // Controlla distanza per decidere se tornare in attack o walk
            float distance = Vector3.Distance(transform.position, player.position);
            if (distance <= 2f) // esempio: sotto 2 metri passa in attack
            {
                currentState = SlimeAnimationState.Attack;
                agent.isStopped = true;
                SetFace(faces.attackFace);
                animator.SetTrigger("Attack");
            }
            else
            {
                currentState = SlimeAnimationState.Walk;
                agent.isStopped = false;
                SetFace(faces.WalkFace);
            }
        }
        else
        {
            currentState = SlimeAnimationState.Idle;
            SetFace(faces.Idleface);
            agent.isStopped = true;
        }
    }

     private void DealDamage()
     {
        if(isPlayerInAttackArea)
            player.GetComponent<VitalsController>().Decrease(Attack);     
     }

}
