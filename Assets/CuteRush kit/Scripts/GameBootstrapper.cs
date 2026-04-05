using UnityEngine;

public class GameBootstrapper : MonoBehaviour
{
    [Header("Player Setup")]
    [SerializeField] private GameObject player;

    [Header("Spawners Setup")]
    [SerializeField] private BaseSpawner[] spawners;

    private void Start()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag(GameConstants.PLAYER_TAG);
        }

        if (player == null)
        {
            return;
        }

        VitalsController vitals = player.GetComponent<VitalsController>();
        InventoryPlayer inventory = player.GetComponentInChildren<InventoryPlayer>();
        PlayerCombat combat = player.GetComponent<PlayerCombat>();
        PlayerUpgrades upgrades = player.GetComponent<PlayerUpgrades>();

        if (vitals == null || inventory == null || combat == null)
        {
            return;
        }

        
        if (UIManager.Instance != null)
        {
            UIManager.Instance.StartGameSequence(inventory, vitals, combat);
        }

        SaveData curr = SaveManager.Instance.currentSave;

        vitals.Init();
        inventory.Init(curr.grenadeCount, curr.medikitCount);

        StartCoroutine(InitCombatWithAddressables(combat));

        if (DifficultyManager.Instance != null)
        {
            DifficultyManager.Instance.InitManager();
        }

        if (spawners == null || spawners.Length == 0)
        {
            spawners = FindObjectsOfType<BaseSpawner>();
        }

        foreach (BaseSpawner spawner in spawners)
        {
            if (spawner != null)
            {
                spawner.InitSpawner();
            }
        }
    }

    private System.Collections.IEnumerator InitCombatWithAddressables(PlayerCombat combat)
    {
        Transform weaponHolder = combat.transform.Find("CameraHolder/FPSCamera/WeaponHolder");
        if (weaponHolder == null)
        {
            weaponHolder = combat.transform.GetComponentInChildren<CameraRecoil>()?.transform.Find("WeaponHolder");
        }

        if (weaponHolder == null)
        {
            GunBase[] allGuns = combat.GetComponentsInChildren<GunBase>(true);
            if (allGuns.Length > 0) weaponHolder = allGuns[0].transform.parent;
        }

        if (weaponHolder != null)
        {
            GunBase startingGun = weaponHolder.GetComponentInChildren<GunBase>();
            if (startingGun != null)
            {
                string weaponName = startingGun.gameObject.name.Replace("(Clone)", "").Replace(" base", "").Trim();
                int level = 1;

                if (SaveManager.Instance != null && SaveManager.Instance.currentSave != null && SaveManager.Instance.currentSave.weaponUpgrades != null)
                {
                    WeaponUpgradesSave upgrades = SaveManager.Instance.currentSave.weaponUpgrades;
                    if (weaponName.Equals("Pistola", System.StringComparison.OrdinalIgnoreCase) || weaponName.Equals("Pistol", System.StringComparison.OrdinalIgnoreCase))
                    {
                        level = upgrades.pistolLevel;
                        weaponName = "Pistol";
                    }
                    else if (weaponName.Equals("SMG", System.StringComparison.OrdinalIgnoreCase))
                    {
                        level = upgrades.smgLevel;
                        weaponName = "SMG";
                    }
                    else if (weaponName.Equals("Railgun", System.StringComparison.OrdinalIgnoreCase))
                    {
                        level = upgrades.railgunLevel;
                        weaponName = "Railgun";
                    }
                }

                string addressablePath = $"Assets/CuteRush kit/Presets/Weapons/Specs/{weaponName} Preset {level}.asset";
                UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<GunStats> statsHandle = UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<GunStats>(addressablePath);
                yield return statsHandle;

                if (statsHandle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
                {
                    startingGun.stats = statsHandle.Result;
                }
                else
                {
                    Debug.LogWarning($"GameBootstrapper Addressables failed to load weapon stats at {addressablePath}. Trying fallback.");
                    string fallbackPath = $"Assets/CuteRush kit/Presets/Weapons/Specs/{weaponName} Preset 1.asset";
                    UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<GunStats> fallbackHandle = UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<GunStats>(fallbackPath);
                    yield return fallbackHandle;
                    if (fallbackHandle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
                    {
                        startingGun.stats = fallbackHandle.Result;
                    }
                }
            }
        }

        combat.Init();
    }
}