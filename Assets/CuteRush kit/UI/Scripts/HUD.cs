using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using UnityEngine;
using UnityEngine.UI;

public class HUD : UIPanel
{
    [SerializeField] private Image healthImage;
    [SerializeField] private Text healthText;
    [SerializeField] private Color emptyHealthColor;
    [SerializeField] private Color fullHealthColor;

    [SerializeField] private Text medikitText;
    [SerializeField] private Text grenadeText;

    [SerializeField] private Text InteractText;
    [SerializeField] private Text PickUpText;
    [SerializeField] private Text messageText;

    public float displayDuration = 3f;
    public float fadeDuration = 1f;

    [SerializeField] private Text Score;
    [SerializeField] private Text Gold;

    [Header("Damage Indicator")]
    [SerializeField] private GameObject damageIndicatorPrefab;
    [SerializeField] private Transform damageIndicatorContainer;
    [SerializeField] private float mergeAngleThreshold = 40f;

    [Header("Acido Borico UI")]
    [SerializeField] private UiAcidoBorico acidoBoricoPanel;

    private Coroutine messageCoroutine;


    

    private List<DamageIndicator> activeIndicators = new List<DamageIndicator>();

    private static string default_interact = "Premi F per ";
    private static string default_pickup = "Premi F per raccogliere ";

    private string lastMessage = "";
    private int lastColor = 0;

    public void SetupMaxAcidoBars(int maxBars)
    {
        if (acidoBoricoPanel != null) acidoBoricoPanel.SetupMaxBars(maxBars);
    }

    public void UpdateAcidoCharges(int charges)
    {
        if (acidoBoricoPanel != null) acidoBoricoPanel.UpdateCharges(charges);
    }

    public void UpdateAcidoProgress(float progress)
    {
        if (acidoBoricoPanel != null) acidoBoricoPanel.UpdateProgress(progress);
    }

    public void UpdateHealth(int currentHealth, int maxHealth)
    {
        float healthPercentage = (float)currentHealth / (float)maxHealth;
        healthImage.fillAmount = healthPercentage;
        healthImage.color = Color.Lerp(emptyHealthColor, fullHealthColor, healthPercentage);
        healthText.text = currentHealth.ToString();
    }

    public void UpdateScore(int score)
    {
        Score.text = score.ToString();
    }

    public void UpdateInventory(int medikitCount, int grenadeCount, int gold)
    {
        if (medikitText != null)
            medikitText.text = medikitCount.ToString();
        if (grenadeText != null)
            grenadeText.text = grenadeCount.ToString();
        if (Gold != null)
            Gold.text = gold.ToString();
    }

    public void ShowMessage(string message)
    {
        ShowMessage(message, Color.white);
    }

    public void ShowMessage(string message, Color color)
    {
        if (messageCoroutine != null)
        {
            StopCoroutine(messageCoroutine);
        }
        messageCoroutine = StartCoroutine(HandleMessageRoutine(message, color));
    }

    private IEnumerator HandleMessageRoutine(string msg, Color col)
    {
        col.a = 1f;

        messageText.text = msg;
        messageText.color = col;
        messageText.gameObject.SetActive(true);

        yield return new WaitForSeconds(displayDuration);

        float elapsed = 0f;
        Color currentColor = messageText.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float newAlpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            currentColor.a = newAlpha;
            messageText.color = currentColor;

            yield return null;
        }

        messageText.gameObject.SetActive(false);
    }


    public void ShowInteract(string item)
    {
        InteractText.text += item;
        InteractText.gameObject.SetActive(true);
    }

    public void HideInteract()
    {
        InteractText.text = default_interact;
        InteractText.gameObject.SetActive(false);
    }

    public void ShowPickUp(string item)
    {
        PickUpText.text += item;
        PickUpText.gameObject.SetActive(true);
    }

    public void HidePickUp()
    {
        PickUpText.text = default_pickup;
        PickUpText.gameObject.SetActive(false);
    }

    public void ShowDamageIndicator(Vector3 enemyPos)
    {

        if (!gameObject.activeInHierarchy) return;

        if (damageIndicatorPrefab == null) return;
        Transform parent = damageIndicatorContainer != null ? damageIndicatorContainer : transform;

        Transform camTrans = Camera.main.transform;
        Vector3 relDir = enemyPos - camTrans.position;
        float xRel = Vector3.Dot(relDir, camTrans.right);
        float yRel = Vector3.Dot(relDir, camTrans.forward);
        float angleDeg = Mathf.Atan2(xRel, yRel) * Mathf.Rad2Deg;

        
        DamageIndicator matchFound = null;
        foreach (var ind in activeIndicators)
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

            activeIndicators.Add(newInd);
        }
    }

    void Update()
    {
        activeIndicators.RemoveAll(x => x == null);
    }

}
