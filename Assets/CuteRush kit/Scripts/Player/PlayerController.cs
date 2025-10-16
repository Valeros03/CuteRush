using System.Collections;
using TheDeveloperTrain.SciFiGuns;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{


    [Header("Speed System")]
    public float walkSpeed = 5.0f;
    public float sneakSpeed = 2.5f;
    public float runSpeed = 8.0f;
    public float crouchWalkSpeed = 3.5f;
    public float crouchRunSpeed = 6.5f;
    public float crouchSneakSpeed = 1f;
    public float jumpForce = 6.0f;
    public bool limitDiagonalSpeed = true;
    public bool toggleRun = false;
    public bool toggleSneak = false;
    public bool airControl = false;
    public bool crouching = false;


    public enum motionstate
    {
        idle,
        running,
        walking,
        jumping
    }

    [Header("Motion System")]
    public motionstate currentMotion;

    [Header("Gravity system")]
    public float gravity = 10.0f;
    public float fallingDamageLimit = 10.0f;
    [SerializeField] private float groundedCheckDistance = 0.2f; // distanza max per considerare "a terra"
    [SerializeField] private float groundRayOffset = 0.1f;       // offset verso l'alto per il raycast
    [SerializeField] private int ungroundedFramesToFall = 3;     // quanti FixedUpdate consecutivi senza terra per dichiarare "in aria"
    [SerializeField] private int groundedFramesToLand = 3;      // quanti FixedUpdate consecutivi con terra per dichiarare "atterrato"
    [SerializeField] private float slopeMaxAngleForGround = 60f; // opzionale: inclinazione massima considerata "pavimento"
    [SerializeField] private float fallMultiplier = 2f; // rende la discesa più veloce (più realistica)
    private bool grounded;
    private int ungroundedFrames = 0;
    private int groundedFrames = 0;
    private float verticalVelocity = 0f;

    [Header("GameObjects")]
    public GameObject camera;
    [SerializeField] private GameObject weaponHolder;
    [SerializeField] private GameObject granade;

    private Animator weaponAnimator;   // animatore dell'arma
    [SerializeField] private Animator granadeAnimator;
    [SerializeField] private InventoryPlayer inventory;


    // Private Variables
    private Vector3 moveDirection;
    private CharacterController controller;
    private Transform myTransform;
    private float speed;
    private float fallStartLevel;
    private bool falling;

    private bool playerControl;
    private Crosshair crosshairScript;
    private GunBase gun;

    private PlayerControl controls; // classe generata

    private bool footstepsActive = false;
    


    // Use this for initialization
    void Start()
    {
        gun = weaponHolder.GetComponentInChildren<GunBase>();
        currentMotion = motionstate.idle;
        moveDirection = Vector3.zero;
        grounded = false;
        playerControl = false;
        controller = GetComponent<CharacterController>();
        myTransform = transform;
        speed = walkSpeed;
        crosshairScript = transform.Find("CameraHolder").transform.Find("FPSCamera").GetComponent<Crosshair>();
        weaponAnimator = transform.Find("CameraHolder").Find("FPSCamera").Find("WeaponHolder").GetComponentInChildren<Animator>();
        // Lock cursor
        Cursor.visible = false;
        AudioManager.Instance.PlayMusic("InGameSong");
        AudioManager.Instance.PlayAmbient("Spaceship Engine Light");
    }

    
    private void Awake()
    {
        controls = new PlayerControl();
        controls.Player.EquipGranade.performed += HandleGranadeEquipe;
        
    }

    private void OnEnable() => controls.Enable();
    private void OnDisable() => controls.Disable();

    private void FixedUpdate()
    {
        float inputX = Input.GetAxis("Horizontal");
        float inputY = Input.GetAxis("Vertical");
        float inputModifyFactor = (inputX != 0.0f && inputY != 0.0f && limitDiagonalSpeed) ? 0.6701f : 1.0f;

        // --- MOVIMENTO ORIZZONTALE ---
        Vector3 move = new Vector3(inputX * inputModifyFactor, 0, inputY * inputModifyFactor);
        move = myTransform.TransformDirection(move);

        float targetSpeed = walkSpeed;
        bool running = Input.GetButton("Run");
        if (running)
        {
            targetSpeed = runSpeed;
            gameObject.GetComponent<AudioPlayerController>().runMode();
        }
        else
        {
            gameObject.GetComponent<AudioPlayerController>().walkMode();
        }
        move *= targetSpeed;

        // --- SALTO ---
        if (grounded && Input.GetButtonDown("Jump"))
        {
            verticalVelocity = jumpForce;
            falling = false;
            currentMotion = motionstate.jumping;
            crosshairScript.IncreaseSpread(2f);
            
        }

        // --- GRAVITÀ / DISCESA ---
        if (!grounded)
        {
            if (verticalVelocity > 0 && !Input.GetButton("Jump"))
            {
                // rilascio del tasto → discesa più rapida
                verticalVelocity -= gravity * fallMultiplier * Time.deltaTime;
            }
            else
            {
                verticalVelocity -= gravity * Time.deltaTime;
            }
        }
        else
        {
            // quando è a terra, mantieni una leggera spinta verso il basso
            if (verticalVelocity < 0)
                verticalVelocity = -0.1f;
        }

        // --- COMBINAZIONE MOVIMENTO ---
        move.y = verticalVelocity;

        // --- MUOVI PERSONAGGIO ---
        CollisionFlags flags = controller.Move(move * Time.deltaTime);

        // --- RAYCAST + Isteresi Ground Check ---
        bool hasBelowFlag = (flags & CollisionFlags.Below) != 0;

        RaycastHit hit;
        bool rayHit = Physics.Raycast(myTransform.position + Vector3.up * groundRayOffset, Vector3.down, out hit, groundedCheckDistance + groundRayOffset);

        bool groundDetected = false;
        if (hasBelowFlag)
        {
            groundDetected = true;
        }
        else if (rayHit)
        {
            float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);
            if (slopeAngle <= slopeMaxAngleForGround)
                groundDetected = true;
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

        // --- TRANSIZIONE DA TERRA A CADUTA ---
        // Se siamo appena saltati, verticalVelocity > 0 → ignoriamo il check di caduta
        if (!groundDetected && ungroundedFrames >= ungroundedFramesToFall)
        {
            if (!falling)
            {
                falling = true;
                fallStartLevel = myTransform.position.y;

            }
            grounded = false;

        }


        // --- TRANSIZIONE DA CADUTA A TERRA ---
        if (groundDetected && groundedFrames >= groundedFramesToLand)
        {
            
            if (falling)
            {
                currentMotion = motionstate.idle;
                crosshairScript.DecreaseSpread(2f);
            }

            falling = false;
            grounded = true;
            groundedFrames = groundedFramesToLand;
            ungroundedFrames = 0;

            if (currentMotion == motionstate.jumping)
                currentMotion = motionstate.idle;
        }


        // --- AUDIO E STATI ---
        float moveThreshold = 0.1f;
        bool isMoving = (Mathf.Abs(inputX) > moveThreshold || Mathf.Abs(inputY) > moveThreshold);

        // Attiva i passi solo al cambio di stato
        if (grounded && isMoving && !footstepsActive)
        {
            currentMotion = motionstate.walking;
            gameObject.GetComponentInChildren<AudioPlayerController>().PlayFootstep();
            footstepsActive = true;
        }
        else if ((!grounded || !isMoving) && footstepsActive)
        {
            
            currentMotion = motionstate.idle;
            gameObject.GetComponentInChildren<AudioPlayerController>().StopFootstep();
            footstepsActive = false;
        }
    }

    void Update()
    {

        if (toggleRun && grounded && Input.GetButtonDown("Run"))
            speed = (speed == walkSpeed ? runSpeed : walkSpeed);

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (gun != null)
            {
                gun.StartReload();
            }
        }
        if (Input.GetButton("Fire1"))
        {
            if (gun != null)
            {
                gun.TryShoot();
            }
            
        }
        if (Input.GetButtonDown("Fire1")) {

            if (granade.activeInHierarchy)
            {
                granade.GetComponent<GrandeThrower>().Activation();
            }

        }else if (Input.GetButtonUp("Fire1"))
        {
            if (granade.activeInHierarchy)
            {
                granade.GetComponent<GrandeThrower>().ThrowGrenade();
                SwitchToWeapon();
                
            }
            if (gun.gameObject.activeInHierarchy)
            {
                if(gun.stats.fireMode == FireMode.Single)
                    gun.ResetSingleShot();
            }
        }

        
    }


    void HandleGranadeEquipe(InputAction.CallbackContext context)
    {
        if (!context.performed) { return; }
        
        if (!granade.activeSelf)
        { 
            if (weaponAnimator != null)
            {
                gun.enabled = false;
                gun.transform.Find("Tracer").gameObject.SetActive(false);
                weaponAnimator.SetTrigger("PosaArma");
            }
        }
        else
        {
            // ---- TORNA ALL'ARMA ----
            if (granadeAnimator != null)
            {
                granadeAnimator.SetTrigger("PosaGranata"); // granata si abbassa
            }
        }
    }

    public void SwitchToGranade()
    {
      
        //disabilita logica e body dell'arma
        
        gun.transform.GetChild(0).gameObject.SetActive(false);

        granade.SetActive(true);

        if (granadeAnimator != null)
        {
            granade.transform.SetParent(granade.GetComponentInParent<Transform>());
            granade.transform.localPosition = new Vector3(-0.352f, -0.664f, 0.011f);
            granade.transform.localRotation = Quaternion.identity;
            granadeAnimator.SetTrigger("PrendiGranata"); // granata si alza
        }
    }

    public void SwitchToWeapon()
    {
        granade.transform.Find("Granade").gameObject.SetActive(true);
        granade.SetActive(false);
        
        gun.enabled = true;
        gun.transform.Find("Tracer").gameObject.SetActive(true);
        gun.transform.GetChild(0).gameObject.SetActive(true);

        if (weaponAnimator != null)
        {
            weaponAnimator.SetTrigger("EquipaggiaArma"); // granata si alza
        }
    }

    public void addMedkit()
    {
        inventory.addMedkit();
    }

    public void addGrenade()
    {
        inventory.addGrenade();
    }
    
    public void addAmmo()
    {
        gun.addMag();
    }
}