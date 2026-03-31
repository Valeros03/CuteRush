using System.Collections;
using UnityEngine;

public class AutoGun : GunBase
{
    protected override void Shoot()
    {
        if (IsInShotCooldown) return;

        StartCoroutine(FireRoutine());
    }

    private IEnumerator FireRoutine()
    {
        IsInShotCooldown = true;

        if (stats.shootDelay > 0f)
            yield return new WaitForSeconds(stats.shootDelay);

        SpawnBulletVisualsAndRaycast();

        yield return new WaitForSeconds(1/stats.fireRate);

        IsInShotCooldown = false;
    }
}
