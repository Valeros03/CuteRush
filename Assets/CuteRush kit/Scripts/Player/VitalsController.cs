using System;
using UnityEngine;
using System.Collections;

public class VitalsController : MonoBehaviour
{
    [Header("[Health Settings]")]
    public int maxHealth;
    public int currentHealth;
    public int medKitHeal = 50;

    public delegate void HealthChanger(int currentValue, int maxValue);
    public delegate void DamageTaker(Vector3 enemyPos);

    public event DamageTaker OnTakeDamage;
    public event HealthChanger OnHealthChange;

    private AudioPlayerController audioController;
    private Vector3 lastHitDirection;

    public void Init()
    {
        currentHealth = maxHealth;
        audioController = GetComponent<AudioPlayerController>();
        OnHealthChange?.Invoke(currentHealth, maxHealth);
    }

    public void UseMedikit()
    {
        Increase(medKitHeal);
        if (audioController != null) audioController.PlayHealSound();
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
            currentHealth = 0;
            Die();
        }

        OnHealthChange?.Invoke(currentHealth, maxHealth);
        OnTakeDamage?.Invoke(damageSourcePosition);

        if (audioController != null) audioController.PlayDamageSound(isPhysical);
    }

    private void Die()
    {
        if (TryGetComponent(out PlayerInput input)) input.enabled = false;
        if (TryGetComponent(out PlayerMovement mov)) mov.enabled = false;
        if (TryGetComponent(out PlayerCombat combat)) combat.enabled = false;

        if (audioController != null) audioController.DeathSound();

        MouseLook mouseLook = GetComponentInChildren<MouseLook>();
        if (mouseLook != null) mouseLook.enabled = false;

        CameraRecoil recoil = GetComponentInChildren<CameraRecoil>();
        if (recoil != null) recoil.enabled = false;

        Camera fpsCam = GetComponentInChildren<Crosshair>().gameObject.GetComponent<Camera>();
        if (fpsCam != null)
        {
            MouseLook scriptLook = fpsCam.GetComponentInParent<MouseLook>();
            if (scriptLook != null) scriptLook.enabled = false;

            CameraRecoil scriptRecoil = fpsCam.GetComponent<CameraRecoil>();
            if (scriptRecoil != null) scriptRecoil.enabled = false;
        }

        if (TryGetComponent(out Animator animator)) animator.enabled = false;

        GetComponentInChildren<Crosshair>().enabled = false;
        StartCoroutine(DeathCameraAnimation(lastHitDirection));

        if (GameManager.Instance != null) GameManager.Instance.GameOver();
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
            targetWorldPos.y = hit.point.y + 0.1f;
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