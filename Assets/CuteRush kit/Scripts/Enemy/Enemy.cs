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
    public float aggroRange = 15f;

    [Header("Base Enemy Settings")]
    public float currentHealth = 3f;
    public int attackDamage = 1;
    public float attackSpeed = 1f;
    public float chaseSpeed = 3.5f;
    public float rotationSpeed = 5f;
    public float flinchDuration = 0.3f;
    public Face faces;
    public GameObject SlimeBody;
    [SerializeField] private Rigidbody rb;
    [SerializeField] protected EnemyHitRecoil hitRecoil;

    protected NavMeshAgent agent;
    protected Transform player;
    protected Animator animator;

    protected bool isDead;
    protected bool isTakingDamage;
    protected bool isPlayerInAttackArea;
    protected bool isChasing;

    private AIState currentState = AIState.Dormant;
    private Vector3 homePosition;
    private float maxLeashRangeSqr;
    protected Material faceMaterial;
    private Coroutine flinchCoroutine;


    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

      
        if (SlimeBody != null) faceMaterial = SlimeBody.GetComponent<Renderer>().materials[1];

        animator.SetFloat("AttackSpeed", attackSpeed);
    }

    protected virtual void Start()
    {
   
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        // E la configurazione (che ora può usare 'agent' in sicurezza)
        SphereCollider aggroTrigger = GetComponent<SphereCollider>();
        if (aggroTrigger != null && aggroTrigger.isTrigger)
            aggroTrigger.radius = aggroRange;
        else
            Debug.LogWarning($"Nemico {name} non ha uno SphereCollider (Trigger) per l'aggro range.");

        if (agent != null)
        {
            agent.speed = chaseSpeed;
            agent.angularSpeed = rotationSpeed * 100f;
        }
    }

    public void Initialize(Vector3 homePos, float leashRange)
    {
        homePosition = homePos;
        maxLeashRangeSqr = leashRange * leashRange;
        SetState(AIState.Dormant);
    }

    protected virtual void Update()
    {
     
        if (isDead || player == null || isTakingDamage)
        {
            return;
        }

        if (agent == null || !agent.isOnNavMesh)
        {
            return;
        }


        switch (currentState)
        {
            case AIState.Dormant:
                Debug.Log("Dormant");
                break;
            case AIState.Chasing:
                HandleChasingState();
                break;
            case AIState.Returning:
                Debug.Log("Returning");
                HandleReturningState();
                break;
        }
    }

    protected abstract void PerformChaseLogic();

    protected abstract void PerformAttack();

    private void HandleChasingState()
    {
        float distFromHomeSqr = (transform.position - homePosition).sqrMagnitude;
        if (distFromHomeSqr > maxLeashRangeSqr)
        {
            SetState(AIState.Returning);
        }
        else
        {
            PerformChaseLogic();
        }
    }

    private void HandleReturningState()
    {
        agent.SetDestination(homePosition);
        agent.isStopped = false;
        animator.SetFloat("Speed", agent.velocity.magnitude);

        if (!agent.pathPending && agent.remainingDistance < agent.stoppingDistance + 1f)
        {
            SetState(AIState.Dormant);
        }
    }

    private void SetState(AIState newState)
    {
        currentState = newState;
        
        switch (currentState)
        {
            case AIState.Dormant:
                if (agent.isOnNavMesh) agent.isStopped = true;
                animator.SetFloat("Speed", 0);
                SetFace(faces.Idleface);
                break;
            case AIState.Chasing:
                if (agent.isOnNavMesh) agent.isStopped = false;
                break;
            case AIState.Returning:
                if (agent.isOnNavMesh) agent.isStopped = false;
                SetFace(faces.Idleface);
                break;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (currentState == AIState.Dormant && other.CompareTag("Player"))
        {
            SetState(AIState.Chasing);
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
            // Avvia il NUOVO flinch e salva un riferimento
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

        if (flinchCoroutine != null)
        {
            StopCoroutine(flinchCoroutine);
            flinchCoroutine = null;
        }


        StopAllCoroutines();
        isTakingDamage = false;

        foreach (Collider col in GetComponents<Collider>())
            col.enabled = false;

        if (hitRecoil != null)
        {
            hitRecoil.DieAndRagdoll(shotDirection, hitPoint);
        }
        else
        {
            animator.SetTrigger("Die");
            if (agent.isOnNavMesh)
            {
                agent.isStopped = true;
                agent.enabled = false;
            }
        }

        Destroy(gameObject, 5f);
    }

}

