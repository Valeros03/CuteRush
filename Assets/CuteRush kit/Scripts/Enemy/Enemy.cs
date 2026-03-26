using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public enum AIState
{
    Dormant,
    Chasing,
    Returning
}

public abstract class Enemy : MonoBehaviour
{
    [Header("AI Behavior")]
    [Tooltip("Raggio fisico del trigger SUL NEMICO per rilevare il player (Q2)")]
    public float personalTriggerRadius = 15f; // Raggio dello SphereCollider

    [Header("Base Enemy Settings")]
    public float maxHealth = 3f;
    public int attackDamage = 1;
    public float attackSpeed = 1f;
    public float chaseSpeed = 3.5f;
    public float rotationSpeed = 5f;
    public float flinchDuration = 0.3f;
    public Face faces;
    public bool isDead;
    public GameObject SlimeBody;
    [SerializeField] private Rigidbody rb;
    [SerializeField] protected EnemyHitRecoil hitRecoil;

    protected NavMeshAgent agent;
    protected Transform player;
    protected Animator animator;
    protected EnemySpawner mySpawnManager;


    protected bool isTakingDamage;
    protected bool isPlayerInAttackArea;
    protected bool isChasing;

    protected float currentHealth;

    private AIState currentState = AIState.Dormant;
    private Vector3 homePosition;
    protected Material faceMaterial;
    private Coroutine flinchCoroutine;
    private bool isPlayerInPersonalTrigger = false;


    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        hitRecoil = GetComponentInChildren<EnemyHitRecoil>();
        currentHealth = maxHealth;

