using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUD : UIPanel
{

    [Header("Health UI")]
    [SerializeField] private Image healthImage;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private Color emptyHealthColor;
    [SerializeField] private Color fullHealthColor;

    [Header("Inventory UI")]
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private TextMeshProUGUI medikitText;
    [SerializeField] private TextMeshProUGUI grenadeText;
    [SerializeField] private TextMeshProUGUI scoreText;

    [Header("Interaction UI")]
    [SerializeField] private TextMeshProUGUI interactText;
    [SerializeField] private TextMeshProUGUI pickUpText;
    [SerializeField] private TextMeshProUGUI messageText;

    [Header("Damage Indicators")]
    [SerializeField] private GameObject damageIndicatorPrefab;
    [SerializeField] private Transform indicatorContainer;
    [SerializeField] private float mergeAngleThreshold = 40f;

    [Header("Special Systems")]
    [SerializeField] private UiAcidoBorico acidoBoricoPanel;

    [Header("Weapon UI")]
    [SerializeField] private TextMeshProUGUI ammoText;

    [Header("Timer UI")]
    [SerializeField] private TextMeshProUGUI timerText; 

    private InventoryPlayer _currentInventory;
    private VitalsController _currentVitals;
    private PlayerCombat _currentCombat;
    private List<DamageIndicator> _activeIndicators = new List<DamageIndicator>();
    private Coroutine _messageCoroutine;

    private const string DEFAULT_INTERACT = "Premi F per ";
    private const string DEFAULT_PICKUP = "Premi F per raccogliere ";

    public void ConnectToPlayer(InventoryPlayer inventory, VitalsController vitals, PlayerCombat combat)
    {
        Disconnect();

        _currentInventory = inventory;
        _currentVitals = vitals;
        _currentCombat = combat;

        if (_currentInventory != null)
        {
            _currentInventory.OnGoldChanged += UpdateGold;
            _currentInventory.OnMedkitsChanged += UpdateMedkit;
            _currentInventory.OnGrenadesChanged += UpdateGrenade;
            _currentInventory.OnMaxAcidoBoricoChanged += SetupMaxAcidoBars;
            _currentInventory.OnAcidoBoricoChanged += UpdateAcidoCharges;
            _currentInventory.OnAcidoRechargeProgress += UpdateAcidoProgress;

            UpdateGold(_currentInventory.getGold());
            UpdateMedkit(_currentInventory.getMedkitCount());
            UpdateGrenade(_currentInventory.getGrenadeCount());
        }

        if (_currentVitals != null)
        {
            _currentVitals.OnHealthChange += UpdateHealth;
            _currentVitals.OnTakeDamage += ShowDamageIndicator;
            UpdateHealth(_currentVitals.currentHealth, _currentVitals.maxHealth);
        }

        if (_currentCombat != null)
        {
            _currentCombat.OnActiveWeaponAmmoChanged += UpdateAmmoUI;
        }


        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnScoreChange += UpdateScore;
            UpdateScore(GameManager.Instance.currentScore);

            GameManager.Instance.OnTimeUpdated += UpdateTimerDisplay;
            UpdateTimerDisplay(0, 0);
        }

        UIEvents.OnShowNotification += ShowMessage;
        UIEvents.OnRequestInteract += ShowInteract;
        UIEvents.OnHideInteract += HideInteract;
    }

    public void Disconnect()
    {
        if (_currentInventory != null)
        {
            _currentInventory.OnGoldChanged -= UpdateGold;
            _currentInventory.OnMedkitsChanged -= UpdateMedkit;
            _currentInventory.OnGrenadesChanged -= UpdateGrenade;
            _currentInventory.OnMaxAcidoBoricoChanged -= SetupMaxAcidoBars;
            _currentInventory.OnAcidoBoricoChanged -= UpdateAcidoCharges;
            _currentInventory.OnAcidoRechargeProgress -= UpdateAcidoProgress;
        }

        if (_currentVitals != null)
        {
            _currentVitals.OnHealthChange -= UpdateHealth;
            _currentVitals.OnTakeDamage -= ShowDamageIndicator;
        }

        if (_currentCombat != null)
        {
            _currentCombat.OnActiveWeaponAmmoChanged -= UpdateAmmoUI;
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnScoreChange -= UpdateScore;
            GameManager.Instance.OnTimeUpdated -= UpdateTimerDisplay;
        }

        UIEvents.OnShowNotification -= ShowMessage;
        UIEvents.OnRequestInteract -= ShowInteract;
        UIEvents.OnHideInteract -= HideInteract;
    }

    private void UpdateAmmoUI(int current, int total)
    {
        if (ammoText != null)
        {
            ammoText.text = $"Bullets {current}/{total}";
        }
    }

    public void UpdateHealth(int current, int max)
    {
        float percent = (float)current / max;
        healthImage.fillAmount = percent;
        healthImage.color = Color.Lerp(emptyHealthColor, fullHealthColor, percent);
        healthText.text = current.ToString();
    }

    public void UpdateGold(int amount) => goldText.text = amount.ToString();
    public void UpdateMedkit(int count) => medikitText.text = count.ToString();
    public void UpdateGrenade(int count) => grenadeText.text = count.ToString();
    public void UpdateScore(int score) => scoreText.text = score.ToString();

    private void ShowMessage(string message, Color? color = null)
    {
        if (_messageCoroutine != null) StopCoroutine(_messageCoroutine);
        _messageCoroutine = StartCoroutine(MessageRoutine(message, color ?? Color.white));
    }

    private IEnumerator MessageRoutine(string msg, Color col)
    {
        messageText.text = msg;
        messageText.color = col;
        messageText.gameObject.SetActive(true);
        yield return new WaitForSeconds(3f);
        messageText.gameObject.SetActive(false);
    }

    private void ShowInteract(string item) { interactText.text = DEFAULT_INTERACT + item; interactText.gameObject.SetActive(true); }
    private void HideInteract() { interactText.gameObject.SetActive(false); }

    public void ShowDamageIndicator(Vector3 enemyPos)
    {
        if (!gameObject.activeInHierarchy || damageIndicatorPrefab == null) return;

        Transform parent = indicatorContainer != null ? indicatorContainer : transform;
        Transform camTrans = Camera.main.transform;
        Vector3 relDir = enemyPos - camTrans.position;

        float xRel = Vector3.Dot(relDir, camTrans.right);
        float yRel = Vector3.Dot(relDir, camTrans.forward);
        float angleDeg = Mathf.Atan2(xRel, yRel) * Mathf.Rad2Deg;

        DamageIndicator matchFound = null;

        foreach (var ind in _activeIndicators)
        {
            if (ind == null) continue;
            float diff = Mathf.Abs(Mathf.DeltaAngle(ind.GetCurrentZRotation(), -angleDeg));

            if (diff < mergeAngleThreshold)
            {
                matchFound = ind;
                break;
            }
        }

        if (matchFound != null)
        {
            matchFound.Refresh(enemyPos);
        }
        else
        {
            GameObject indicatorGO = Instantiate(damageIndicatorPrefab, parent);
            RectTransform rect = indicatorGO.GetComponent<RectTransform>();
            rect.anchoredPosition = Vector2.zero;

            DamageIndicator newInd = indicatorGO.GetComponent<DamageIndicator>();
            newInd.Initialize(enemyPos);
            _activeIndicators.Add(newInd);
        }
    }

    private void UpdateTimerDisplay(int minutes, int seconds)
    {
        if (timerText != null)
        {
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    public void SetupMaxAcidoBars(int bars) => acidoBoricoPanel.SetupMaxBars(bars);
    public void UpdateAcidoCharges(int charges) => acidoBoricoPanel.UpdateCharges(charges);
    public void UpdateAcidoProgress(float p) => acidoBoricoPanel.UpdateProgress(p);

    private void Update()
    {
        _activeIndicators.RemoveAll(x => x == null);
    }
}