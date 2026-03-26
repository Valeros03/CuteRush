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

    // Riferimenti agli altri script del player
    private PlayerController playerController;
    private VitalsController vitalsController;

    private float baseWalkSpeed;
    private float baseRunSpeed;
    private float baseJumpForce;
    private int baseMedkitHeal;

    void Awake()
    {
        playerController = GetComponent<PlayerController>();
        vitalsController = GetComponent<VitalsController>();

        if (playerController != null)
        {
            baseWalkSpeed = playerController.walkSpeed;
            baseRunSpeed = playerController.runSpeed;
            baseJumpForce = playerController.jumpForce;
        }
        if(vitalsController != null)
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
        if (playerController == null) return;

        playerController.walkSpeed = baseWalkSpeed + (speedLevel * extraSpeedPerLevel);
        playerController.runSpeed = baseRunSpeed + (speedLevel * extraSpeedPerLevel);

        playerController.jumpForce = baseJumpForce + (jumpLevel * extraJumpPerLevel);
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