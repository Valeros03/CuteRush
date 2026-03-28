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

        // Il raycast parte dal centro esatto della telecamera (il tuo crosshair)
        Vector3 startPos = Camera.main.transform.position + Camera.main.transform.forward * 0.5f;
        Vector3 velocity = Camera.main.transform.forward * granadeSpeed;

        // IMPORTANTE: Usiamo la gravità reale di Unity per far combaciare la linea blu con la fisica!
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

        // Usiamo l'intervallo di tempo ESATTO che usa la fisica di Unity (di default 0.02f)
        float timeStep = Time.fixedDeltaTime;

        for (int i = 0; i < steps; i++)
        {
            // IL SEGRETO DI UNITY: Semi-Implicit Euler Integration
            // Prima si applica la gravità alla velocità...
            vel += Vector3.down * gravity * timeStep;

            // ...E POI si calcola la nuova posizione! (Prima facevamo il contrario)
            Vector3 nextPos = pos + vel * timeStep;

            // Controllo della collisione
            if (Physics.Linecast(pos, nextPos, out RaycastHit hit, mask))
            {
                return hit.point;
            }

            // Fallback nel caso attraversi il pavimento (z = 0) che avevi scritto tu
            if (pos.y > 0 && nextPos.y <= 0) // (Ho cambiato Z in Y, perché il pavimento di solito è in basso, sull'asse Y!)
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
