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

    public static event Action<int, int, int> OnInventoryChanged;

    [Header("Acido Borico Settings")]
    public int maxAcidoBorico = 1;
    public float acidoRechargeTime = 15f;
    private int currentAcidoBorico;
    private bool isRechargingAcido = false;

    public static event Action<int> OnAcidoBoricoChanged;
    public static event Action<float> OnAcidoRechargeProgress;
    public static event Action<int> OnMaxAcidoBoricoChanged;

    private void Awake()
    {
        if (PlayerPrefs.HasKey("MaxAcidoBorico"))
        {
            maxAcidoBorico = PlayerPrefs.GetInt("MaxAcidoBorico");
        }
    }

    private void Start()
    {
        currentAcidoBorico = maxAcidoBorico;
        OnMaxAcidoBoricoChanged?.Invoke(maxAcidoBorico);

        OnInventoryChanged?.Invoke(MedkitCount, GrenadeCount, coins);
        OnAcidoBoricoChanged?.Invoke(currentAcidoBorico);
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

    public int getMedkitCount()
    {
        return MedkitCount;
    }

    public int getGrenadeCount()
    {
        return GrenadeCount;
    }

    public bool addMedkit()
    {
        if (MedkitCount < MaxMedkit)
        {
            MedkitCount++;
            OnInventoryChanged?.Invoke(MedkitCount, GrenadeCount, coins);
            return true;
        }
        return false;
    }

    public bool removeMedkit()
    {
        if (MedkitCount > 0)
        {
            MedkitCount--;
            OnInventoryChanged?.Invoke(MedkitCount, GrenadeCount, coins);
            return true;
        }
        return false;
    }

    public bool addGrenade()
    {
        if (GrenadeCount < MaxGrenade)
        {
            GrenadeCount++;
            OnInventoryChanged?.Invoke(MedkitCount, GrenadeCount, coins);
            return true;
        }
        return false;
    }

    public bool removeGrenade()
    {
        if (GrenadeCount > 0)
        {
            GrenadeCount--;
            OnInventoryChanged?.Invoke(MedkitCount, GrenadeCount, coins);
            return true;
        }
        return false;
    }

    public void addCoin()
    {
        coins++;
        OnInventoryChanged?.Invoke(MedkitCount, GrenadeCount, coins);
    }

    public void addCoin(int count)
    {
        
        coins += count;
        OnInventoryChanged?.Invoke(MedkitCount, GrenadeCount, coins);
    }

    public void removeCoin(int count)
    {
        coins -= count;
        OnInventoryChanged?.Invoke(MedkitCount, GrenadeCount, coins);
    }

    public int getCoins()
    {
        return coins;
    }
}