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
    [SerializeField] private Text NotOpenable;

    [Header("Damage Indicator")]
    [SerializeField] private GameObject damageIndicatorPrefab;
    [SerializeField] private Transform damageIndicatorContainer;

   
    [SerializeField] private float mergeAngleThreshold = 40f;

    private List<DamageIndicator> activeIndicators = new List<DamageIndicator>();

    private static string default_interact = "Premi F per ";
    private static string default_pickup = "Premi F per raccogliere ";


    public void UpdateHealth(int currentHealth, int maxHealth)
    {
        // 1. Calcoliamo la percentuale esatta (è fondamentale usare i float per avere i decimali!)
        float healthPercentage = (float)currentHealth / (float)maxHealth;

        // 2. Impostiamo il fillAmount (usando = e non +=)
        healthImage.fillAmount = healthPercentage;

        // 3. Facciamo sfumare il colore in base alla percentuale
        healthImage.color = Color.Lerp(emptyHealthColor, fullHealthColor, healthPercentage);

        // 4. Aggiorniamo il testo
        healthText.text = currentHealth.ToString();
    }

    public void UpdateInventory(int medikitCount, int grenadeCount)
    {
        if (medikitText != null)
            medikitText.text = medikitCount.ToString();
        if (grenadeText != null)
            grenadeText.text = grenadeCount.ToString();
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

    public void ShowNotOpenable()
    {
        StartCoroutine(nameof(ShowOpen));
    }

    IEnumerator ShowOpen()
    {
        NotOpenable.gameObject.SetActive(true);
        yield return new WaitForSeconds(3);
        NotOpenable.gameObject.SetActive(false);
    }

    public void ShowDamageIndicator(Vector3 enemyPos)
    {
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
