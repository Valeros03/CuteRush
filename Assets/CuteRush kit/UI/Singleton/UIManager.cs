using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("UI Panel")]
    [SerializeField] private HUD HUD;

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
        GameManager.Instance.OnScoreChange += UpdateScore;
        VitalsController.OnHealthChange += UpdateHealth;
    }

    private void OnDisable()
    {
        InventoryPlayer.OnInventoryChanged -= UpdateInventoryUI;
        GameManager.Instance.OnScoreChange -= UpdateScore;
        VitalsController.OnHealthChange -= UpdateHealth; 
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

    
}
