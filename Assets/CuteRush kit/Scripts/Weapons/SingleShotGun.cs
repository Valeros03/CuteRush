using System.Collections;
using UnityEngine;

public class SingleShotGun : GunBase
{
    // Se vuoi, puoi personalizzare il cooldown (quanto tempo deve passare prima di poter sparare di nuovo)
    [SerializeField] private float shotCooldown = 0.2f;

    protected override void Shoot()
    {
        // sparo solo se pronta
        if (IsInShotCooldown) return;

        // avvia procedura di sparo singolo
        StartCoroutine(SingleShotRoutine());
    }

    private IEnumerator SingleShotRoutine()
    {
        IsInShotCooldown = true;

        // opzionale: attesa prima dello sparo (shootDelay)
        if (stats.shootDelay > 0f)
            yield return new WaitForSeconds(stats.shootDelay);

        SpawnBulletVisualsAndRaycast();

        // cooldown tra un colpo e l'altro
        yield return new WaitForSeconds(shotCooldown);

        // cooldown finito, ma l’arma resta “bloccata” finché il giocatore non rilascia e ripreme
        // quindi non resettiamo subito IsInShotCooldown qui,
        // lo farà manualmente PlayerController quando rilascia il tasto.
    }

    public override void ResetSingleShot()
    {
        // qui resettiamo il cooldown solo quando il giocatore rilascia il tasto
        IsInShotCooldown = false;
    }
}
