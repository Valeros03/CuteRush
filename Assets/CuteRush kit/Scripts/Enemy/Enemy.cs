using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public enum AIState
{
    Dormant,
    Chasing,
    Returning
}

public abstract class Enemy : MonoBehaviour
{
    [Header("AI Behavior")]
    public float personalTriggerRadius = 15f;

    [Header("Base Enemy Settings (Giorno 1)")]
    public float maxHealth = 3f;
    public int attackDamage = 1;
    public float attackSpeed = 1f;
    protected float chaseSpeed = 3.5f;
    public float rotationSpeed = 5f;
    public float flinchDuration = 0.3f;
    public float flinchCooldown = 1.0f;
    public AnimationCurve speedScalingCurve;

    public Face faces;
    public bool isDead;
    public GameObject SlimeBody;
    [SerializeField] private Rigidbody rb;
    [SerializeField] protected EnemyHitRecoil hitRecoil;
    [SerializeField] private GameObject marker;

    [Header("Kill Points")]
    public int killPoints;

    protected NavMeshAgent agent;
    protected Transform player;
    protected Animator animator;
    protected EnemySpawner mySpawnManager;

    protected bool isTakingDamage;
    protected bool isPlayerInAttackArea;
    protected bool isChasing;

    protected float currentHealth;
    protected float scaledMaxHealth;
    protected int scaledAttackDamage;


    private AIState currentState = AIState.Dormant;
    private Vector3 homePosition;
    protected Material faceMaterial;
    private Coroutine flinchCoroutine;
    private bool isPlayerInPersonalTrigger = false;
    private float lastFlinchTime = -10f;
    private GameObject itemDropPrefab;

    protected VitalsController _targetVitals;

    [Header("Smart Aim Analysis")]
    public float observationWindow = 0.4f;

    private Queue<PlayerRecord> playerHistory = new Queue<PlayerRecord>();

    [Header("Riflessi Umani (Cognitive Inertia)")]
    public float aiAdaptationSpeed = 2f;

    [Header("Tactical Shooting Rules")]
    public float aimTolerance = 10f;

    [Header("Aim Inaccuracy (Weapon Spread)")]
    public float maxAimError = 1.5f;
    [Range(0f, 1f)] public float perfectShotChance = 0.3f;

    private Vector3 smoothedAimVelocity;

    private struct PlayerRecord
    {
        public Vector3 position;
        public float time;
        public PlayerRecord(Vector3 p, float t) { position = p; time = t; }
    }

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        hitRecoil = GetComponentInChildren<EnemyHitRecoil>();

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
        SphereCollider aggroTrigger = GetComponent<SphereCollider>();
        if (aggroTrigger != null && aggroTrigger.isTrigger)
            aggroTrigger.radius = personalTriggerRadius;

