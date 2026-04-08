using UnityEngine;
using TheDeveloperTrain.SciFiGuns;
using System;
using UnityEngine.AddressableAssets;

[RequireComponent(typeof(PlayerInput))]
public class PlayerCombat : MonoBehaviour
{
    [Header("Equipaggiamento")]
    [SerializeField] private GameObject weaponHolder;
    [SerializeField] private GameObject granade;

    [Header("Animatori e Inventario")]
    [SerializeField] private Animator granadeAnimator;
    [SerializeField] private InventoryPlayer inventory;

    private PlayerInput input;
    private Animator weaponAnimator;
    private GunBase gun;
    private GrandeThrower thrower;

    public event Action<int, int> OnActiveWeaponAmmoChanged;

    private bool isInitialized = false;

    public void Init()
    {
        input = GetComponent<PlayerInput>();

        if (granade != null)
        {
            thrower = granade.GetComponent<GrandeThrower>();
        }

        Transform fpsCam = transform.Find(GameConstants.TRANSFORM_CAMERA_HOLDER)?.Find(GameConstants.TRANSFORM_FPS_CAMERA);
        if (fpsCam != null)
        {
            weaponAnimator = fpsCam.Find(GameConstants.TRANSFORM_WEAPON_HOLDER)?.GetComponentInChildren<Animator>();
        }

        if (weaponHolder != null)
        {
            StartCoroutine(InitWeaponRoutine());
        }
        else
        {
            isInitialized = true;
            if (enabled) SubscribeInputs();
        }
    }

