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

    public LayerMask collisionMask;
    public int trajectorySteps = 50; 

    private MouseLook playerMouseLook;
    private Transform arrivingPoint;

    void Start()
    {
        if (combat == null) combat = GetComponentInParent<PlayerCombat>();
        if (interaction == null) interaction = GetComponentInParent<PlayerInteraction>();
        if (audioPlayer == null) audioPlayer = GetComponentInParent<AudioPlayerController>();

        playerMouseLook = GetComponentInParent<MouseLook>();
        arrivingPoint = transform.Find("ArrivingPoint");

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

        Vector3 hitPoint = SimulateTrajectory(startPos, velocity, realGravity, trajectorySteps, collisionMask);

        if (arrivingPoint != null)
        {
            arrivingPoint.position = hitPoint + Vector3.up * 0.2f;
            arrivingPoint.rotation = Quaternion.Euler(90f, 0f, 0f);

          
            arrivingPoint.localScale = new Vector3(explosionRadius * 2f, explosionRadius * 2f, 1f);
        }
    }

    public void ThrowGrenade()
    {
        if (interaction != null) interaction.removeGrenade();

        arrivingPoint.gameObject.SetActive(false);

        if (audioPlayer != null) audioPlayer.playThrow();

        if (Camera.main == null) return;

        Vector3 spawnPos = Camera.main.transform.position + Camera.main.transform.forward * 0.5f;
        Vector3 initialVelocity = Camera.main.transform.forward * granadeSpeed;

        GameObject g = Instantiate(grenadePrefab, spawnPos, Quaternion.identity);
        Granade granadeScript = g.GetComponent<Granade>();

        if (granadeScript != null)
        {
            granadeScript.maxDamage = damage;
            granadeScript.radius = 1.25f;
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

    Vector3 SimulateTrajectory(Vector3 startPos, Vector3 velocity, float gravity, int steps, LayerMask mask)
    {
        Vector3 pos = startPos;
        Vector3 vel = velocity;

        float timeStep = Time.fixedDeltaTime;

        for (int i = 0; i < steps; i++)
        {

            vel += Vector3.down * gravity * timeStep;
            Vector3 nextPos = pos + vel * timeStep;

            if (Physics.Linecast(pos, nextPos, out RaycastHit hit, mask))
            {
                return hit.point;
            }

            if (pos.y > 0 && nextPos.y <= 0)
            {
                float t = pos.y / (pos.y - nextPos.y);
                return Vector3.Lerp(pos, nextPos, t);
            }

            pos = nextPos;
        }

        return pos;
    }

    public void Activation()
    {

        arrivingPoint.gameObject.SetActive(true);
        transform.Find("Granade").gameObject.SetActive(false);
    }



    public void EquipGun()
    {
        if (combat != null)
        {
            combat.SwitchToWeapon();
        }
    }

}
