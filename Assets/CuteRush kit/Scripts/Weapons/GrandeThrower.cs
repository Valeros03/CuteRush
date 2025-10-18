using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GrandeThrower : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    [SerializeField] private float granadeSpeed;
    [SerializeField] private float gravity;
    public GameObject grenadePrefab;
    public float damage;

    public LayerMask collisionMask;
    public int trajectorySteps = 50; 

    private MouseLook playerMouseLook;
    private Transform arrivingPoint;

    void Start()
    {
        playerMouseLook = player.GetComponentInChildren<MouseLook>();
        arrivingPoint = transform.Find("ArrivingPoint");

    }

    void Update()
    {
        UpdateArrivingPoint();
    }

    void UpdateArrivingPoint()
    {
        Vector3 startPos = transform.position;
        


        Vector3 dir = Quaternion.Euler(playerMouseLook.transform.eulerAngles.x, playerMouseLook.transform.eulerAngles.y, 0f) * Vector3.forward;
        startPos = transform.position + dir * 0.5f + Vector3.up * 0.5f;
        Vector3 velocity = dir * granadeSpeed;

        Vector3 hitPoint = SimulateTrajectory(startPos, velocity, gravity, trajectorySteps, collisionMask);

        if (arrivingPoint != null)
        {
            arrivingPoint.position = hitPoint + Vector3.up * 0.2f;
            arrivingPoint.rotation = Quaternion.Euler(90f, 0f, 0f);
        }
    }

    Vector3 SimulateTrajectory(Vector3 startPos, Vector3 velocity, float gravity, int steps, LayerMask mask)
    {
        Vector3 pos = startPos;
        Vector3 vel = velocity;
        float timeStep = 0.1f;

        for (int i = 0; i < steps; i++)
        {
            Vector3 nextPos = pos + vel * timeStep;
            vel += Vector3.down * gravity * timeStep;

            if (Physics.Linecast(pos, nextPos, out RaycastHit hit, mask))
            {
                return hit.point;
            }

            if (pos.z > 0 && nextPos.z <= 0)
            {
                float t = pos.z / (pos.z - nextPos.z);
                Vector3 groundHit = Vector3.Lerp(pos, nextPos, t);
                return groundHit;
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

    public void ThrowGrenade()
    {

        arrivingPoint.gameObject.SetActive(false);

        player.GetComponent<AudioPlayerController>().playThrow();

        Vector3 spawnPos;
        if (transform.position != null)
            spawnPos = transform.position;
        else if (arrivingPoint != null)
            spawnPos = arrivingPoint.position + Vector3.up * 0.2f; 
        else
            spawnPos = transform.position + transform.forward * 0.5f;

        Vector3 horizontalDir;
        if (arrivingPoint != null)
        {
            Vector3 delta = arrivingPoint.position - spawnPos;

            delta.y = 0f;

            if (delta.sqrMagnitude > 0.001f)
                horizontalDir = delta.normalized;
            else
                horizontalDir = Vector3.forward; // fallback
        }
        else if (Camera.main != null)
        {
            horizontalDir = Camera.main.transform.forward;
            horizontalDir.y = 0f;
            horizontalDir.Normalize();
        }
        else
        {
            horizontalDir = Vector3.forward;
        }

        spawnPos = transform.position + horizontalDir * 0.5f + Vector3.up * 0.5f;

        Vector3 dir = Quaternion.Euler(playerMouseLook.transform.eulerAngles.x, playerMouseLook.transform.eulerAngles.y, 0f) * Vector3.forward;
        Vector3 initialVelocity = dir * granadeSpeed;

        GameObject g = Instantiate(grenadePrefab, spawnPos, Quaternion.identity);
        g.GetComponent<Granade>().maxDamage = damage;
        Rigidbody rb = g.GetComponent<Rigidbody>();
        if (rb == null)
        {

            return;
        }

        rb.velocity = initialVelocity;
        rb.useGravity = true;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

       
    }

    public void EquipGun()
    {
        player.SwitchToWeapon();
    }

}