        if (rb != null) rb.isKinematic = true;
    }

    public virtual void OnEnable()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        marker.SetActive(true);
        float diffMultiplier = 1.0f;
        if (DifficultyManager.Instance != null)
        {
            diffMultiplier = DifficultyManager.Instance.currentMultiplier;
        }


        if (player != null)
        {
            _targetVitals = player.GetComponent<VitalsController>();
        }

        scaledMaxHealth = maxHealth * diffMultiplier;
        scaledAttackDamage = Mathf.RoundToInt(attackDamage * diffMultiplier);

        currentHealth = scaledMaxHealth;

        if (agent != null)
        {

            agent.speed = speedScalingCurve.Evaluate(diffMultiplier);
            agent.angularSpeed = rotationSpeed * 100f;
        }

        isDead = false;
        isTakingDamage = false;
        isPlayerInPersonalTrigger = false;
        flinchCoroutine = null;

        foreach (Collider col in GetComponents<Collider>())
        {
            if (!col.isTrigger) col.enabled = true;
        }

        if (faces.Idleface != null) SetFace(faces.Idleface);

        if (animator != null)
        {
            animator.enabled = true;
            animator.Rebind();
            animator.ResetTrigger("Die");
            animator.Play("Locomotion", 0, 0f);
            animator.Update(0f);
            animator.SetFloat("AttackSpeed", attackSpeed);
            animator.SetFloat("Speed", 0f);
        }

        if (agent != null)
        {
            agent.enabled = true;
            agent.isStopped = false;
            agent.ResetPath();
            agent.velocity = Vector3.zero;
        }

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

        TrackPlayerMovement();

        if (agent == null || !agent.isOnNavMesh) return;
        if (mySpawnManager == null) return;

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
                if (agent.isOnNavMesh && agent.enabled) agent.isStopped = true;
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
        if (other.CompareTag("Player")) isPlayerInPersonalTrigger = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) isPlayerInPersonalTrigger = false;
    }

    protected void SetFace(Texture tex)
    {
        if (faceMaterial != null && tex != null)
            faceMaterial.SetTexture("_MainTex", tex);
    }

    public virtual void TakeDamage(float damageAmount, Vector3 shotDirection, Vector3 hitPoint)
    {
        if (isDead) return;

        currentHealth -= damageAmount;
        SetFace(faces.damageFace);

        if (currentHealth <= 0)
        {
            Die(shotDirection, hitPoint);
        }
        else
        {
            if (Time.time >= lastFlinchTime + flinchCooldown)
            {
                if (flinchCoroutine != null) StopCoroutine(flinchCoroutine);
                lastFlinchTime = Time.time;
                flinchCoroutine = StartCoroutine(DamageFlinchRoutine(shotDirection, hitPoint));
            }
        }
    }

    protected virtual IEnumerator DamageFlinchRoutine(Vector3 shotDirection, Vector3 hitPoint)
    {
        if (agent.isOnNavMesh && agent.enabled) agent.isStopped = true;
        if (animator != null) animator.enabled = false;
        if (hitRecoil != null) hitRecoil.ApplyHit(shotDirection, hitPoint);

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

    public void SetDropItem(GameObject dropPrefab)
    {
        itemDropPrefab = dropPrefab;
    }

    protected virtual void Die(Vector3 shotDirection, Vector3 hitPoint)
    {
        if(GameManager.Instance!=null)
        GameManager.Instance.AddKillScore(killPoints);
        marker.SetActive(false);
        if (isDead) return;
        isDead = true;

        if (flinchCoroutine != null)
        {
            StopCoroutine(flinchCoroutine);
            flinchCoroutine = null;
        }

        isTakingDamage = false;

        foreach (Collider col in GetComponents<Collider>())
        {
            if (!col.isTrigger) col.enabled = false;
        }

        if (agent != null && agent.enabled)
        {
            agent.isStopped = true;
            agent.enabled = false;
        }

        if (animator != null)
        {
            animator.enabled = true;
            animator.SetTrigger("Die");
        }

        StartCoroutine(DisableAfterTime(3f));
    }

    protected IEnumerator DisableAfterTime(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (itemDropPrefab != null)
        {
            Instantiate(itemDropPrefab, transform.position + Vector3.up * 0.5f, itemDropPrefab.transform.rotation);
            itemDropPrefab = null;
        }

        gameObject.SetActive(false);
    }

    protected Vector3 GetPredictedPlayerPosition(float bulletSpeed, Vector3 originPoint)
    {
        Vector3 targetPoint = player.position + Vector3.up * 0.5f;

        Vector3 instantVelocity = Vector3.zero;
        PlayerMovement pMove = player.GetComponent<PlayerMovement>();
        if (pMove != null) instantVelocity = pMove.currentVelocity;

        Vector3 historicalVelocity = Vector3.zero;
        if (playerHistory.Count > 1)
        {
            PlayerRecord oldestRecord = playerHistory.Peek();
            Vector3 movementVector = player.position - oldestRecord.position;
            float timePassed = Time.time - oldestRecord.time;
            if (timePassed > 0) historicalVelocity = movementVector / timePassed;
        }
        else historicalVelocity = instantVelocity;

        float blendFactor = 0.5f;
        Vector3 idealVelocity = Vector3.Lerp(historicalVelocity, instantVelocity, blendFactor);
        smoothedAimVelocity = Vector3.Lerp(smoothedAimVelocity, idealVelocity, Time.deltaTime * aiAdaptationSpeed);

        Vector3 finalVelocity = smoothedAimVelocity;
        if (finalVelocity.y < 0) finalVelocity.y = 0;
        if (finalVelocity.magnitude < 0.5f) return targetPoint;

        float distance = Vector3.Distance(originPoint, targetPoint);
        float timeToHit = distance / bulletSpeed;
        timeToHit = Mathf.Min(timeToHit, 1.2f);
        float dampening = 1.0f;

        // Restituisce SEMPRE il calcolo perfetto
        return targetPoint + (finalVelocity * timeToHit * dampening);
    }

    private void TrackPlayerMovement()
    {
        playerHistory.Enqueue(new PlayerRecord(player.position, Time.time));
        while (playerHistory.Count > 0 && (Time.time - playerHistory.Peek().time) > observationWindow)
        {
            playerHistory.Dequeue();
        }
    }

}