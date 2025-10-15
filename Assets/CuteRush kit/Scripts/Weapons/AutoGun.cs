using System.Collections;
using UnityEngine;

public class AutoGun : GunBase
{
    protected override void Shoot()
    {
        if (IsInShotCooldown) return;

        // avvia il cooldown
        StartCoroutine(FireRoutine());
    }

    private IEnumerator FireRoutine()
    {
        IsInShotCooldown = true;

        // opzionale: ritardo prima dello sparo (es. animazione o spread)
        if (stats.shootDelay > 0f)
            yield return new WaitForSeconds(stats.shootDelay);

        SpawnBulletVisualsAndRaycast();

        // attesa del rateo di fuoco
        yield return new WaitForSeconds(1/stats.fireRate);

        IsInShotCooldown = false;
    }
}
