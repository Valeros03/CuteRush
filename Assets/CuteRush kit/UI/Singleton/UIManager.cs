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

    private void OnEnable()
    {
        InventoryPlayer.OnInventoryChanged += UpdateInventoryUI;
        VitalsController.OnHealthChange += UpdateHealth;
    }

    private void OnDisable()
    {
        InventoryPlayer.OnInventoryChanged -= UpdateInventoryUI;
    }

    public void UpdateInventoryUI(int medikitCount, int grenadeCount)
    {
        HUD.UpdateInventory(medikitCount, grenadeCount);
    }

    public void UpdateHealth(int health)
    {
        HUD.UpdateHealth(health);

    }
}
