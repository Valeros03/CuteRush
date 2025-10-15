using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioGunController))]
[RequireComponent(typeof(Animator))]
public abstract class GunBase : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The particle systems for the gun, if any")]
    public ParticleSystem[] gunParticleSystems;

    [Header("Settings")]
    public Transform firePoint;          // punto da cui parte il raycast
    public LayerMask hitLayers;          // layer colpibili (es. Enemy)

    [Header("Effects")]
    public LineRenderer tracer;
    public float tracerDuration = 0.05f;
    private float tracerTimer;

    [Header("UI")]
    private GameObject weaponUI;
    private Text bulletNumberUIText;

    protected Animator animator;
    protected AudioGunController audioController;
    protected Camera mainCamera;

    [Header("Stats")]
    public GunStats stats;

    [HideInInspector] public int currentBulletCount;
    protected int currentMagLeft;

    [HideInInspector] public bool isReloading = false;
    public bool IsInShotCooldown { get; protected set; } = false;

    // Events
    public Action onBulletShot;
    public Action onGunReloadStart;
    public Action onGunShootingStart;


    private PlayerController player;
    protected virtual void Awake()
    {
        audioController = GetComponent<AudioGunController>();
        animator = GetComponent<Animator>();
        mainCamera = Camera.main;
        player = GetComponentInParent<PlayerController>();
    }

    protected virtual void Start()
    {
        if (stats == null)
        {
            Debug.LogError($"{name}: stats non assegnate!");
            return;
        }

        currentBulletCount = stats.magazineSize;
        currentMagLeft = stats.totalAmmo;

        // fallback UI find (solo se non impostato dall'inspector)
        if (weaponUI == null)
        {
            weaponUI = GameObject.Find("Canvas/WeaponUI");
        }

        if (bulletNumberUIText == null && weaponUI != null)
        {
            bulletNumberUIText = weaponUI.transform.Find("Ammo").GetComponent<Text>();
        }

    }

    protected virtual void Update()
    {
        if (weaponUI != null && weaponUI.activeSelf && bulletNumberUIText != null)
        {
            bulletNumberUIText.text = $"Bullets {currentBulletCount}/{currentMagLeft}";
        }

        if (tracer != null && tracer.enabled)
        {
            tracerTimer -= Time.deltaTime;
            if (tracerTimer <= 0f)
                tracer.enabled = false;
        }
    }

    // PUBLIC API: chiamato dall'input / player
    public void TryShoot()
    {
        if (!gameObject.activeSelf || !enabled) return;
        if (currentBulletCount <= 0)
        {
            StartReload();
            return;
        }

        if (!isReloading && !IsInShotCooldown)
        {
            onGunShootingStart?.Invoke();

            // effetti visuali
            foreach (var ps in gunParticleSystems) if (ps != null) ps.Play();

            // delega al comportamento concreto
            Shoot();
        }
    }

    // Classe concreta implementa questa funzione
    protected abstract void Shoot();

    protected void SpawnBulletVisualsAndRaycast()
    {
        // logica consolidata di raycast, tracers, danno, suoni, decremento ammo, eventi
        if (mainCamera == null) mainCamera = Camera.main;
        if (mainCamera == null) return;

        RaycastHit hit;
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        Vector3 targetPoint;
        audioController?.PlayShoot();

        if (Physics.Raycast(ray, out hit, stats.range, hitLayers))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(stats.range);
        }

        // direzione con un piccolo spread
        Vector3 direction = (targetPoint - firePoint.position).normalized;

        if (Physics.Raycast(firePoint.position, direction, out hit, stats.range, hitLayers))
        {
            var enemy = hit.collider.GetComponentInParent<Enemy>();
            Vector3 shotDir = (hit.point - firePoint.position).normalized;
            if (enemy != null)
            {
                enemy.TakeDamage(stats.damage, shotDir, hit.point);
            }

            DrawTracer(firePoint.position, hit.point);
        }
        else
        {
            DrawTracer(firePoint.position, targetPoint);
        }

        currentBulletCount--;
        onBulletShot?.Invoke();
    }

    private void DrawTracer(Vector3 start, Vector3 end)
    {
        if (tracer == null) return;
        tracer.enabled = true;
        Vector3 offset = Vector3.zero;
        if ((end - start).magnitude < 5f && mainCamera != null)
            offset = (mainCamera.transform.position - start).normalized * 0.1f;
        tracer.SetPosition(0, start + offset);
        tracer.SetPosition(1, end);
        tracerTimer = tracerDuration;
    }

    // RELOAD
    public void StartReload()
    {
        if (!gameObject.activeSelf || isReloading) return;
        if (currentBulletCount >= stats.magazineSize) return;
        StartCoroutine(ReloadCoroutine());
    }

    protected virtual IEnumerator ReloadCoroutine()
    {
        onGunReloadStart?.Invoke();
        isReloading = true;
        if (animator != null) animator.SetTrigger("Recharge");

        float audioDelay = audioController != null ? audioController.Recharge.length : 0f;
        if (stats.reloadDuration > audioDelay)
        {
            yield return new WaitForSeconds(stats.reloadDuration - audioDelay);
            audioController?.PlayRecharge();
            yield return new WaitForSeconds(audioDelay);
        }
        else
        {
            // se reloadDuration <= audioDelay, suona comunque e aspetta reloadDuration
            audioController?.PlayRecharge();
            yield return new WaitForSeconds(stats.reloadDuration);
        }

        // refill
        if (currentMagLeft > 0)
        {
            int needed = stats.magazineSize - currentBulletCount;
            int toLoad = Mathf.Min(needed, currentMagLeft);
            currentBulletCount += toLoad;
            currentMagLeft -= toLoad;
        }

        isReloading = false;
    }

    // helper coroutine per cooldown
    protected IEnumerator ShotCooldownCoroutine(float cooldown)
    {
        IsInShotCooldown = true;
        yield return new WaitForSeconds(cooldown);
        IsInShotCooldown = false;
    }

    // metodo per resettare manualmente single-shot (es. input release)
    public virtual void ResetSingleShot() { IsInShotCooldown = false; }

    private void EquipGranade()
    {
        player.SwitchToGranade();
    }
}
