using System;
using UnityEngine;
using System.Collections;

public class VitalsController : MonoBehaviour
{
    [Header("[Health Settings]")]
    public int maxHealth;
    public int currentHealth;
    public int medKitHeal = 50;

    public static event Action<int, int> OnHealthChange;

    private AudioPlayerController audioController;
    private Vector3 lastHitDirection;

    public void Start()
    {
        currentHealth = maxHealth;
        audioController = GetComponent<AudioPlayerController>();
        OnHealthChange?.Invoke(currentHealth, maxHealth);
    }

    public void UseMedikit()
    {
        Increase(medKitHeal);
        audioController.PlayHealSound();
    }

    private void Increase(int value)
    {
        currentHealth += value;
        if (currentHealth > maxHealth) currentHealth = maxHealth;

        OnHealthChange?.Invoke(currentHealth, maxHealth);
    }

    public void Decrease(int value, Vector3 damageSourcePosition, bool isPhysical = false)
    {
        currentHealth -= value;
        lastHitDirection = damageSourcePosition;
        if (currentHealth <= 0)
        {
            Die();
            currentHealth = 0;
        }
        OnHealthChange?.Invoke(currentHealth, maxHealth);

        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowDamageIndicator(damageSourcePosition);
        }

        if (audioController != null)
        {
            audioController.PlayDamageSound(isPhysical);
        }
    }

    private void Die()
    {

        if (TryGetComponent(out PlayerInput input)) input.enabled = false;
        if (TryGetComponent(out PlayerMovement mov)) mov.enabled = false;
        if (TryGetComponent(out PlayerCombat combat)) combat.enabled = false;

        AudioPlayerController audioPlayer = GetComponent<AudioPlayerController>();
        if (audioPlayer != null) audioPlayer.DeathSound();

        MouseLook mouseLook = GetComponentInChildren<MouseLook>();
        if (mouseLook != null) mouseLook.enabled = false;

        CameraRecoil recoil = GetComponentInChildren<CameraRecoil>();
        if (recoil != null) recoil.enabled = false;

        Camera fpsCam = GetComponentInChildren<Crosshair>().gameObject.GetComponent<Camera>();
        if (fpsCam != null)
        {
            MouseLook scriptLook = fpsCam.GetComponentInParent<MouseLook>();
            scriptLook.enabled = false;
            CameraRecoil scriptRecoil = fpsCam.GetComponent<CameraRecoil>();
            scriptRecoil.enabled = false;

        }

        if (TryGetComponent(out Animator animator)) animator.enabled = false;


        StartCoroutine(DeathCameraAnimation(lastHitDirection));
        GameManager.Instance.GameOver();
    }

    private IEnumerator DeathCameraAnimation(Vector3 hitDirection)
    {

        Camera fpsCam = GetComponentInChildren<Crosshair>().gameObject.GetComponent<Camera>();
        if (fpsCam == null) yield break;

        Transform camTransform = fpsCam.transform;
        Vector3 startLocalPos = camTransform.localPosition;
        Quaternion startRot = camTransform.localRotation;

        Vector3 localHitDir = transform.InverseTransformDirection(hitDirection);
        float rollSideAngle = (localHitDir.x < 0) ? 70f : -70f;
        Quaternion endRot = Quaternion.Euler(startRot.eulerAngles.x, startRot.eulerAngles.y, rollSideAngle);

        Vector3 targetWorldPos = camTransform.position;
        if (Physics.Raycast(camTransform.position, Vector3.down, out RaycastHit hit, 2f))
        {
            targetWorldPos.y = hit.point.y + 0.15f;
        }
        else
        {
            targetWorldPos.y -= 0.8f;
        }

        Vector3 endLocalPos = camTransform.parent != null
            ? camTransform.parent.InverseTransformPoint(targetWorldPos)
            : targetWorldPos;

        float duration = 1.0f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / duration;

            camTransform.localPosition = Vector3.Lerp(startLocalPos, endLocalPos, t);
            camTransform.localRotation = Quaternion.Slerp(startRot, endRot, t);

            yield return null;
        }
    }
}