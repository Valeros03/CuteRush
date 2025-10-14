using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GrandeThrower : MonoBehaviour
{
    [SerializeField] PlayerController player;
    [SerializeField] float granadeSpeed;
    [SerializeField] float gravity;
    [SerializeField] int GranadeCount;
    public GameObject grenadePrefab;

    public LayerMask collisionMask; // layer di terreno + ostacoli
    public int trajectorySteps = 50; // precisione del campionamento

    private MouseLook playerMouseLook;
    private Transform arrivingPoint;

    //UI Settings
    private GameObject GranadeUI;
    private Text bulletNumberUI;

    void Start()
    {
        playerMouseLook = player.GetComponentInChildren<MouseLook>();
        arrivingPoint = transform.Find("ArrivingPoint");

        if (GranadeUI == null)
        {
            GranadeUI = GameObject.Find("Canvas/WeaponUI");
        }

        if (bulletNumberUI == null && GranadeUI != null)
        {
            bulletNumberUI = GranadeUI.transform.Find("Granade").GetComponent<Text>();
        }

        bulletNumberUI.text = string.Format("Granate {0}", GranadeCount);
    }

    void Update()
    {
        UpdateArrivingPoint();
    }

    void UpdateArrivingPoint()
    {
        Vector3 startPos = transform.position;

        Vector3 dir = Quaternion.Euler(playerMouseLook.transform.eulerAngles.x, playerMouseLook.transform.eulerAngles.y, 0f) * Vector3.forward;
        Vector3 velocity = dir * granadeSpeed;

        // Calcolo del punto di arrivo
        Vector3 hitPoint = SimulateTrajectory(startPos, velocity, gravity, trajectorySteps, collisionMask);

        // Aggiorna il marker
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
        float timeStep = 0.1f; // quanto “avanza” il tempo ogni passo

        for (int i = 0; i < steps; i++)
        {
            Vector3 nextPos = pos + vel * timeStep;
            vel += Vector3.down * gravity * timeStep;

            // Controllo collisione tra pos e nextPos
            if (Physics.Linecast(pos, nextPos, out RaycastHit hit, mask))
            {
                // 💥 Collisione trovata prima di arrivare a terra
                return hit.point;
            }

            // Controllo se attraversa il piano y = 0 (terra)
            if (pos.z > 0 && nextPos.z <= 0)
            {
                // Calcolo preciso di dove taglia il piano
                float t = pos.z / (pos.z - nextPos.z);
                Vector3 groundHit = Vector3.Lerp(pos, nextPos, t);
                return groundHit;
            }

            pos = nextPos;
        }

        // Se non trova nulla, restituisci ultima posizione (fuori range)
        return pos;
    }

    public int GetGranadeCount()
    {
        return GranadeCount;
    }

    public void Activation()
    {

        arrivingPoint.gameObject.SetActive(true);
        transform.Find("Granade").gameObject.SetActive(false);
    }

    public void ThrowGrenade()
    {

        arrivingPoint.gameObject.SetActive(false);
        GranadeCount--;

        Vector3 spawnPos;
        if (transform.position != null)
            spawnPos = transform.position;
        else if (arrivingPoint != null)
            spawnPos = arrivingPoint.position + Vector3.up * 0.2f; // piccolo offset sopra il terreno
        else
            spawnPos = transform.position + transform.forward * 0.5f;

        // Calcola la direzione orizzontale verso il punto di arrivo (o dalla camera)
        Vector3 horizontalDir;
        if (arrivingPoint != null)
        {
            // Delta tra punto di arrivo e spawn
            Vector3 delta = arrivingPoint.position - spawnPos;

            // Proietta sul piano orizzontale (y = 0)
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

        // componi la velocità iniziale dato l'angolo di lancio
        float angleDeg = playerMouseLook.transform.eulerAngles.x;
        if (angleDeg > 180f) angleDeg -= 360f; // converti in [-180, 180]
        float angleRad = angleDeg * Mathf.Deg2Rad;
        float vy = granadeSpeed * Mathf.Sin(angleRad);            // componente verticale
        float vxz = granadeSpeed * Mathf.Cos(angleRad);           // componente nella direzione orizzontale
        Vector3 initialVelocity = horizontalDir * vxz + Vector3.down * vy;

        // Instanzia la granata
        GameObject g = Instantiate(grenadePrefab, spawnPos, Quaternion.identity);
        Rigidbody rb = g.GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Grenade prefab needs a Rigidbody component!");
            return;
        }

        // Impostazioni fisiche utili
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
