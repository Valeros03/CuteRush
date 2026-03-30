using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("UI Panel")]
    [SerializeField] private HUD HUD;
    [Header("Screens")]
    [SerializeField] private GameOverScreen gameOverScreen;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(transform.root.gameObject);
        HUD.Show();
    }
    public void ShowMessage(string message)
    {
        HUD.ShowMessage(message);
    }

    public void ShowMessage(string message, Color color)
    {
        HUD.ShowMessage(message, color);
    }

    private void OnEnable()
    {
        InventoryPlayer.OnInventoryChanged += UpdateInventoryUI;
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnScoreChange += UpdateScore;
            GameManager.Instance.OnGameOver += HandleGameOver;
        }

        VitalsController.OnHealthChange += UpdateHealth;

        InventoryPlayer.OnMaxAcidoBoricoChanged += SetupMaxAcidoBars;
        InventoryPlayer.OnAcidoBoricoChanged += UpdateAcidoCharges;
        InventoryPlayer.OnAcidoRechargeProgress += UpdateAcidoProgress;
    }

    private void OnDisable()
    {
        InventoryPlayer.OnInventoryChanged -= UpdateInventoryUI;
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnScoreChange -= UpdateScore;
            GameManager.Instance.OnGameOver -= HandleGameOver;
        }

        VitalsController.OnHealthChange -= UpdateHealth;

        InventoryPlayer.OnMaxAcidoBoricoChanged -= SetupMaxAcidoBars;
        InventoryPlayer.OnAcidoBoricoChanged -= UpdateAcidoCharges;
        InventoryPlayer.OnAcidoRechargeProgress -= UpdateAcidoProgress;
    }

    public void UpdateScore(int score)
    {
        HUD.UpdateScore(score);
    }

    public void UpdateInventoryUI(int medikitCount, int grenadeCount, int gold)
    {
        HUD.UpdateInventory(medikitCount, grenadeCount, gold);
    }

    public void UpdateHealth(int currentHealth, int maxHealth)
    {
        HUD.UpdateHealth(currentHealth, maxHealth);
    }

    public void ShowInteract(string item)
    {
        HUD.ShowInteract(item);

    }
    public void ShowPickUp(string item)
    {
        HUD.ShowPickUp(item);

    }

    public void HideInteract() 
    {
        HUD.HideInteract();
    }

    public void HidePickUp()
    {
        HUD.HidePickUp();
    }

    public void ShowDamageIndicator(Vector3 damageSourcePos)
    {
        HUD.ShowDamageIndicator(damageSourcePos);
    }

    public void SetupMaxAcidoBars(int maxBars)
    {
        HUD.SetupMaxAcidoBars(maxBars);
    }

    public void UpdateAcidoCharges(int charges)
    {
        HUD.UpdateAcidoCharges(charges);
    }

    public void UpdateAcidoProgress(float progress)
    {
        HUD.UpdateAcidoProgress(progress);
    }


    private void HandleGameOver()
    {
        if (HUD != null) HUD.gameObject.SetActive(false);

        if (gameOverScreen != null)
        {
            float timeSurvived = Time.timeSinceLevelLoad;

            int totalPoints = GameManager.Instance.currentScore;
            int totalKills = GameManager.Instance.killNumber;
            float multiplier = GameManager.Instance.difficultyMultiplier;

            gameOverScreen.ShowGameOverStats(timeSurvived, totalPoints, totalKills, multiplier);
        }
    }


}
