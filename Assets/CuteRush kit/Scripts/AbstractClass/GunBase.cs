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
    public Transform firePoint;        
    public LayerMask hitLayers;          

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

    public Action onBulletShot;
    public Action onGunReloadStart;
    public Action onGunShootingStart;


    private PlayerController player;
    private Crosshair crosshair; 

    protected virtual void Awake()
    {
        audioController = GetComponent<AudioGunController>();
        animator = GetComponent<Animator>();
        mainCamera = Camera.main;
        player = GetComponentInParent<PlayerController>();

        crosshair = FindObjectOfType<Crosshair>();
    }

    protected virtual void Start()
    {
        if (stats == null)
        {
            return;
        }

        currentBulletCount = stats.magazineSize;
        currentMagLeft = stats.totalAmmo;


        if (crosshair != null)
        {
            crosshair.LoadGunSettings(stats);
        }

        if (weaponUI == null)
        {
            weaponUI = GameObject.Find("Canvas/HUD/WeaponUI");
        }

        if (bulletNumberUIText == null && weaponUI != null)
        {
            bulletNumberUIText = weaponUI.transform.Find("Ammo").GetComponent<Text>();
        }

    }

    protected virtual void Update()
    {
        if (player != null)
        {
            if (weaponUI != null && weaponUI.activeSelf && bulletNumberUIText != null)
            {
                bulletNumberUIText.text = $"Bullets {currentBulletCount}/{currentMagLeft}";
            }
        }
        if (tracer != null && tracer.enabled)
        {
            tracerTimer -= Time.deltaTime;
            if (tracerTimer <= 0f)
                tracer.enabled = false;
        }
    }

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

            foreach (var ps in gunParticleSystems) if (ps != null) ps.Play();

            Shoot();
        }
    }

    protected abstract void Shoot();

    protected virtual void SpawnBulletVisualsAndRaycast()
    {
        if (mainCamera == null) mainCamera = Camera.main;
        if (mainCamera == null) return;

        audioController?.PlayShoot();

        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        Crosshair crosshair = FindObjectOfType<Crosshair>();
        if (crosshair != null)
        {
            float bloomFactor = crosshair.spread / 2000f;
            Vector3 randomOffset = mainCamera.transform.right * UnityEngine.Random.Range(-bloomFactor, bloomFactor) +
                                   mainCamera.transform.up * UnityEngine.Random.Range(-bloomFactor, bloomFactor);
            ray.direction = (ray.direction + randomOffset).normalized;
        }

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, stats.range, hitLayers))
        {
            var enemy = hit.collider.GetComponentInParent<Enemy>();

            if (enemy != null)
            {
                enemy.TakeDamage(stats.damage, ray.direction, hit.point);
            }

         
            DrawTracer(firePoint.position, hit.point);
        }
        else
        {
            Vector3 targetPoint = ray.GetPoint(stats.range);
            DrawTracer(firePoint.position, targetPoint);
        }

        currentBulletCount--;
        onBulletShot?.Invoke();

        if (crosshair != null)
        {
            crosshair.ApplyShootKick();
        }
    }

    protected void DrawTracer(Vector3 start, Vector3 end)
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

            audioController?.PlayRecharge();
            yield return new WaitForSeconds(stats.reloadDuration);
        }

        if (currentMagLeft > 0)
        {
            int needed = stats.magazineSize - currentBulletCount;
            int toLoad = Mathf.Min(needed, currentMagLeft);
            currentBulletCount += toLoad;
            currentMagLeft -= toLoad;
        }

        isReloading = false;
    }

    protected IEnumerator ShotCooldownCoroutine(float cooldown)
    {
        IsInShotCooldown = true;
        yield return new WaitForSeconds(cooldown);
        IsInShotCooldown = false;
    }

    public virtual void ResetSingleShot() { IsInShotCooldown = false; }

    private void EquipGranade()
    {
        player.SwitchToGranade();
    }

    public virtual bool addMag()
    {
        if (currentMagLeft >= stats.totalAmmo)
        {
            return false;
        }

        currentMagLeft += stats.magazineSize;

        if (currentMagLeft > stats.totalAmmo)
        {
            currentMagLeft = stats.totalAmmo;
        }

        return true;
    }
}
