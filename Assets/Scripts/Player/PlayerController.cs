using System.Collections;
using System.Collections.Generic;
using TheDeveloperTrain.SciFiGuns;
using UnityEngine;

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
    public float jumpSpeed = 6.0f;
    public bool limitDiagonalSpeed = true;
    public bool toggleRun = false;
    public bool toggleSneak = false;
    public bool airControl = false;
    public bool crouching = false;

    public enum motionstate
    {
        idle,
        running,
        jumping
    }
    [Header("Motion System")]
    public motionstate currentMotion;

    [Header("Gravity system")]
    public float gravity = 10.0f;
    public float fallingDamageLimit = 10.0f;
    private bool grounded;

    [Header("Input System")]
    public KeyCode inventoryKey = new KeyCode();

    [Header("GameObjects")]
    public GameObject camera;
    [SerializeField] private GameObject weaponHolder;
    [SerializeField] private GameObject granadeHolder;

    [SerializeField] private Animator weaponAnimator;   // animatore dell'arma
    [SerializeField] private Animator granadeAnimator;


    // Private Variables
    private Vector3 moveDirection;
    private CharacterController controller;
    private Transform myTransform;
    private float speed;
    private RaycastHit hit;
    private float fallStartLevel;
    private bool falling;
    private bool punching;
    private bool playerControl;
    private Crosshair crosshairScript;
    private Gun gun;
    

    // Use this for initialization
    void Start()
    {
        gun = weaponHolder.GetComponentInChildren<Gun>();
        currentMotion = motionstate.idle;
        moveDirection = Vector3.zero;
        grounded = false;
        playerControl = false;
        controller = GetComponent<CharacterController>();
        myTransform = transform;
        speed = walkSpeed;
        crosshairScript = transform.Find("FPSCamera").GetComponent<Crosshair>();

        // Lock cursor
        Cursor.visible = false;
    }

    private void FixedUpdate()
    {
        float inputX = Input.GetAxis("Horizontal");
        float inputY = Input.GetAxis("Vertical");
        float inputModifyFactor = (inputX != 0.0f && inputY != 0.0f && limitDiagonalSpeed) ? 0.6701f : 1.0f;

        if (inputX == 0 && inputY == 0)
        {
            currentMotion = motionstate.idle;
        }

        if (grounded)
        {
            if (falling)
            {
                falling = false;
                if (myTransform.position.y < (fallStartLevel - fallingDamageLimit))
                {
                    FallingDamageAlert(fallStartLevel - myTransform.position.y);
                }
            }

            if (!toggleRun)
            {
                bool running = Input.GetButton("Run");
                speed = running ? runSpeed : walkSpeed;

                if (running)
                {
                    currentMotion = motionstate.running;
                    crosshairScript.IncreaseSpread(0.5f);
                }
                else
                {
                    if (crosshairScript.spread != crosshairScript.minSpread)
                        crosshairScript.DecreaseSpread(2f);
                }
            }

            if (Input.GetButtonUp("Run"))
            {
                currentMotion = PlayerController.motionstate.idle;
             
            }

            if (!toggleSneak)
            {
                bool sneaking = Input.GetButton("Sneak");
                speed = sneaking ? sneakSpeed : speed;
                //anim.SetBool("sneaking", sneaking);
            }

            if (crouching)
            {
                speed = Input.GetButton("Run") ? crouchRunSpeed : crouchWalkSpeed;
                speed = Input.GetButton("Sneak") ? crouchSneakSpeed : speed;
            }

            moveDirection = new Vector3(inputX * inputModifyFactor, 0, inputY * inputModifyFactor);
            moveDirection = myTransform.TransformDirection(moveDirection) * speed;

            if (Input.GetButton("Jump"))
            {
                moveDirection.y = jumpSpeed;
                crosshairScript.IncreaseSpread(10f);
                currentMotion = motionstate.jumping;
            }

            if (Input.GetButton("Fire1"))
            {
                if (gun != null)
                {
                    gun.Shoot();
                }
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                if (gun != null)
                {
                    gun.Reload();
                }
            }
            if (Input.GetKeyDown(KeyCode.G))
            {
                HandleGranadeEquipe();
            }
        }
        else
        {
            if (!falling)
            {
                falling = true;
                fallStartLevel = myTransform.position.y;
            }

            if (airControl && playerControl)
            {
                moveDirection.x = inputX * speed * inputModifyFactor;
                moveDirection.z = inputY * speed * inputModifyFactor;
                moveDirection = myTransform.TransformDirection(moveDirection);
            }
        }

        grounded = (controller.Move(moveDirection * Time.deltaTime) & CollisionFlags.Below) != 0;
        moveDirection.y -= gravity * Time.deltaTime;
    }

    void Update()
    {

        if (toggleRun && grounded && Input.GetButtonDown("Run"))
            speed = (speed == walkSpeed ? runSpeed : walkSpeed);

    }

    void FallingDamageAlert(float fallDistance)
    {
        print("Ouch! Fell " + fallDistance + " units!");
    }


    void HandleGranadeEquipe()
    {
        if (!granadeHolder.activeSelf)
        {
            // ---- EQUIPAGGIA LA GRANATA ----
            if (weaponAnimator != null)
            {
                weaponAnimator.SetTrigger("PosaArma"); // arma si abbassa
            }

            StartCoroutine(SwitchToGranade());
        }
        else
        {
            // ---- TORNA ALL'ARMA ----
            if (granadeAnimator != null)
            {
                granadeAnimator.SetTrigger("PosaGranata"); // granata si abbassa
            }

            StartCoroutine(SwitchToWeapon());
        }
    }

    IEnumerator SwitchToGranade()
    {
        // tempo animazione abbassamento arma
        yield return new WaitForSeconds(0.5f);

        weaponHolder.transform.GetChild(0).gameObject.SetActive(false);
        granadeHolder.SetActive(true);

        if (granadeAnimator != null)
        {
            granadeAnimator.SetTrigger("PrendiGranata"); // granata si alza
        }
    }

    IEnumerator SwitchToWeapon()
    {
        // tempo animazione abbassamento granata
        yield return new WaitForSeconds(0.5f);

        granadeHolder.SetActive(false);
        weaponHolder.transform.GetChild(0).gameObject.SetActive(true);

        if (weaponAnimator != null)
        {
            weaponAnimator.SetTrigger("EquipaggiaArma"); // arma si alza
        }
    }
}