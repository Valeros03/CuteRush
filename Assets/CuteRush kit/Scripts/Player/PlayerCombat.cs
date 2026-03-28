using UnityEngine;
using TheDeveloperTrain.SciFiGuns;

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

    private void Awake()
    {
        input = GetComponent<PlayerInput>();

        if (weaponHolder != null)
        {
            gun = weaponHolder.GetComponentInChildren<GunBase>();
        }

        if (granade != null)
        {
            thrower = granade.GetComponent<GrandeThrower>();
        }
    }

    private void Start()
    {
        Transform fpsCam = transform.Find("CameraHolder")?.Find("FPSCamera");
        if (fpsCam != null)
        {
            weaponAnimator = fpsCam.Find("WeaponHolder")?.GetComponentInChildren<Animator>();
        }
    }

    private void OnEnable()
    {
        input.OnReload += HandleReload;
        input.OnFireStart += HandleFireStart;
        input.OnFireCancel += HandleFireCancel;
        input.OnEquipGrenade += HandleGrenadeEquip;
    }

    private void OnDisable()
    {
        input.OnReload -= HandleReload;
        input.OnFireStart -= HandleFireStart;
        input.OnFireCancel -= HandleFireCancel;
        input.OnEquipGrenade -= HandleGrenadeEquip;
    }

    private void Update()
    {
        if (input.IsFiring && gun != null && gun.gameObject.activeInHierarchy)
        {
            if (gun.stats.fireMode == FireMode.Auto)
            {
                gun.TryShoot();
            }
        }
    }

    


    private void HandleReload()
    {
        if (gun != null && gun.gameObject.activeInHierarchy)
        {
            gun.StartReload();
        }
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
            if (gun.stats.fireMode == FireMode.Single)
            {
                gun.ResetSingleShot();
            }
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

                Transform tracer = gun.transform.Find("Tracer");
                if (tracer != null) tracer.gameObject.SetActive(false);

                weaponAnimator.SetTrigger("PosaArma");
            }
        }
        else
        {
            if (granadeAnimator != null)
            {
                granadeAnimator.SetTrigger("PosaGranata");
            }
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
                granadeAnimator.SetTrigger("PrendiGranata");
            }
        }
    }

    public void SwitchToWeapon()
    {
        if (granade != null)
        {
            Transform gMesh = granade.transform.Find("Granade");
            if (gMesh != null) gMesh.gameObject.SetActive(true);
            granade.SetActive(false);
        }

        if (gun != null)
        {
            gun.enabled = true;
            Transform tracer = gun.transform.Find("Tracer");
            if (tracer != null) tracer.gameObject.SetActive(true);

            if (gun.transform.childCount > 0)
                gun.transform.GetChild(0).gameObject.SetActive(true);

            if (weaponAnimator != null)
            {
                weaponAnimator.SetTrigger("EquipaggiaArma");
            }
        }
    }

    public void EquipWeapon(GameObject newWeaponPrefab)
    {
        if (granade != null && granade.activeInHierarchy)
        {
            SwitchToWeapon();
        }

        if (gun != null)
        {
            Destroy(gun.gameObject);
        }

        GameObject newWeapon = Instantiate(newWeaponPrefab, weaponHolder.transform);
        newWeapon.transform.localPosition = Vector3.zero;
        newWeapon.transform.localRotation = Quaternion.identity;

        gun = newWeapon.GetComponent<GunBase>();
        weaponAnimator = newWeapon.GetComponent<Animator>();

        if (gun != null)
        {
            gun.enabled = true;
            Transform tracer = gun.transform.Find("Tracer");
            if (tracer != null) tracer.gameObject.SetActive(true);
        }

        CameraRecoil camRecoil = transform.GetComponentInChildren<CameraRecoil>();
        RecoilController newRecoilCtrl = newWeapon.GetComponentInChildren<RecoilController>();

        if (camRecoil != null && newRecoilCtrl != null)
        {
            camRecoil.SetNewWeapon(gun, newRecoilCtrl);
        }
    }

    public string GetCurrentWeaponName()
    {
        if (gun != null)
        {
            return gun.gameObject.name.Replace("(Clone)", "").Trim();
        }
        return "Nessuna Arma";
    }

    public bool AddAmmoToGun()
    {
        if (gun != null) return gun.addMag();
        return false;
    }
}