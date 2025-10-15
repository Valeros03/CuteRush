using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public enum SlimeAnimationState { Idle, Walk, Attack, Damage }

public class Enemy : MonoBehaviour
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
    public float currentHealth = 3;
    private bool isDead = false;
    
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
        if(isDead) return;
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
        if(isDead) return;

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
    public void TakeDamage(float damageAmount, Vector3 shotDirection, Vector3 hitPoint)
    {
        if (isDead) return;

        isTakingDamage = true;

        Damage(damageAmount); //deal damage

        currentState = SlimeAnimationState.Damage;
        SetFace(faces.damageFace); //animate face


        //applica la hit come animazione dai colpi
        if (currentHealth <= 0)
        {
            isDead = true;
            StartCoroutine(nameof(Die));
            gameObject.GetComponentInChildren<EnemyHitRecoil>().DieAndRagdoll(shotDirection, hitPoint);

        }
        else 
        {
            gameObject.GetComponentInChildren<EnemyHitRecoil>().ApplyHit(shotDirection);
        }

        Invoke(nameof(ResumeChaseOrAttack), 0.5f); //se non muore ritorna ad attaccare o inseguire
    }

    private void ResumeChaseOrAttack()
    {

        isTakingDamage = false;
        if (!gameObject.activeInHierarchy || isDead) return;
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
    

    public void Damage(float damageAmount)
    {
        //subtract damage amount when Damage function is called
        currentHealth -= damageAmount;

    }

    private IEnumerator Die()
    {
        yield return new WaitForSeconds(1.5f);
        gameObject.SetActive(false);
    }

   

}
