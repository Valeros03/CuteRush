using System.Collections;
using System.Linq;
using UnityEngine;

public class ChargeGun : GunBase
{
    private Coroutine chargeCoroutine;
    [Header("Charge Gun Settings")]
    [SerializeField] private float sphereRadius = 0.3f;      // spessore del raggio
    [SerializeField] private float penetrationFalloff = 0.75f; // moltiplicatore di danno per ogni nemico successivo
    [SerializeField] private int maxPenetrations = 5;         // quanti nemici può attraversare

    protected override void Shoot()
    {
        // se già in caricamento, ignora o potresti voler rilasciare il tiro
        if (chargeCoroutine == null)
            chargeCoroutine = StartCoroutine(ChargeRoutine());
    }

    private IEnumerator ChargeRoutine()
    {
        IsInShotCooldown = true;
        audioController?.PlayCharge();
        float elapsed = 0f;

        // attesa della charge; puoi estendere per rilasciare quando il giocatore rilascia il tasto
        while (elapsed < stats.shootDelay)
        {
            if (!gameObject.activeSelf || isReloading) // abort
            {
                IsInShotCooldown = false;
                chargeCoroutine = null;
                yield break;
            }
            elapsed += Time.deltaTime;
            yield return null;
        }

        // check che l'arma sia ancora quella attiva (es. switch)
        if (gameObject.activeInHierarchy)
            SpawnBulletVisualsAndRaycast();

        // cooldown dopo il colpo
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

        // Direzione di base del colpo
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        Vector3 direction = ray.direction;

        // Lancia lo SphereCastAll per trovare tutti i colpi lungo la linea
        RaycastHit[] hits = Physics.SphereCastAll(
            firePoint.position,
            sphereRadius,
            direction,
            stats.range,
            hitLayers
        );

        // Ordina in base alla distanza, come per un raggio fisico
        hits = hits.OrderBy(h => h.distance).ToArray();

        // Traccia un raggio visivo per il primo impatto o per l’intera portata
        Vector3 tracerEnd = hits.Length > 0
            ? hits[Mathf.Min(hits.Length - 1, maxPenetrations - 1)].point
            : firePoint.position + direction * stats.range;

        DrawTracer(firePoint.position, tracerEnd);

        // Danno progressivo
        float currentDamage = stats.damage;
        int enemiesHit = 0;

        foreach (var hit in hits)
        {
            Enemy enemy = hit.collider.GetComponentInParent<Enemy>();

            if (enemy != null)
            {
                Vector3 shotDir = (hit.point - firePoint.position).normalized;
                enemy.TakeDamage(currentDamage, shotDir, hit.point);
                currentDamage *= penetrationFalloff; // riduce il danno ad ogni nemico colpito
                enemiesHit++;

                if (enemiesHit >= maxPenetrations)
                    break;
            }
            else
            {
                // muro o ostacolo → ferma il colpo
                break;
            }
        }

        // Aggiornamento ammo e callback
        currentBulletCount--;
        onBulletShot?.Invoke();
    }

    public override void addMag()
    {
        if (currentMagLeft * stats.magazineSize >= stats.totalAmmo) return;
        currentMagLeft += 2;
    }
}
