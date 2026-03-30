using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Speed System")]
    public float walkSpeed = 5.0f;
    public float runSpeed = 8.0f;
    public float jumpForce = 6.0f;
    public bool limitDiagonalSpeed = true;
    public bool toggleRun = false;

    public enum motionstate { idle, running, walking, jumping }
    [Header("Motion System")]
    public motionstate currentMotion;

    [Header("Gravity system")]
    public float gravity = 10.0f;
    [SerializeField] private float groundedCheckDistance = 0.2f;
    [SerializeField] private float groundRayOffset = 0.1f;
    [SerializeField] private int ungroundedFramesToFall = 3;
    [SerializeField] private int groundedFramesToLand = 3;
    [SerializeField] private float slopeMaxAngleForGround = 60f;

    private CharacterController controller;
    private PlayerInput input;
    private AudioPlayerController audioPlayer;
    private Crosshair crosshairScript;

    private Transform myTransform;
    private Vector3 moveDirection;
    private bool grounded;
    private int ungroundedFrames = 0;
    private int groundedFrames = 0;
    private float verticalVelocity = 0f;
    private bool falling;
    private bool footstepsActive = false;
    private bool jumpTriggered = false;

    [HideInInspector] public Vector3 currentVelocity;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        input = GetComponent<PlayerInput>();
        audioPlayer = GetComponent<AudioPlayerController>();
        myTransform = transform;
    }

    private void Start()
    {
        currentMotion = motionstate.idle;

        Transform fpsCam = transform.Find("CameraHolder")?.Find("FPSCamera");
        if (fpsCam != null) crosshairScript = fpsCam.GetComponent<Crosshair>();
    }

    private void OnEnable()
    {
        input.OnJump += HandleJump;
    }

    private void OnDisable()
    {
        input.OnJump -= HandleJump;
    }

    private void HandleJump()
    {
        if (grounded)
        {
            jumpTriggered = true;
        }
    }

    private void FixedUpdate()
    {
        float inputX = input.MoveInput.x;
        float inputY = input.MoveInput.y;
        float inputModifyFactor = (inputX != 0.0f && inputY != 0.0f && limitDiagonalSpeed) ? 0.6701f : 1.0f;

        Vector3 move = new Vector3(inputX * inputModifyFactor, 0, inputY * inputModifyFactor);
        move = myTransform.TransformDirection(move);

        float targetSpeed = walkSpeed;

        if (input.IsRunning && !toggleRun)
        {
            targetSpeed = runSpeed;
            if (audioPlayer != null) audioPlayer.runMode();
        }
        else
        {
            if (audioPlayer != null) audioPlayer.walkMode();
        }
        move *= targetSpeed;

        if (jumpTriggered)
        {
            verticalVelocity = jumpForce;
            falling = false;
            currentMotion = motionstate.jumping;
            jumpTriggered = false;
        }

        if (!grounded)
        {
            verticalVelocity -= gravity * Time.deltaTime;
        }
        else
        {
            if (verticalVelocity < 0) verticalVelocity = -0.1f;
        }

        move.y = verticalVelocity;

        CollisionFlags flags = controller.Move(move * Time.deltaTime);

        currentVelocity = controller.velocity;

        bool hasBelowFlag = (flags & CollisionFlags.Below) != 0;
        bool rayHit = Physics.Raycast(myTransform.position + Vector3.up * groundRayOffset, Vector3.down, out RaycastHit hit, groundedCheckDistance + groundRayOffset);
        bool groundDetected = false;

        if (hasBelowFlag) groundDetected = true;
        else if (rayHit)
        {
            float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);
            if (slopeAngle <= slopeMaxAngleForGround) groundDetected = true;
        }

        if (groundDetected)
        {
            groundedFrames++;
            ungroundedFrames = 0;
        }
        else
        {
            ungroundedFrames++;
            groundedFrames = 0;
        }

        if (!groundDetected && ungroundedFrames >= ungroundedFramesToFall)
        {
            if (!falling) falling = true;
            grounded = false;
        }

        if (groundDetected && groundedFrames >= groundedFramesToLand)
        {
            if (falling) currentMotion = motionstate.idle;
            falling = false;
            grounded = true;
            groundedFrames = groundedFramesToLand;
            ungroundedFrames = 0;
            if (currentMotion == motionstate.jumping) currentMotion = motionstate.idle;
        }

        float moveThreshold = 0.1f;
        bool isMoving = (Mathf.Abs(inputX) > moveThreshold || Mathf.Abs(inputY) > moveThreshold);

        if (crosshairScript != null)
        {
            crosshairScript.isMoving = isMoving;
            crosshairScript.isJumping = !grounded;
        }

        if (grounded && isMoving && !footstepsActive)
        {
            currentMotion = motionstate.walking;
            if (audioPlayer != null) audioPlayer.PlayFootstep();
            footstepsActive = true;
        }
        else if ((!grounded || !isMoving) && footstepsActive)
        {
            currentMotion = motionstate.idle;
            if (audioPlayer != null) audioPlayer.StopFootstep();
            footstepsActive = false;
        }
    }
}