using System;
using System.Collections;
using UnityEngine;

public class InventoryPlayer : MonoBehaviour
{
    private int MedkitCount = 0;
    private int GrenadeCount = 0;
    private int coins = 0;

    private static int MaxMedkit = 10;
    private static int MaxGrenade = 10;

    [Header("Acido Borico Settings")]
    public int maxAcidoBorico = 1;
    public float acidoRechargeTime = 15f;
    private int currentAcidoBorico;
    private bool isRechargingAcido = false;

    public delegate void ResourceChangedHandler(int newAmount);

    public event ResourceChangedHandler OnGoldChanged;
    public event ResourceChangedHandler OnMedkitsChanged;
    public event ResourceChangedHandler OnGrenadesChanged;

    public event Action<int> OnAcidoBoricoChanged;
    public event Action<float> OnAcidoRechargeProgress;
    public event Action<int> OnMaxAcidoBoricoChanged;

    public void Init()
    {
        if (PlayerPrefs.HasKey("MaxAcidoBorico"))
        {
            maxAcidoBorico = PlayerPrefs.GetInt("MaxAcidoBorico");
        }

        currentAcidoBorico = maxAcidoBorico;

        OnMaxAcidoBoricoChanged?.Invoke(maxAcidoBorico);
        OnAcidoBoricoChanged?.Invoke(currentAcidoBorico);
        OnMedkitsChanged?.Invoke(MedkitCount);
        OnGoldChanged?.Invoke(coins);
        OnGrenadesChanged?.Invoke(GrenadeCount);
        addGrenade();
        
    }

    public bool UseAcidoBorico()
    {
        if (currentAcidoBorico > 0)
        {
            currentAcidoBorico--;
            OnAcidoBoricoChanged?.Invoke(currentAcidoBorico);

            if (!isRechargingAcido)
            {
                StartCoroutine(RechargeAcidoRoutine());
            }
            return true;
        }
        return false;
    }

    private IEnumerator RechargeAcidoRoutine()
    {
        isRechargingAcido = true;

        while (currentAcidoBorico < maxAcidoBorico)
        {
            float elapsed = 0f;

            while (elapsed < acidoRechargeTime)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / acidoRechargeTime;
                OnAcidoRechargeProgress?.Invoke(progress);
                yield return null;
            }

            currentAcidoBorico++;
            OnAcidoBoricoChanged?.Invoke(currentAcidoBorico);
        }

        OnAcidoRechargeProgress?.Invoke(0f);
        isRechargingAcido = false;
    }

    public int getMedkitCount() => MedkitCount;
    public int getGrenadeCount() => GrenadeCount;
    public int getGold() => coins;

    public bool addMedkit()
    {
        if (MedkitCount < MaxMedkit) { MedkitCount++; OnMedkitsChanged?.Invoke(MedkitCount); return true; }
        return false;
    }

    public bool removeMedkit()
    {
        if (MedkitCount > 0) { MedkitCount--; OnMedkitsChanged?.Invoke(MedkitCount); return true; }
        return false;
    }

    public bool addGrenade()
    {
        if (GrenadeCount < MaxGrenade) { GrenadeCount++; OnGrenadesChanged?.Invoke(GrenadeCount); return true; }
        return false;
    }

    public bool removeGrenade()
    {
        if (GrenadeCount > 0) { GrenadeCount--; OnGrenadesChanged?.Invoke(GrenadeCount); return true; }
        return false;
    }

    public void addCoin(int count = 1) { coins += count; OnGoldChanged?.Invoke(coins); }
    public void removeCoin(int count) { coins -= count; OnGoldChanged?.Invoke(coins); }
}