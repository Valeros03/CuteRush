using System.Collections;
using UnityEngine;

public class SingleShotGun : GunBase
{
    [SerializeField] private float shotCooldown = 0.2f;

    protected override void Shoot()
    {
        if (IsInShotCooldown) return;

        StartCoroutine(SingleShotRoutine());
    }

    private IEnumerator SingleShotRoutine()
    {
        IsInShotCooldown = true;

        if (stats.shootDelay > 0f)
            yield return new WaitForSeconds(stats.shootDelay);

        SpawnBulletVisualsAndRaycast();

        yield return new WaitForSeconds(shotCooldown);

    }

    public override void ResetSingleShot()
    {
        IsInShotCooldown = false;
    }
}