        if (SlimeBody != null)
        {
            Renderer renderer = SlimeBody.GetComponent<Renderer>();
            Material[] instancedMaterials = renderer.materials;
            if (instancedMaterials.Length > 1) faceMaterial = instancedMaterials[1];
        }
        animator.SetFloat("AttackSpeed", attackSpeed);
    }

    protected virtual void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform; 

        SphereCollider aggroTrigger = GetComponent<SphereCollider>();
        if (aggroTrigger != null && aggroTrigger.isTrigger)
            aggroTrigger.radius = personalTriggerRadius;

        if (agent != null) { agent.speed = chaseSpeed; agent.angularSpeed = rotationSpeed * 100f; }
        if (rb != null) rb.isKinematic = true;
    }

    public void OnEnable()
    {
        // 1. Reset Statistiche base
        currentHealth = maxHealth;
        isDead = false;
        isTakingDamage = false;
        isPlayerInPersonalTrigger = false;
        flinchCoroutine = null;

        // 2. Reset Collider
        foreach (Collider col in GetComponents<Collider>())
        {
            if (!col.isTrigger) col.enabled = true;
        }

        // 3. Reset Grafica (Faccia)
        if (faces.Idleface != null) SetFace(faces.Idleface);



        // 4. Reset Animator (IL FIX MAGICO È QUI)
        if (animator != null)
        {
            animator.enabled = true;

            // A. Cancella manualmente i Trigger per evitare loop fantasma
            animator.ResetTrigger("Die");
            animator.ResetTrigger("Attack");
            animator.ResetTrigger("Shoot");

            // B. Forza l'Animator allo stato di default bypassando le transizioni
            animator.Play("Locomotion", 0, 0f);

            animator.SetFloat("AttackSpeed", attackSpeed);
            animator.SetFloat("Speed", 0f);
        }

        // 5. Reset NavMeshAgent
        if (agent != null)
        {
            agent.enabled = true;
            agent.isStopped = false;
            agent.ResetPath();
            agent.velocity = Vector3.zero;
        }

        // Reset dello stato della Macchina a Stati
        SetState(AIState.Dormant);
    }


    public void Initialize(Vector3 homePos, EnemySpawner manager) 
    {
        homePosition = homePos;
        mySpawnManager = manager; 
        SetState(AIState.Dormant);
    }

    protected virtual void Update()
    {
        if (isDead || player == null || isTakingDamage) return;
        if (agent == null || !agent.isOnNavMesh) return;
        if (mySpawnManager == null)
        {
            return;
        }

        bool q1_isPlayerInArea = mySpawnManager.IsPlayerInArea;
        bool q2_isPlayerInPersonal = isPlayerInPersonalTrigger;
        AIState s0_currentState = currentState;

        AIState nextState = DetermineNextState(s0_currentState, q1_isPlayerInArea, q2_isPlayerInPersonal);

        if (s0_currentState == AIState.Returning)
        {
            if (!agent.pathPending && agent.remainingDistance < agent.stoppingDistance + 2.0f)
            {
                nextState = AIState.Dormant; 
            }
        }

        if (nextState != s0_currentState)
        {
            SetState(nextState);
        }


        switch (currentState) 
        {
            case AIState.Dormant:
                break;
            case AIState.Chasing:
                PerformChaseLogic();
                break;
            case AIState.Returning:
                HandleReturningMovement();
                break;
        }
    }

    private AIState DetermineNextState(AIState current, bool q1_inArea, bool q2_inPersonal)
    {
        switch (current)
        {
            case AIState.Dormant:
                if (q1_inArea && q2_inPersonal) return AIState.Chasing;
                else return AIState.Dormant;

            case AIState.Chasing:
                if (!q1_inArea) return AIState.Returning;
                else return AIState.Chasing;

            case AIState.Returning:
                if (q1_inArea) return AIState.Chasing;
                else return AIState.Returning; 

            default:
                return current;
        }
    }

    protected abstract void PerformChaseLogic();

    protected abstract void PerformAttack();

    private void HandleReturningMovement()
    {
        agent.SetDestination(homePosition);
        agent.isStopped = false;
        animator.SetFloat("Speed", agent.velocity.magnitude);
        SetFace(faces.WalkFace);
    }

    private void SetState(AIState newState)
    {
        if (currentState == newState) return;

       currentState = newState;

        switch (currentState)
        {
            case AIState.Dormant:

                if (agent.isOnNavMesh && agent.enabled) agent.isStopped=true;
                animator.SetFloat("Speed", 0);
                SetFace(faces.Idleface);

                break;
            case AIState.Chasing:
                if (agent.isOnNavMesh && agent.enabled) agent.isStopped = false;
  
                break;
            case AIState.Returning:
                if (agent.isOnNavMesh && agent.enabled) agent.isStopped = false;
                SetFace(faces.Idleface);
                break;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInPersonalTrigger = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
           isPlayerInPersonalTrigger = false;
        }
    }

    protected void SetFace(Texture tex)
    {
        if (faceMaterial != null && tex != null)
            faceMaterial.SetTexture("_MainTex", tex);
    }

    public virtual void TakeDamage(float damageAmount, Vector3 shotDirection, Vector3 hitPoint)
    {
        if (isDead) return;

        // Se stavo già subendo danno, devo resettare l'animator prima di applicare il nuovo danno
        if (isTakingDamage && flinchCoroutine != null)
        {
            StopCoroutine(flinchCoroutine);
            if (animator != null) animator.enabled = true; // Riattivo preventivamente
            if (agent != null && agent.isOnNavMesh) agent.isStopped = false;
        }

        isTakingDamage = true;
        currentHealth -= damageAmount;

        SetFace(faces.damageFace);

        if (currentHealth <= 0)
        {
            Die(shotDirection, hitPoint);
        }
        else
        {
            if (flinchCoroutine != null)
            {
                StopCoroutine(flinchCoroutine);
            }
            flinchCoroutine = StartCoroutine(DamageFlinchRoutine(shotDirection, hitPoint));
        }
    }

    protected virtual IEnumerator DamageFlinchRoutine(Vector3 shotDirection, Vector3 hitPoint)
    {

        if (agent.isOnNavMesh && agent.enabled)
        {
            agent.isStopped = true;
        }

        if (animator != null)
        {
            animator.enabled = false;
           
        }

        if (hitRecoil != null)
        {
            hitRecoil.ApplyHit(shotDirection, hitPoint);
        }



        yield return new WaitForSeconds(flinchDuration);

        isTakingDamage = false;
        flinchCoroutine = null;
        if (isDead) yield break; 

        if (agent.isOnNavMesh && agent.enabled && currentState != AIState.Dormant)
        {
            agent.isStopped = false;
        }
        
        if (animator != null)
        {
            animator.enabled = true;
            animator.Play("Locomotion", -1, 0f);
            animator.SetFloat("Speed", 0f);

        }

    }


    protected virtual void Die(Vector3 shotDirection, Vector3 hitPoint)
    {
        if (isDead) return;
        isDead = true;

        // Ferma il flinch se è in corso
        if (flinchCoroutine != null)
        {
            StopCoroutine(flinchCoroutine);
            flinchCoroutine = null;
        }

        isTakingDamage = false;

        // Disabilita collider
        foreach (Collider col in GetComponents<Collider>())
        {
            if (!col.isTrigger)
                col.enabled = false;
        }

        // Ferma l'agente
        if (agent != null && agent.enabled)
        {
            agent.isStopped = true;
            agent.enabled = false;
        }

        // --- CORREZIONE QUI ---
        if (animator != null)
        {
            // IMPORTANTE: Se stavi flinchando, l'animator era spento. 
            // Devi riaccenderlo ORA per poter suonare l'animazione di morte.
            animator.enabled = true;
            animator.SetTrigger("Die");
        }

        StartCoroutine(DisableAfterTime(3f));
    }

    protected IEnumerator DisableAfterTime(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
    }



}

