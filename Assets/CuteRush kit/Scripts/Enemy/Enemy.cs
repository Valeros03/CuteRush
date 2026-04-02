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

    [Header("Base Enemy Settings (Giorno 1)")]
    public float maxHealth = 3f;
    public int attackDamage = 1;
    public float attackSpeed = 1f;
    protected float chaseSpeed = 3.5f;
    public float rotationSpeed = 5f;
    public float flinchDuration = 0.3f;
    public float flinchCooldown = 1.0f;
    public float flinchChance = 0.3f;
    public AnimationCurve speedScalingCurve;

    public Face faces;
    public bool isDead;
    public GameObject SlimeBody;
    [SerializeField] private Rigidbody rb;
    [SerializeField] protected EnemyHitRecoil hitRecoil;
    [SerializeField] private GameObject marker;

    [Header("Visual Feedback (Pooling)")]
    public ParticleSystem hitParticlePrefab;
    public int hitParticlePoolSize = 3;

    private List<ParticleSystem> hitParticlePool = new List<ParticleSystem>();

    [Header("Health Visuals")]
    public Color healthyColor = Color.white;
    public Color nearDeathColor = Color.red;

    private Renderer slimeRenderer;
    private MaterialPropertyBlock propBlock;

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

    [Header("AI Behavior")]
    public float personalTriggerRadius = 15f;

    [Header("Riflessi Umani (Cognitive Inertia)")]
    public float aiAdaptationSpeed = 2f;

    [Header("Tactical Shooting Rules")]
    public float aimTolerance = 10f;

    [Header("Aim Inaccuracy (Weapon Spread)")]
    public float maxAimError = 1.5f;
    [Range(0f, 1f)] public float perfectShotChance = 0.3f;

    private Vector3 smoothedAimVelocity;

    [Header("Dual-Window Psychology")]
    [Tooltip("Tempo in secondi per i riflessi a breve termine (es. 0.2)")]
    public float shortWindowTime = 0.2f;
    [Tooltip("Tempo in secondi per l'analisi strategica (es. 1.5)")]
    public float longWindowTime = 1.5f;

    [Header("Vision Settings")]
    public LayerMask obstacleMask;
    public float projectileRadius = 0.3f;

    protected struct PlayerRecord
    {
        public Vector3 position;
        public float time;
        public PlayerRecord(Vector3 p, float t) { position = p; time = t; }
    }

    protected List<PlayerRecord> playerHistory = new List<PlayerRecord>();

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
        animator.SetFloat(GameConstants.ANIM_ATTACK_SPEED, attackSpeed);

        if (hitParticlePrefab != null)
        {
            for (int i = 0; i < hitParticlePoolSize; i++)
            {
                ParticleSystem p = Instantiate(hitParticlePrefab, transform.position, Quaternion.identity, transform);
                p.gameObject.SetActive(false);
                hitParticlePool.Add(p);
            }
        }

        if (SlimeBody != null)
        {
            slimeRenderer = SlimeBody.GetComponent<Renderer>();
        }
        else
        {
            slimeRenderer = GetComponentInChildren<Renderer>();
        }
        propBlock = new MaterialPropertyBlock();
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
        player = GameObject.FindGameObjectWithTag(GameConstants.PLAYER_TAG)?.transform;
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

        Debug.Log(diffMultiplier);

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


        if (playerHistory != null) playerHistory.Clear();
        smoothedAimVelocity = Vector3.zero;

        foreach (Collider col in GetComponents<Collider>())
        {
            if (!col.isTrigger) col.enabled = true;
        }

        if (faces.Idleface != null) SetFace(faces.Idleface);

        if (animator != null)
        {
            animator.enabled = true;
            animator.Rebind();
            animator.ResetTrigger(GameConstants.ANIM_DIE);
            animator.Play(GameConstants.ANIM_LOCOMOTION, 0, 0f);
            animator.Update(0f);
            animator.SetFloat(GameConstants.ANIM_ATTACK_SPEED, attackSpeed);
            animator.SetFloat(GameConstants.ANIM_SPEED, 0f);
        }

        if (agent != null)
        {
            agent.enabled = false;
            agent.enabled = true;

            agent.isStopped = false;
            agent.ResetPath();
            agent.velocity = Vector3.zero;
        }

        if (slimeRenderer != null && propBlock != null)
        {
            slimeRenderer.GetPropertyBlock(propBlock);
            propBlock.SetColor("_Color", healthyColor);
            slimeRenderer.SetPropertyBlock(propBlock);
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

        TrackPlayerHistory();

        if (agent == null || !agent.isOnNavMesh) return;
        if (mySpawnManager == null) return;

        bool q1_isPlayerInArea = mySpawnManager.IsPlayerInArea;
        bool q2_isPlayerInPersonal = isPlayerInPersonalTrigger;
        AIState s0_currentState = currentState;

        AIState nextState = DetermineNextState(s0_currentState, q1_isPlayerInArea, q2_isPlayerInPersonal);

        if (s0_currentState == AIState.Returning)
        {
            if (agent.HasReachedDestination())
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
        animator.SetFloat(GameConstants.ANIM_SPEED, agent.velocity.magnitude);
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
                animator.SetFloat(GameConstants.ANIM_SPEED, 0);
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
        if (other.CompareTag(GameConstants.PLAYER_TAG)) isPlayerInPersonalTrigger = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(GameConstants.PLAYER_TAG)) isPlayerInPersonalTrigger = false;
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

        if (slimeRenderer != null && maxHealth > 0)
        {
            float healthPercent = currentHealth / scaledMaxHealth;
            slimeRenderer.GetPropertyBlock(propBlock);
            Color currentColor = Color.Lerp(nearDeathColor, healthyColor, healthPercent);
            propBlock.SetColor("_Color", currentColor);
            slimeRenderer.SetPropertyBlock(propBlock);
        }

        if (hitParticlePrefab != null)
        {
            ParticleSystem splat = GetPooledHitParticle();
            if (splat != null)
            {
                splat.transform.SetParent(null);
                splat.transform.position = hitPoint;
                splat.transform.rotation = Quaternion.LookRotation(-shotDirection);

                splat.gameObject.SetActive(true);
                splat.Play();

                StartCoroutine(DisableParticleAfterTime(splat, 1.0f));
            }
        }


        if (currentHealth <= 0)
        {
            Die(shotDirection, hitPoint);
        }
        else
        {
            if (Time.time >= lastFlinchTime + flinchCooldown)
            {
                if (Random.value <= flinchChance)
                {
                    isTakingDamage = true;
                    SetFace(faces.damageFace);
                    if (flinchCoroutine != null) StopCoroutine(flinchCoroutine);
                    lastFlinchTime = Time.time;
                    flinchCoroutine = StartCoroutine(DamageFlinchRoutine(shotDirection, hitPoint));
                }
            }
        }
    }

    protected virtual IEnumerator DamageFlinchRoutine(Vector3 shotDirection, Vector3 hitPoint)
    {
        if (agent.isOnNavMesh && agent.enabled) agent.isStopped = true;

        InterruptAttack();

        if (animator != null) animator.enabled = false;
        if (hitRecoil != null) hitRecoil.ApplyHit(shotDirection, hitPoint);

        yield return new WaitForSeconds(flinchDuration);

        isTakingDamage = false;
        flinchCoroutine = null;
        if (isDead) yield break;

        if (animator != null) animator.enabled = true;

        if (agent.isOnNavMesh && agent.enabled && currentState != AIState.Dormant)
        {
            agent.isStopped = false;
        }
    }

    protected virtual void InterruptAttack()
    {
        if (animator != null)
        {
            animator.ResetTrigger(GameConstants.ANIM_ATTACK);
            animator.ResetTrigger(GameConstants.ANIM_SHOOT);
            animator.Play(GameConstants.ANIM_LOCOMOTION, 0, 0f);
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
        SetFace(faces.damageFace);
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
            animator.SetTrigger(GameConstants.ANIM_DIE);
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

    protected Vector3 GetPredictedPlayerPosition(float bulletSpeed, Vector3 fireOrigin)
    {
        Vector3 targetPoint = player.position + Vector3.up * 0.5f;

        Vector3 shortTermVel = GetVelocityOverWindow(shortWindowTime);
        Vector3 longTermVel = GetVelocityOverWindow(longWindowTime);

        float dotProduct = Vector3.Dot(shortTermVel.normalized, longTermVel.normalized);
        float aiConfidence = Mathf.Clamp01((dotProduct + 1f) / 2f);

        Vector3 chosenVelocity = Vector3.Lerp(longTermVel, shortTermVel, aiConfidence);

        smoothedAimVelocity = Vector3.Lerp(smoothedAimVelocity, chosenVelocity, Time.deltaTime * aiAdaptationSpeed);

        Vector3 finalVelocity = smoothedAimVelocity;
        if (finalVelocity.y < 0) finalVelocity.y = 0;

        float distance = Vector3.Distance(fireOrigin, targetPoint);
        float timeToHit = distance / bulletSpeed;
        timeToHit = Mathf.Min(timeToHit, 1.2f);

        float predictionDampening = Mathf.Lerp(0.0f, 1.0f, aiConfidence);

        return targetPoint + (finalVelocity * timeToHit * predictionDampening);
    }

    private Vector3 GetVelocityOverWindow(float timeWindow)
    {
        if (playerHistory.Count < 2) return Vector3.zero;

        float targetTime = Time.time - timeWindow;
        PlayerRecord pastRecord = playerHistory[playerHistory.Count - 1];

        for (int i = playerHistory.Count - 1; i >= 0; i--)
        {
            if (playerHistory[i].time <= targetTime)
            {
                pastRecord = playerHistory[i];
                break;
            }
            pastRecord = playerHistory[i];
        }

        Vector3 movement = player.position - pastRecord.position;
        float timePassed = Time.time - pastRecord.time;

        if (timePassed > 0.01f) return movement / timePassed;
        return Vector3.zero;
    }

    private void TrackPlayerHistory()
    {
        if (player == null) return;

        playerHistory.Add(new PlayerRecord(player.position, Time.time));

        while (playerHistory.Count > 0 && Time.time - playerHistory[0].time > longWindowTime + 0.2f)
        {
            playerHistory.RemoveAt(0);
        }
    }

    protected bool HasClearShot(Vector3 fireOrigin, Vector3 targetPoint)
    {
        Vector3 directionToTarget = targetPoint - fireOrigin;
        float distanceToTarget = directionToTarget.magnitude;

        if (Physics.SphereCast(fireOrigin, projectileRadius, directionToTarget.normalized, out RaycastHit hit, distanceToTarget, obstacleMask))
        {
            return false;
        }

        return true;
    }

    private ParticleSystem GetPooledHitParticle()
    {
        foreach (ParticleSystem p in hitParticlePool)
        {
            if (!p.gameObject.activeInHierarchy) return p;
        }
        return null;
    }

    private IEnumerator DisableParticleAfterTime(ParticleSystem particle, float delay)
    {
        yield return new WaitForSeconds(delay);
        particle.gameObject.SetActive(false);
        particle.transform.SetParent(transform);
    }
}