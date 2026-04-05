using UnityEngine;

public class PlayerUpgrades : MonoBehaviour
{
    [Header("Livelli Attuali (0 = Base)")]
    public int movementSpeedLevel = 0;
    public int jumpLevel = 0;
    public int medkitLevel = 0;
    public int maxHealthLevel = 0;
    public int flitchLevel = 0;

    [Header("Livelli Massimi")]
    public int maxMaxHealthLevel = 4;
    public int maxSpeedLevel = 3;
    public int maxJumpLevel = 3;
    public int maxMedkitLevel = 4;
    public int maxFlitchLevel = 3;

    [Header("Valore Aggiunto per ogni Livello")]
    [Tooltip("Quanta velocità si aggiunge al walkSpeed e runSpeed per ogni livello?")]
    public float extraSpeedPerLevel = 1.0f;

    [Tooltip("Quanta forza di salto si aggiunge per ogni livello?")]
    public float extraJumpPerLevel = 1.5f;

    [Tooltip("Quanta salute in più cura il medikit per ogni livello? (o capacità massima dello zaino)")]
    public int extraHealingPerLevel = 5;

    public int extraHealthPerLevel = 25;

    private PlayerMovement movement;
    private VitalsController vitalsController;

    private float baseWalkSpeed;
    private float baseRunSpeed;
    private float baseJumpForce;
    private int baseMedkitHeal;
    private int baseMaxHealth;

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
            baseMaxHealth = vitalsController.maxHealth;
        }
    }

    public void Init(PlayerUpgradesSave save)
    {
        UploadLevelPlayer(save);
        UpdatePlayerStats();
    }

    private void UploadLevelPlayer(PlayerUpgradesSave save)
    {
        movementSpeedLevel = save.movementSpeedLevel;
        maxHealthLevel = save.maxHealthLevel;
        medkitLevel = save.medikitHealLevel;
        jumpLevel = save.jumpForceLevel;
        flitchLevel = save.flitchLevel;
        PlayerPrefs.SetInt("FlitchProbLevel", save.flitchLevel);
    }

    private void UpdatePlayerStats()
    {
        if (movement == null) return;

        movement.walkSpeed = baseWalkSpeed + (movementSpeedLevel * extraSpeedPerLevel);
        movement.jumpForce = baseJumpForce + (jumpLevel * extraJumpPerLevel);
        movement.runSpeed = baseRunSpeed + (movementSpeedLevel * extraSpeedPerLevel);
        vitalsController.medKitHeal = baseMedkitHeal + (medkitLevel * extraHealingPerLevel);
        vitalsController.maxHealth = baseMaxHealth + (maxHealthLevel * extraHealthPerLevel);
        PlayerPrefs.SetInt("FlitchProbLevel", flitchLevel);
    }
}