using UnityEngine;


public class RecoilController : MonoBehaviour
    {
        public enum Handedness { Right, Left }

        [Tooltip("Affects the lateral direction of the recoil jolt")]
        public Handedness handedness = Handedness.Right;

        [Tooltip("Recoil configuration profile")]
        public RecoilProfile profile;

        [Tooltip("If true, recoil offsets stack per shot. Otherwise, each shot resets recoil.")]
        public bool stackRecoil = false;

        private Vector3 targetPositionOffset = Vector3.zero;
        private Quaternion targetRotationOffset = Quaternion.identity;

        private Vector3 currentPositionOffset = Vector3.zero;
        private Quaternion currentRotationOffset = Quaternion.identity;

        private float recoilTimer = 1f;
        private float recoveryTimer = 1f;

        private bool isRecoiling = false;

        private Vector3 originalPosition;
        private Quaternion originalRotation;

        private GunBase gun;

        private void Start()
        {
            originalPosition = transform.localPosition;
            originalRotation = transform.localRotation;

            gun = transform.parent.GetComponent<GunBase>();
            if (gun != null)
                gun.onBulletShot += StartRecoil;
        }

        private void Update()
        {
            if (isRecoiling)
            {
                recoilTimer += Time.deltaTime / profile.recoilDuration;
                float t = Mathf.Clamp01(recoilTimer);
                float curveValue = profile.recoilCurve.Evaluate(t);

                currentPositionOffset = Vector3.Lerp(Vector3.zero, targetPositionOffset, curveValue);
                currentRotationOffset = Quaternion.Slerp(Quaternion.identity, targetRotationOffset, curveValue);

                if (t >= 1f)
                {
                    isRecoiling = false;
                    recoveryTimer = 0f;
                }
            }
            else
            {
                recoveryTimer += Time.deltaTime / profile.recoveryDuration;
                float t = Mathf.Clamp01(recoveryTimer);
                float curveValue = profile.recoveryCurve.Evaluate(t);

                currentPositionOffset = Vector3.Lerp(targetPositionOffset, Vector3.zero, curveValue);
                currentRotationOffset = Quaternion.Slerp(targetRotationOffset, Quaternion.identity, curveValue);
            }

            ApplyTransform();
        }

        private void ApplyTransform()
        {
            transform.localPosition = originalPosition + currentPositionOffset;
            transform.localRotation = originalRotation * currentRotationOffset;
        }

        private void StartRecoil()
        {
            float sideJoltDirection = handedness == Handedness.Left ? -1f : 1f;

            // Position offset (indietro)
            Vector3 newPosOffset = Vector3.forward * profile.movementAmplitude;

            // Rotation offset
            Quaternion newRotOffset = Quaternion.Euler(
                profile.rotationAmplitudeVertical, // vertical recoil (pitch)
                Random.Range(0.25f, 0.5f) * profile.rotationAmplitudeHorizontal * sideJoltDirection, // yaw
                Random.Range(-0.2f, 0.2f) * profile.rotationAmplitudeHorizontal * sideJoltDirection   // roll
            );

            // Apply or stack recoil
            if (stackRecoil)
            {
                targetPositionOffset += newPosOffset;
                targetRotationOffset = newRotOffset * targetRotationOffset;
            }
            else
            {
                targetPositionOffset = newPosOffset;
                targetRotationOffset = newRotOffset;
            }

            // Clamp limits
            targetPositionOffset = Vector3.ClampMagnitude(targetPositionOffset, profile.maxMovementOffset);

            Vector3 euler = targetRotationOffset.eulerAngles;

            // prendi solo una frazione piccola per la rotazione verticale dell'arma
            float weaponVerticalFraction = 0.05f; // 5% del vero rinculo verticale
            euler.x = Mathf.Clamp(ClampAngle(euler.x) * weaponVerticalFraction, -profile.maxRotationOffsetVertical, profile.maxRotationOffsetVertical);

            // la rotazione laterale (y e z) rimane invariata
            euler.y = Mathf.Clamp(ClampAngle(euler.y), -profile.maxRotationOffsetHorizontal, profile.maxRotationOffsetHorizontal);
            euler.z = Mathf.Clamp(ClampAngle(euler.z), -profile.maxRotationOffsetHorizontal, profile.maxRotationOffsetHorizontal);

            targetRotationOffset = Quaternion.Euler(euler);

            recoilTimer = 0f;
            isRecoiling = true;
        }

        private float ClampAngle(float angle)
        {
            if (angle > 180f)
                angle -= 360f;
            return angle;
        }
}
