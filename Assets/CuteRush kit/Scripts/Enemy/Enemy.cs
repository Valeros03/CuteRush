using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public enum SlimeAnimationState { Idle, Walk, Attack, Damage }

public abstract class Enemy : MonoBehaviour
{
    [Header("Base Enemy Settings")]
    public float currentHealth = 3f;
    public int attackDamage = 1;
    public float attackSpeed = 1f;
    public float chaseSpeed = 3.5f;
    public float rotationSpeed = 5f;

    [Header("References")]
    public Animator animator;
    public NavMeshAgent agent;
    public GameObject SmileBody;
    public Face faces;

    [Header("Attack Range Settings")]
    public float attackRange = 3f;     // Distanza massima per attaccare
    public float minDistance = 2f;     // Distanza minima per non avvicinarsi troppo


    protected Transform player;
    protected Material faceMaterial;

    protected bool isDead;
    protected bool isTakingDamage;
    protected bool isPlayerInAttackArea;
    protected bool isChasing;

   

    public SlimeAnimationState currentState;

    protected virtual void Start()
    {
        faceMaterial = SmileBody.GetComponent<Renderer>().materials[1];
        agent.speed = chaseSpeed;
        SetFace(faces.Idleface);
        currentState = SlimeAnimationState.Idle;

        // trova il player una volta
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player != null)
        {
            StartChasing();
        }
    }

    protected virtual void Update()
    {
        if (isDead || player == null) return;

        if (isTakingDamage)
        {
            animator.SetFloat("Speed", 0);
            agent.isStopped = true;
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Se dentro il range d’attacco ma non troppo vicino
        if (distanceToPlayer <= attackRange)
        {
            agent.isStopped = true;
            FacePlayer();
            currentState = SlimeAnimationState.Attack;
            animator.SetTrigger("Attack");
            SetFace(faces.attackFace);
            return;
        }

        // Se fuori range -> insegue
        if (isChasing)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
            animator.SetFloat("Speed", agent.velocity.magnitude);
            SetFace(faces.WalkFace);
            FacePlayer();
            currentState = SlimeAnimationState.Walk;
        }
    }
    private void FacePlayer()
    {
        Vector3 dir = (player.position - transform.position).normalized;
        dir.y = 0; // evita rotazione verticale
        if (dir != Vector3.zero)
        {
            Quaternion lookRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * rotationSpeed);
        }
    }


    protected void SetFace(Texture tex) => faceMaterial.SetTexture("_MainTex", tex);

    public virtual void HandleTriggerEnter(TriggerCollider trigger, Collider playerCollider)
    {
        if (isDead) return;
        player = playerCollider.transform;

        if (trigger.gameObject.name == "AttackTrigger")
        {
            agent.isStopped = true;
            currentState = SlimeAnimationState.Attack;
            SetFace(faces.attackFace);
            animator.SetTrigger("Attack");
            isPlayerInAttackArea = true;
        }
    }

    public virtual void HandleTriggerExit(TriggerCollider trigger, Collider playerCollider)
    {
        if (trigger.gameObject.name == "AttackTrigger")
        {
            isPlayerInAttackArea = false;
            agent.isStopped = false;
            currentState = SlimeAnimationState.Walk;
            SetFace(faces.WalkFace);
        }
    }

    public void TakeDamage(float dmg, Vector3 shotDir, Vector3 hitPoint)
    {
        if (isDead) return;
        isTakingDamage = true;

        currentHealth -= dmg;
        currentState = SlimeAnimationState.Damage;
        SetFace(faces.damageFace);

        if (currentHealth <= 0)
        {
            Die(shotDir, hitPoint);
        }
        else
        {
            GetComponentInChildren<EnemyHitRecoil>()?.ApplyHit(shotDir);
            Invoke(nameof(ResumeChaseOrAttack), 0.5f);
        }
    }

    protected virtual void ResumeChaseOrAttack()
    {
        isTakingDamage = false;
        if (isDead) return;

        if (isPlayerInAttackArea)
        {
            currentState = SlimeAnimationState.Attack;
            animator.SetTrigger("Attack");
        }
        else
        {
            currentState = SlimeAnimationState.Walk;
            agent.isStopped = false;
            SetFace(faces.WalkFace);
        }
    }

    protected virtual void StartChasing()
    {
        isChasing = true;
        currentState = SlimeAnimationState.Walk;
        agent.isStopped = false;
        SetFace(faces.WalkFace);
    }

    protected virtual void Die(Vector3 shotDir, Vector3 hitPoint)
    {
        isDead = true;
        GetComponentInChildren<EnemyHitRecoil>()?.DieAndRagdoll(shotDir, hitPoint);
        StartCoroutine(DisableAfterDelay(1.5f));
    }

    private IEnumerator DisableAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
    }

    // --- da implementare nei figli ---
    protected abstract void PerformAttack();
}