    private System.Collections.IEnumerator InitWeaponRoutine()
    {
        yield return Addressables.InitializeAsync();

        string selectedWeaponName = PlayerPrefs.GetString("Weapon", GameConstants.WEAPON_PISTOL);
        string prefabKey = $"{selectedWeaponName}";

        foreach (Transform child in weaponHolder.transform)
        {
            Destroy(child.gameObject);
        }

        UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<GameObject> weaponHandle = Addressables.InstantiateAsync(prefabKey, weaponHolder.transform);
        yield return weaponHandle;

        if (weaponHandle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
        {
            GameObject readyWeapon = weaponHandle.Result;
            readyWeapon.transform.localPosition = Vector3.zero;
            readyWeapon.transform.localRotation = Quaternion.identity;

            weaponAnimator = readyWeapon.GetComponent<Animator>();
            GunBase startingGun = readyWeapon.GetComponent<GunBase>();

            if (startingGun != null)
            {
                int level = 1;

                if (SaveManager.Instance != null && SaveManager.Instance.currentSave != null && SaveManager.Instance.currentSave.weaponUpgrades != null)
                {
                    WeaponUpgradesSave upgrades = SaveManager.Instance.currentSave.weaponUpgrades;
                    if (selectedWeaponName == GameConstants.WEAPON_PISTOL)
                    {
                        level = upgrades.pistolLevel;
                    }
                    else if (selectedWeaponName == GameConstants.WEAPON_SMG)
                    {
                        level = upgrades.smgLevel;
                    }
                    else if (selectedWeaponName == GameConstants.WEAPON_RAILGUN)
                    {
                        level = upgrades.railgunLevel;
                    }
                }

                string addressableKey = $"{selectedWeaponName} Preset {level}";
                UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<GunStats> statsHandle = default;
                bool pathIsValid = true;

                try
                {
                    statsHandle = Addressables.LoadAssetAsync<GunStats>(addressableKey);
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"[PlayerCombat] Errore critico Addressables sulla chiave '{addressableKey}': {e.Message}");
                    pathIsValid = false;
                }

                if (pathIsValid)
                {
                    yield return statsHandle;

                    if (statsHandle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
                    {
                        startingGun.stats = statsHandle.Result;
                    }
                    else
                    {
                        Debug.LogWarning($"PlayerCombat Addressables failed to load weapon stats at {addressableKey}. Trying fallback.");

                        string fallbackKey = $"{selectedWeaponName} Preset 1";
                        UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<GunStats> fallbackHandle = default;
                        bool fallbackPathIsValid = true;

                        try
                        {
                            fallbackHandle = Addressables.LoadAssetAsync<GunStats>(fallbackKey);
                        }
                        catch (System.Exception e)
                        {
                            Debug.LogError($"[PlayerCombat] Fallita l'inizializzazione del fallback '{fallbackKey}': {e.Message}");
                            fallbackPathIsValid = false;
                        }

                        if (fallbackPathIsValid)
                        {
                            yield return fallbackHandle;

                            if (fallbackHandle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
                            {
                                startingGun.stats = fallbackHandle.Result;
                            }
                            else
                            {
                                Debug.LogError($"[PlayerCombat] Fallito anche il caricamento effettivo del fallback '{fallbackKey}'!");
                            }
                        }
                    }
                }

                ConnectGun(startingGun);
                startingGun.Init(this);
                startingGun.enabled = true;

                Transform tracer = startingGun.transform.Find(GameConstants.TRANSFORM_TRACER);
                if (tracer != null) tracer.gameObject.SetActive(true);

                CameraRecoil camRecoil = transform.GetComponentInChildren<CameraRecoil>();
                RecoilController newRecoilCtrl = startingGun.GetComponentInChildren<RecoilController>();
                if (camRecoil != null && newRecoilCtrl != null)
                {
                    camRecoil.SetNewWeapon(startingGun, newRecoilCtrl);
                }
            }
        }
        else
        {
            Debug.LogError($"Failed to load weapon prefab from Addressables at {prefabKey}");
        }

        isInitialized = true;
        if (enabled) SubscribeInputs();
    }

    private void OnEnable()
    {
        if (isInitialized) SubscribeInputs();
    }

    private void OnDisable()
    {
        if (isInitialized) UnsubscribeInputs();
    }

    private void SubscribeInputs()
    {
        if (input == null) return;
        input.OnReload += HandleReload;
        input.OnFireStart += HandleFireStart;
        input.OnFireCancel += HandleFireCancel;
        input.OnEquipGrenade += HandleGrenadeEquip;
    }

    private void UnsubscribeInputs()
    {
        if (input == null) return;
        input.OnReload -= HandleReload;
        input.OnFireStart -= HandleFireStart;
        input.OnFireCancel -= HandleFireCancel;
        input.OnEquipGrenade -= HandleGrenadeEquip;
    }

    private void OnDestroy()
    {
        UnsubscribeInputs();
        if (gun != null) gun.OnAmmoChanged -= HandleWeaponAmmoChanged;
    }

    private void Update()
    {
        if (!isInitialized) return;

        if (input.IsFiring && gun != null && gun.gameObject.activeInHierarchy)
        {
            if (gun.stats.fireMode == FireMode.Auto)
            {
                gun.TryShoot();
            }
        }
    }

    private void ConnectGun(GunBase newGun)
    {
        if (gun != null) gun.OnAmmoChanged -= HandleWeaponAmmoChanged;

        gun = newGun;

        if (gun != null) gun.OnAmmoChanged += HandleWeaponAmmoChanged;
    }

    private void HandleWeaponAmmoChanged(int currentAmmo, int totalAmmo)
    {
        OnActiveWeaponAmmoChanged?.Invoke(currentAmmo, totalAmmo);
    }

    private void HandleReload()
    {
        if (gun != null && gun.gameObject.activeInHierarchy) gun.StartReload();
    }

    private void HandleFireStart()
    {
        if (granade != null && granade.activeInHierarchy && thrower != null)
        {
            thrower.Activation();
        }
        else if (gun != null && gun.gameObject.activeInHierarchy)
        {
            if (gun.stats.fireMode == FireMode.Single || gun.stats.fireMode == FireMode.charge)
            {
                gun.TryShoot();
            }
        }
    }

    private void HandleFireCancel()
    {
        if (granade != null && granade.activeInHierarchy && thrower != null)
        {
            thrower.ThrowGrenade();
            SwitchToWeapon();
        }
        else if (gun != null && gun.gameObject.activeInHierarchy)
        {
            if (gun.stats.fireMode == FireMode.Single) gun.ResetSingleShot();
        }
    }

    private void HandleGrenadeEquip()
    {
        if (inventory == null || inventory.getGrenadeCount() <= 0) return;

        if (granade != null && !granade.activeSelf)
        {
            if (weaponAnimator != null && gun != null)
            {
                gun.enabled = false;
                Transform tracer = gun.transform.Find(GameConstants.TRANSFORM_TRACER);
                if (tracer != null) tracer.gameObject.SetActive(false);
                weaponAnimator.SetTrigger(GameConstants.ANIM_POSA_ARMA);
            }
        }
        else
        {
            if (granadeAnimator != null) granadeAnimator.SetTrigger(GameConstants.ANIM_POSA_GRANATA);
        }
    }

    public void SwitchToGranade()
    {
        if (gun != null && gun.transform.childCount > 0)
        {
            gun.transform.GetChild(0).gameObject.SetActive(false);
        }

        if (granade != null)
        {
            granade.SetActive(true);
            if (granadeAnimator != null)
            {
                granade.transform.SetParent(granade.GetComponentInParent<Transform>());
                granade.transform.localPosition = new Vector3(-0.352f, -0.664f, 0.011f);
                granade.transform.localRotation = Quaternion.identity;
                granadeAnimator.SetTrigger(GameConstants.ANIM_PRENDI_GRANATA);
            }

            if (input != null && input.IsFiring && thrower != null)
            {
                thrower.Activation();
            }
        }
    }

    public void SwitchToWeapon()
    {
        if (granade != null)
        {
            Transform gMesh = granade.transform.Find(GameConstants.TRANSFORM_GRANADE);
            if (gMesh != null) gMesh.gameObject.SetActive(true);
            granade.SetActive(false);
        }

        if (gun != null)
        {
            gun.enabled = true;
            Transform tracer = gun.transform.Find(GameConstants.TRANSFORM_TRACER);
            if (tracer != null) tracer.gameObject.SetActive(true);

            if (gun.transform.childCount > 0)
                gun.transform.GetChild(0).gameObject.SetActive(true);

            if (weaponAnimator != null) weaponAnimator.SetTrigger(GameConstants.ANIM_EQUIP_ARMA);
        }
    }

    public void EquipWeapon(GameObject readyWeapon)
    {
        if (granade != null && granade.activeInHierarchy) SwitchToWeapon();

        if (gun != null)
        {
            gun.OnAmmoChanged -= HandleWeaponAmmoChanged;
            if (!Addressables.ReleaseInstance(gun.gameObject))
            {
                Destroy(gun.gameObject);
            }
        }

        readyWeapon.transform.SetParent(weaponHolder.transform, false);
        readyWeapon.transform.localPosition = Vector3.zero;
        readyWeapon.transform.localRotation = Quaternion.identity;
        readyWeapon.transform.localScale = Vector3.one;
        readyWeapon.SetActive(true);

        weaponAnimator = readyWeapon.GetComponent<Animator>();
        GunBase newGunComponent = readyWeapon.GetComponent<GunBase>();

        if (newGunComponent != null)
        {
            ConnectGun(newGunComponent);
            newGunComponent.Init(this);

            newGunComponent.enabled = true;
            Transform tracer = newGunComponent.transform.Find(GameConstants.TRANSFORM_TRACER);
            if (tracer != null) tracer.gameObject.SetActive(true);
        }

        CameraRecoil camRecoil = transform.GetComponentInChildren<CameraRecoil>();
        RecoilController newRecoilCtrl = readyWeapon.GetComponentInChildren<RecoilController>();

        if (camRecoil != null && newRecoilCtrl != null)
        {
            camRecoil.SetNewWeapon(gun, newRecoilCtrl);
        }
    }

    public string GetCurrentWeaponName()
    {
        if (gun != null) return gun.gameObject.name.Replace("(Clone)", "").Trim();
        return "Nessuna Arma";
    }

    public bool AddAmmoToGun()
    {
        if (gun != null) return gun.addMag();
        return false;
    }
}