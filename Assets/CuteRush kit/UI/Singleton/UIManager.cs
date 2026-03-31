using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private HUD hud;
    [SerializeField] private MainMenu mainMenu;
    [SerializeField] private GameOverScreen gameOverScreen;

    public HUD HUD => hud;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void OpenMenu()
    {
        mainMenu.Show();
        hud.Hide();
        gameOverScreen.Hide();
    }

    public void StartGameSequence(InventoryPlayer inv, VitalsController vitals, PlayerCombat combat)
    {
        mainMenu.Hide();
        gameOverScreen.Hide();
        hud.Show();
        hud.ConnectToPlayer(inv, vitals, combat);

        Camera menuCam = GetComponentInChildren<Camera>();
        if (menuCam != null)
        {
            menuCam.gameObject.SetActive(false);
        }
    }

    public void EndGameSequence()
    {
        hud.Hide();
        hud.Disconnect();
        gameOverScreen.Show();
    }
}