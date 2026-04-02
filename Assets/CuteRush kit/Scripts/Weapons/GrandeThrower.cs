using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GrandeThrower : MonoBehaviour
{
    [SerializeField] private PlayerCombat combat;
    [SerializeField] private PlayerInteraction interaction;
    [SerializeField] private AudioPlayerController audioPlayer;
    [SerializeField] private float granadeSpeed;

    public float explosionRadius = 5f;
    public GameObject grenadePrefab;
    public float damage;

    [Header("Calibrazione Segnalatore Visivo")]
    [Range(0.01f, 5f)] public float indicatorCorrection = 0.5f;

    [Header("Tactical Aiming")]
    [Tooltip("0.05 = Rallentatore estremo mentre miri")]
    [Range(0f, 1f)] public float aimTimeScale = 0.5f;

    public LayerMask collisionMask;
    public int trajectorySteps = 50;

    private MouseLook playerMouseLook;
    private Transform arrivingPoint;
    private Transform centerMark;

    private Quaternion markInitialRotation;

    void Start()
    {
        if (combat == null) combat = GetComponentInParent<PlayerCombat>();
        if (interaction == null) interaction = GetComponentInParent<PlayerInteraction>();
        if (audioPlayer == null) audioPlayer = GetComponentInParent<AudioPlayerController>();

        playerMouseLook = GetComponentInParent<MouseLook>();
        arrivingPoint = transform.Find("ArrivingPoint");
        centerMark = transform.Find("CenterMark");

        if (centerMark != null)
        {
            markInitialRotation = centerMark.rotation;
        }
    }

    void Update()
    {
        UpdateArrivingPoint();
    }

    void UpdateArrivingPoint()
    {
        if (Camera.main == null) return;

        Vector3 startPos = Camera.main.transform.position + Camera.main.transform.forward * 0.5f;
        Vector3 velocity = Camera.main.transform.forward * granadeSpeed;

        float realGravity = Mathf.Abs(Physics.gravity.y);

        Vector3 hitNormal;
        Vector3 hitPoint = SimulateTrajectory(startPos, velocity, realGravity, trajectorySteps, collisionMask, out hitNormal);

        Quaternion surfaceTilt = Quaternion.FromToRotation(Vector3.up, hitNormal);

        if (arrivingPoint != null)
        {
            arrivingPoint.position = hitPoint + hitNormal * 0.2f;
            arrivingPoint.rotation = surfaceTilt * Quaternion.Euler(90f, 0f, 0f);

            float finalScale = (explosionRadius * 2f) * indicatorCorrection;
            arrivingPoint.localScale = new Vector3(finalScale, finalScale, 1f);
        }

        if (centerMark != null)
        {
            centerMark.position = hitPoint + hitNormal * 0.21f;
            centerMark.rotation = surfaceTilt * markInitialRotation;
        }
    }

    public void ThrowGrenade()
    {
        Time.timeScale = 1f;

        if (interaction != null) interaction.removeGrenade();

        if (arrivingPoint != null) arrivingPoint.gameObject.SetActive(false);
        if (centerMark != null) centerMark.gameObject.SetActive(false);

        if (audioPlayer != null) audioPlayer.playThrow();

        if (Camera.main == null) return;

        Vector3 spawnPos = Camera.main.transform.position + Camera.main.transform.forward * 0.5f;
        Vector3 initialVelocity = Camera.main.transform.forward * granadeSpeed;

        GameObject g = Instantiate(grenadePrefab, spawnPos, Quaternion.identity);
        Granade granadeScript = g.GetComponent<Granade>();

        if (granadeScript != null)
        {
            granadeScript.maxDamage = damage;
            granadeScript.radius = explosionRadius;
        }

        Rigidbody rb = g.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = initialVelocity;
            rb.useGravity = true;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
        }
    }

    Vector3 SimulateTrajectory(Vector3 startPos, Vector3 velocity, float gravity, int steps, LayerMask mask, out Vector3 hitNormal)
    {
        Vector3 pos = startPos;
        Vector3 vel = velocity;
        float timeStep = 0.02f;

        for (int i = 0; i < steps; i++)
        {
            vel += Vector3.down * gravity * timeStep;
            Vector3 nextPos = pos + vel * timeStep;

            if (Physics.Linecast(pos, nextPos, out RaycastHit hit, mask))
            {
                hitNormal = hit.normal;
                return hit.point;
            }

            if (pos.y > 0 && nextPos.y <= 0)
            {
                float t = pos.y / (pos.y - nextPos.y);
                hitNormal = Vector3.up;
                return Vector3.Lerp(pos, nextPos, t);
            }

            pos = nextPos;
        }

        hitNormal = Vector3.up;
        return pos;
    }

    public void Activation()
    {
        Time.timeScale = aimTimeScale;

        if (arrivingPoint != null) arrivingPoint.gameObject.SetActive(true);
        if (centerMark != null) centerMark.gameObject.SetActive(true);

        transform.Find("Granade").gameObject.SetActive(false);
    }

    public void EquipGun()
    {
        Time.timeScale = 1f;

        if (combat != null)
        {
            combat.SwitchToWeapon();
        }
    }
}