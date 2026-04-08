using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

[System.Serializable]
public class WeaponSpawnData
{
    public string weaponName;
    public AssetReferenceGameObject realWeaponRef;
    public AssetReferenceGameObject hologramRef;
}

public class WeaponSpawner : InteractableItem
{
    [Header("Configurazione Piattaforma")]
    public float respawnTime = 90f;
    public Transform spawnPoint;
    public List<WeaponSpawnData> availableWeapons;

    private GameObject currentHologram;
    private GameObject preInstantiatedWeapon;
    private WeaponSpawnData currentWeaponData;

    private bool isReady = false;
    private bool playerInTrigger = false;

    void Start()
    {
        StartCoroutine(RespawnSequence(0f));
    }

    public override void Interact(PlayerInteraction player = null)
    {
        if (!isReady || preInstantiatedWeapon == null) return;

        GiveWeaponToPlayer(preInstantiatedWeapon);
        preInstantiatedWeapon = null;

        if (currentHologram != null)
        {
            if (!Addressables.ReleaseInstance(currentHologram)) Destroy(currentHologram);
            currentHologram = null;
        }

        HideInteraction();
        isReady = false;

        StartCoroutine(RespawnSequence(respawnTime));
    }

    private IEnumerator RespawnSequence(float waitTime)
    {
        yield return Addressables.InitializeAsync();

        currentWeaponData = ChooseNextWeapon();

        if (currentWeaponData.hologramRef != null && currentWeaponData.hologramRef.RuntimeKeyIsValid())
        {
            AsyncOperationHandle<GameObject> hologramHandle = Addressables.InstantiateAsync(currentWeaponData.hologramRef, spawnPoint.position, spawnPoint.rotation, transform);
            yield return hologramHandle;

            currentHologram = hologramHandle.Result;
            currentHologram.SetActive(false);
        }
        if (currentWeaponData.realWeaponRef != null && currentWeaponData.realWeaponRef.RuntimeKeyIsValid())
        {
            AsyncOperationHandle<GameObject> weaponHandle = Addressables.InstantiateAsync(currentWeaponData.realWeaponRef, spawnPoint.position, spawnPoint.rotation, transform);
            yield return weaponHandle;

            preInstantiatedWeapon = weaponHandle.Result;
            preInstantiatedWeapon.SetActive(false);

            GunBase gunComponent = preInstantiatedWeapon.GetComponent<GunBase>();
            if (gunComponent != null)
            {
                string weaponName = currentWeaponData.weaponName;
                int level = 1;

                if (SaveManager.Instance != null && SaveManager.Instance.currentSave != null && SaveManager.Instance.currentSave.weaponUpgrades != null)
                {
                    WeaponUpgradesSave upgrades = SaveManager.Instance.currentSave.weaponUpgrades;
                    if (weaponName == GameConstants.WEAPON_PISTOL)
                    {
                        level = upgrades.pistolLevel;
                    }
                    else if (weaponName == GameConstants.WEAPON_SMG)
                    {
                        level = upgrades.smgLevel;
                    }
                    else if (weaponName == GameConstants.WEAPON_RAILGUN)
                    {
                        level = upgrades.railgunLevel;
                    }
                }

                string addressablePath = $"Assets/CuteRush kit/Presets/Weapons/Specs/{weaponName} Preset {level}.asset";
                AsyncOperationHandle<GunStats> statsHandle = Addressables.LoadAssetAsync<GunStats>(addressablePath);
                yield return statsHandle;

                if (statsHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    gunComponent.stats = statsHandle.Result;
                }
                else
                {
                    Debug.LogWarning($"Addressables failed to load weapon stats at {addressablePath}. Trying fallback.");
                    string fallbackPath = $"Assets/CuteRush kit/Presets/Weapons/Specs/{weaponName} Preset 1.asset";
                    AsyncOperationHandle<GunStats> fallbackHandle = Addressables.LoadAssetAsync<GunStats>(fallbackPath);
                    yield return fallbackHandle;
                    if (fallbackHandle.Status == AsyncOperationStatus.Succeeded)
                    {
                        gunComponent.stats = fallbackHandle.Result;
                    }
                    else
                    {
                        Debug.LogError($"Fallback Addressables failed to load at {fallbackPath}.");
                    }
                }
            }
        }
        yield return new WaitForSeconds(waitTime);

        isReady = true;
        if (currentHologram != null) currentHologram.SetActive(true);
        if (playerInTrigger) ShowInteraction();
    }

    private WeaponSpawnData ChooseNextWeapon()
    {
        PlayerCombat combat = FindObjectOfType<PlayerCombat>();
        string currentWeaponName = combat != null ? combat.GetCurrentWeaponName() : "";
        List<WeaponSpawnData> filteredWeapons = new List<WeaponSpawnData>();

        foreach (WeaponSpawnData weaponData in availableWeapons)
            if (weaponData.weaponName != currentWeaponName)
                filteredWeapons.Add(weaponData);

        if (filteredWeapons.Count == 0) filteredWeapons = availableWeapons;

        return filteredWeapons[Random.Range(0, filteredWeapons.Count)];
    }

    private void GiveWeaponToPlayer(GameObject readyWeapon)
    {
        PlayerCombat combat = FindObjectOfType<PlayerCombat>();
        if (combat != null) combat.EquipWeapon(readyWeapon);
    }

    public override void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(GameConstants.PLAYER_TAG))
        {
            playerInTrigger = true;
            if (isReady) ShowInteraction();
        }
    }

    public override void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag(GameConstants.PLAYER_TAG))
        {
            playerInTrigger = false;
            HideInteraction();
        }
    }
}