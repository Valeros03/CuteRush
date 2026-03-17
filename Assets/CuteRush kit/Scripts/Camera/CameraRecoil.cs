using UnityEngine;

public class CameraRecoil : MonoBehaviour
{
    private float currentRecoil = 0f;
    private float targetRecoil = 0f;
    private float recoverySpeed = 0f;

    private GunBase gun;
    private RecoilController recoilController;

    void Start()
    {
        // Se c'è già un'arma all'avvio, la inizializziamo
        GunBase initialGun = GetComponentInChildren<GunBase>();
        RecoilController initialRecoil = GetComponentInChildren<RecoilController>();

        if (initialGun != null && initialRecoil != null)
        {
            SetNewWeapon(initialGun, initialRecoil);
        }
    }

    // NUOVO METODO: Chiameremo questo quando il player cambia arma
    public void SetNewWeapon(GunBase newGun, RecoilController newRecoil)
    {
        // 1. DISISCRIVIAMOCI dalla vecchia arma (importantissimo per non creare errori e memory leak!)
        if (gun != null)
        {
            gun.onBulletShot -= ApplyRecoil;
        }

        // 2. Aggiorniamo i riferimenti con la nuova arma
        gun = newGun;
        recoilController = newRecoil;

        // 3. ISCRIVIAMOCI alla nuova arma
        if (gun != null)
        {
            gun.onBulletShot += ApplyRecoil;
        }

        // 4. Ricalcoliamo la velocità di recupero in base al profilo della nuova arma
        if (recoilController != null && recoilController.profile != null)
        {
            recoverySpeed = recoilController.profile.rotationAmplitudeVertical / recoilController.profile.recoveryDuration * 0.8f;
        }
    }

    void Update()
    {
        // interpolazione dolce verso il target
        currentRecoil = Mathf.Lerp(currentRecoil, targetRecoil, Time.deltaTime * recoverySpeed);

        // applica il pitch verticale
        transform.localRotation = Quaternion.Euler(-currentRecoil, 0f, 0f);

        // graduale ritorno a 0
        targetRecoil = Mathf.Lerp(targetRecoil, 0f, Time.deltaTime * (recoverySpeed * 0.5f));
    }

    private void ApplyRecoil()
    {
        if (recoilController == null || recoilController.profile == null) return;

        // ad ogni colpo aggiungi un po’ di kick verticale
        targetRecoil += recoilController.profile.rotationAmplitudeVertical;
        targetRecoil = Mathf.Clamp(targetRecoil, 0f, recoilController.profile.maxRotationOffsetVertical); // limite cumulativo
    }
}