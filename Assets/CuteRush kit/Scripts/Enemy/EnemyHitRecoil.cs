using UnityEngine;

public class EnemyHitRecoil : MonoBehaviour
{
    [Tooltip("Forza del rinculo quando il nemico viene colpito")]
    public float recoilForce = 0.15f;
    [Tooltip("Forza della rotazione del colpo")]
    public float rotationForce = 10f;
    [Tooltip("Tempo di ritorno alla posizione originale")]
    public float recoveryTime = 0.3f;

    private Vector3 targetPosOffset;
    private Quaternion targetRotOffset;

    private Vector3 currentPosOffset;
    private Quaternion currentRotOffset;

    private Vector3 originalPos;
    private Quaternion originalRot;

    private float t = 1f;
    private bool isRecoiling = false;

    [Header("Ragdoll settings")]
    [Tooltip("Rigidbodies dei bone (setto isKinematic true in Inspector)")]
    public Rigidbody[] ragdollRigidbodies;
    [Tooltip("Forza dell'impulso applicato al ragdoll")]
    public float ragdollForce = 6f;
    [Tooltip("Torque random per far girare")]
    public float ragdollTorque = 3f;

    private bool isDead = false;

    [SerializeField] private Animator animator;
    [SerializeField] private UnityEngine.AI.NavMeshAgent agent;
    private Transform rootTransform;

    private void Start()
    {
        originalPos = transform.localPosition;
        originalRot = transform.localRotation;
    }

    private void Awake()
    {
        if (ragdollRigidbodies == null || ragdollRigidbodies.Length == 0)
            ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();

        rootTransform = transform.parent;
    }

    private void Update()
    {
        if (isRecoiling)
        {
            t += Time.deltaTime / recoveryTime;
            float curve = Mathf.SmoothStep(0, 1, t);

            currentPosOffset = Vector3.Lerp(targetPosOffset, Vector3.zero, curve);
            currentRotOffset = Quaternion.Slerp(targetRotOffset, Quaternion.identity, curve);

            if (t >= 1f)
                isRecoiling = false;

            ApplyTransform();
        }
    }

    public void ApplyHit(Vector3 hitDirection, Vector3 hitPoint)
    {
     
        Vector3 recoilDir = -hitDirection.normalized;
        Vector3 localRecoilDir = rootTransform.InverseTransformDirection(recoilDir);
        targetPosOffset = localRecoilDir * recoilForce;


  
        Vector3 hitVector = transform.InverseTransformPoint(hitPoint);
        Vector3 primaryAxis = Vector3.Cross(hitVector.normalized, localRecoilDir.normalized);

        float randomRotationStrength = Random.Range(rotationForce * 0.7f, rotationForce * 1.3f);


        Quaternion primaryRotation = Quaternion.AngleAxis(randomRotationStrength, primaryAxis);


        float randomWobble = Random.Range(-rotationForce * 0.5f, rotationForce * 0.5f);
        Quaternion wobbleRotation = Quaternion.AngleAxis(randomWobble, rootTransform.up); 


        targetRotOffset = primaryRotation * wobbleRotation;

        t = 0f;
        isRecoiling = true;
    }

    private void ApplyTransform()
    {
        transform.localPosition = originalPos + currentPosOffset;
        transform.localRotation = originalRot * currentRotOffset;
    }


    public void DieAndRagdoll(Vector3 shotDirection, Vector3 hitPoint)
    {
        if (isDead) return;
        isDead = true;

        animator.enabled = false;
        if (agent) agent.enabled = false;

        foreach (var rb in ragdollRigidbodies)
        {
            rb.isKinematic = false;
            rb.detectCollisions = true;
            // applica una spinta vicino al punto d'impatto per effetto più realistico
            rb.AddForceAtPosition(shotDirection * ragdollForce, hitPoint, ForceMode.Impulse);
            // piccolo torque casuale
            Vector3 torque = Random.onUnitSphere * ragdollTorque;
            rb.AddTorque(torque, ForceMode.Impulse);
        }

    }
}
