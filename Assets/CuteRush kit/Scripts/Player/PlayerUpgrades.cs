using UnityEngine;

public class PlayerUpgrades : MonoBehaviour
{
    [Header("Livelli Attuali (0 = Base)")]
    public int speedLevel = 0;
    public int jumpLevel = 0;
    public int medkitLevel = 0;

    [Header("Livelli Massimi")]
    public int maxSpeedLevel = 5;
    public int maxJumpLevel = 3;
    public int maxMedkitLevel = 4;

    [Header("Valore Aggiunto per ogni Livello")]
    [Tooltip("Quanta velocità si aggiunge al walkSpeed e runSpeed per ogni livello?")]
    public float extraSpeedPerLevel = 1.0f;

    [Tooltip("Quanta forza di salto si aggiunge per ogni livello?")]
    public float extraJumpPerLevel = 1.5f;

    [Tooltip("Quanta salute in più cura il medikit per ogni livello? (o capacità massima dello zaino)")]
    public int extraHealingPerLevel = 5;

    private PlayerMovement movement;
    private VitalsController vitalsController;

    private float baseWalkSpeed;
    private float baseRunSpeed;
    private float baseJumpForce;
    private int baseMedkitHeal;

    void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        vitalsController = GetComponent<VitalsController>();

        if (movement != null)
        {
            baseWalkSpeed = movement.walkSpeed;
            baseRunSpeed = movement.runSpeed;
            baseJumpForce = movement.jumpForce;
        }
        if (vitalsController != null)
        {
            baseMedkitHeal = vitalsController.medKitHeal;
        }
    }

    void Start()
    {
        UpdatePlayerStats();
    }

    private void UpdatePlayerStats()
    {
        if (movement == null) return;

        movement.walkSpeed = baseWalkSpeed + (speedLevel * extraSpeedPerLevel);
        movement.runSpeed = baseRunSpeed + (speedLevel * extraSpeedPerLevel);
        movement.jumpForce = baseJumpForce + (jumpLevel * extraJumpPerLevel);
        vitalsController.medKitHeal = baseMedkitHeal + (medkitLevel * extraHealingPerLevel);
    }

    public bool UpgradeSpeed()
    {
        if (speedLevel >= maxSpeedLevel)
        {
            return false;
        }

        speedLevel++;
        UpdatePlayerStats();
        return true;
    }

    public bool UpgradeJump()
    {
        if (jumpLevel >= maxJumpLevel) return false;

        jumpLevel++;
        UpdatePlayerStats();
        return true;
    }

    public bool UpgradeMedkit()
    {
        if (medkitLevel >= maxMedkitLevel) return false;

        medkitLevel++;
        UpdatePlayerStats();
        return true;
    }
}