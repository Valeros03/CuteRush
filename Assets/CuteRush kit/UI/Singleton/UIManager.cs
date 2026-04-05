using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private HUD hud;
    [SerializeField] private MainMenu mainMenu;
    [SerializeField] private MainMenuSaveIntegration saveMenu;
    [SerializeField] private GameOverScreen gameOverScreen;
    [SerializeField] private GameObject latterbox;
    [SerializeField] private GameObject topBar;
    [SerializeField] private GameObject bottomBar;

    // [Requires Inspector Setup] Create a Loading Screen UI panel, and assign it to this variable.
    // Ensure it contains a button that calls Bootstrapper.Instance.ContinueFromLoadingScreen() on click.
    [SerializeField] private GameObject loadingScreen;

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

    public void ShowLatterbox()
    {
        
        latterbox.SetActive(true);
    }

    public void FadeLatterbox()
    {
        latterbox.GetComponent<Animator>().SetTrigger(GameConstants.ANIM_FADE);
    }
    public void ResetLatterbox()
    {
        topBar.transform.localScale = new Vector3(1, 1, 1);
        bottomBar.transform.localScale = new Vector3(1, 1, 1);
        latterbox.SetActive(false);
    }

    public void StartGameSequence(InventoryPlayer inv, VitalsController vitals, PlayerCombat combat)
    {
        mainMenu.Hide();
        gameOverScreen.Hide();
        hud.Show();
        hud.ConnectToPlayer(inv, vitals, combat);
    }

    public void EndGameSequence(float time, int score, int kill, float diffMult, float kpmMult)
    {
        hud.Hide();
        hud.Disconnect();
        gameOverScreen.Show();
        gameOverScreen.ShowGameOverStats(time,score,kill,diffMult, kpmMult);
    }

    public void StartGameMenu()
    {
        saveMenu.Hide();
    }

    public void ShowLoadingScreen()
    {
        if (loadingScreen != null)
            loadingScreen.SetActive(true);
    }

    public void HideLoadingScreen()
    {
        if (loadingScreen != null)
            loadingScreen.SetActive(false);
    }
}