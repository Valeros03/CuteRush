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
            // trova il gun nel parent
            gun = GetComponentInChildren<GunBase>();
            recoilController = GetComponentInChildren<RecoilController>();
            if (gun != null)
                gun.onBulletShot += ApplyRecoil;

            recoverySpeed = recoilController.profile.rotationAmplitudeVertical / recoilController.profile.recoveryDuration * 0.8f;
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
            // ad ogni colpo aggiungi un po’ di kick verticale
            targetRecoil += recoilController.profile.rotationAmplitudeVertical;
            targetRecoil = Mathf.Clamp(targetRecoil, 0f, recoilController.profile.maxRotationOffsetVertical); // limite cumulativo
        }
}

