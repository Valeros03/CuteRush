using System.Collections;
using System.Linq;
using UnityEngine;

public class ChargeGun : GunBase
{
    [Header("Base Timings (For Scaling)")]
    [Tooltip("La durata ORIGINALE del tempo di carica (shootDelay) prima degli upgrade. Serve per scalare l'audio.")]
    public float baseChargeDuration = 1.0f;

    private Coroutine chargeCoroutine;

    protected override void Shoot()
    {
        if (chargeCoroutine == null)
            chargeCoroutine = StartCoroutine(ChargeRoutine());
    }

    private IEnumerator ChargeRoutine()
    {
        IsInShotCooldown = true;

        float currentDelay = Mathf.Max(0.001f, stats.shootDelay);
        float speedMultiplier = baseChargeDuration / currentDelay;

        AudioSource audioSource = null;
        float originalPitch = 1f;

        if (audioController != null)
        {
            audioSource = audioController.GetComponent<AudioSource>();
            if (audioSource != null)
            {
                originalPitch = audioSource.pitch;
                audioSource.pitch = speedMultiplier;
            }

            audioController.PlayCharge();
        }

        float elapsed = 0f;

        while (elapsed < stats.shootDelay)
        {
            if (!gameObject.activeSelf || isReloading)
            {
                if (audioSource != null) audioSource.pitch = originalPitch;

                IsInShotCooldown = false;
                chargeCoroutine = null;
                yield break;
            }
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (audioSource != null) audioSource.pitch = originalPitch;

        if (gameObject.activeInHierarchy)
            SpawnBulletVisualsAndRaycast();

        float cooldown = 1f / stats.fireRate - stats.shootDelay;
        if (cooldown > 0f) yield return new WaitForSeconds(cooldown);

        IsInShotCooldown = false;
        chargeCoroutine = null;
    }

    private void OnDisable()
    {
        if (chargeCoroutine != null)
        {
            StopCoroutine(chargeCoroutine);
            chargeCoroutine = null;
            IsInShotCooldown = false;

            if (audioController != null)
            {
                AudioSource src = audioController.GetComponent<AudioSource>();
                if (src != null) src.pitch = 1f;
            }
        }
    }

    protected override void SpawnBulletVisualsAndRaycast()
    {
        if (mainCamera == null) mainCamera = Camera.main;
        if (mainCamera == null) return;

        audioController?.PlayShoot();

        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        Vector3 direction = ray.direction;

        RaycastHit[] hits = Physics.SphereCastAll(
            firePoint.position,
            stats.sphereRadius,
            direction,
            stats.range,
            hitLayers
        );

        hits = hits.OrderBy(h => h.distance).ToArray();

        Vector3 tracerEnd = hits.Length > 0
            ? hits[Mathf.Min(hits.Length - 1, stats.maxPenetrations - 1)].point
            : firePoint.position + direction * stats.range;

        DrawTracer(firePoint.position, tracerEnd);

        float currentDamage = stats.damage;
        int enemiesHit = 0;

        foreach (var hit in hits)
        {
            Enemy enemy = hit.collider.GetComponentInParent<Enemy>();

            if (enemy != null)
            {
                Vector3 shotDir = (hit.point - firePoint.position).normalized;
                enemy.TakeDamage(currentDamage, shotDir, hit.point);
                currentDamage *= stats.penetrationFalloff;
                enemiesHit++;

                if (enemiesHit >= stats.maxPenetrations)
                    break;
            }
            else
            {
                break;
            }
        }

        currentBulletCount--;
        onBulletShot?.Invoke();
        HandleOnAmmoChange();
    }

    public override bool addMag()
    {
        if (currentMagLeft >= stats.totalAmmo)
        {
            return false;
        }
        currentMagLeft += (stats.magazineSize * 2);

        if (currentMagLeft > stats.totalAmmo)
        {
            currentMagLeft = stats.totalAmmo;
        }

        HandleOnAmmoChange();

        return true;
    }
}