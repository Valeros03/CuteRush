using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    private PlayerControl controls;

    public Vector2 MoveInput { get; private set; }
    public bool IsRunning { get; private set; }
    public bool IsFiring { get; private set; }

    public event Action OnJump;
    public event Action OnReload;
    public event Action OnInteract;
    public event Action OnHeal;
    public event Action OnEquipGrenade;
    public event Action OnFireStart;
    public event Action OnFireCancel;

    private void Awake()
    {
        controls = new PlayerControl();

        controls.Player.Jump.performed += HandleJump;
        controls.Player.Reload.performed += HandleReload;
        controls.Player.Interact.performed += HandleInteract;
        controls.Player.Heal.performed += HandleHeal;
        controls.Player.EquipGranade.performed += HandleEquipGrenade;

        controls.Player.Fire.started += HandleFireStart;
        controls.Player.Fire.canceled += HandleFireCancel;
    }

    private void OnEnable() => controls.Enable();
    private void OnDisable() => controls.Disable();

    private void Update()
    {
        MoveInput = controls.Player.Move.ReadValue<Vector2>();

        IsRunning = controls.Player.Run.ReadValue<float>() > 0f;
        IsFiring = controls.Player.Fire.ReadValue<float>() > 0f;
    }


    private void HandleJump(InputAction.CallbackContext context)
    {
        OnJump?.Invoke();
    }

    private void HandleReload(InputAction.CallbackContext context)
    {
        OnReload?.Invoke();
    }

    private void HandleInteract(InputAction.CallbackContext context)
    {
        OnInteract?.Invoke();
    }

    private void HandleHeal(InputAction.CallbackContext context)
    {
        OnHeal?.Invoke();
    }

    private void HandleEquipGrenade(InputAction.CallbackContext context)
    {
        OnEquipGrenade?.Invoke();
    }

    private void HandleFireStart(InputAction.CallbackContext context)
    {
        OnFireStart?.Invoke();
    }

    private void HandleFireCancel(InputAction.CallbackContext context)
    {
        OnFireCancel?.Invoke();
    }
}