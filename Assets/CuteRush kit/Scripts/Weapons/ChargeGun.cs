using System.Collections;
using System.Linq;
using UnityEngine;

public class ChargeGun : GunBase
{
    private Coroutine chargeCoroutine;
    [Header("Charge Gun Settings")]
    [SerializeField] private float sphereRadius = 0.3f;
    [SerializeField] private float penetrationFalloff = 0.75f;
    [SerializeField] private int maxPenetrations = 5;

    protected override void Shoot()
    {
        if (chargeCoroutine == null)
            chargeCoroutine = StartCoroutine(ChargeRoutine());
    }

    private IEnumerator ChargeRoutine()
    {
        IsInShotCooldown = true;
        audioController?.PlayCharge();
        float elapsed = 0f;

        while (elapsed < stats.shootDelay)
        {
            if (!gameObject.activeSelf || isReloading)
            {
                IsInShotCooldown = false;
                chargeCoroutine = null;
                yield break;
            }
            elapsed += Time.deltaTime;
            yield return null;
        }

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
            sphereRadius,
            direction,
            stats.range,
            hitLayers
        );

        hits = hits.OrderBy(h => h.distance).ToArray();

        Vector3 tracerEnd = hits.Length > 0
            ? hits[Mathf.Min(hits.Length - 1, maxPenetrations - 1)].point
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
                currentDamage *= penetrationFalloff;
                enemiesHit++;

                if (enemiesHit >= maxPenetrations)
                    break;
            }
            else
            {
                break;
            }
        }

        currentBulletCount--;
        onBulletShot?.Invoke();
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

        return true;
    }
}
