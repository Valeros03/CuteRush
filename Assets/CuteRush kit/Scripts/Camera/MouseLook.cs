using UnityEngine;
using UnityEngine.InputSystem;

public class MouseLook : MonoBehaviour
{
    [Header("Riferimenti")]
    public GameObject player;

    [Header("Input")]
    [Tooltip("Trascina qui la tua Action 'Look' (es. Mouse Delta)")]
    public InputActionReference lookAction;

    [Header("Impostazioni")]
    public float xSensitivity = 10;
    public float ySensitivity = 10;
    public float smoothing = 0.4f;
    public int min = -60;
    public int max = 60;

    private float mouseOffsetY;
    private float mouseOffsetX;

    private float xtargetRotation = 10;
    private float ytargetRotation = 10;

    private void OnEnable()
    {
        if (lookAction != null) lookAction.action.Enable();
    }

    private void OnDisable()
    {
        if (lookAction != null) lookAction.action.Disable();
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        Vector2 lookInput = Vector2.zero;
        if (lookAction != null)
        {
            lookInput = lookAction.action.ReadValue<Vector2>();
        }

        mouseOffsetY = lookInput.y * ySensitivity * Time.deltaTime;
        mouseOffsetX = lookInput.x * xSensitivity * Time.deltaTime;


        ytargetRotation += -mouseOffsetY;
        ytargetRotation = ytargetRotation % 360;
        ytargetRotation = Mathf.Clamp(ytargetRotation, min, max);

        xtargetRotation += mouseOffsetX;
        xtargetRotation = xtargetRotation % 360;

        transform.localRotation = Quaternion.Lerp(
            transform.localRotation,
            Quaternion.Euler(ytargetRotation, 0, 0),
            Time.deltaTime * 10 / smoothing
        );

        player.transform.rotation = Quaternion.Lerp(
            player.transform.rotation,
            Quaternion.Euler(0, xtargetRotation, 0),
            Time.deltaTime * 10 / smoothing
        );
    }
}