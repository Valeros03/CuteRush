using System.Collections;
using UnityEngine;

public class ChargeGun : GunBase
{
    private Coroutine chargeCoroutine;

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
}
