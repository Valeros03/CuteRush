using UnityEngine;

public enum FireMode
{
    Single = 0,
    Auto = 1,
    charge = 2
}

[CreateAssetMenu(fileName = "NewGunStats", menuName = "Weapons/Gun Stats")]
public class GunStats : ScriptableObject
{
    [Header("Ammo Settings")]
    [Tooltip("The amount of bullets you carry entirely, this is where ammo is reduced from when reloading")]
    public int totalAmmo = 100;
    [Tooltip("The max amount of bullets that can be loaded in the gun at any given time.")]
    public int magazineSize = 10;

    [Header("Damage & Range")]
    public float damage = 0f;
    public float range = 0f;

    [Header("Timings & Speed")]
    [Tooltip("In seconds, the time it takes for the gun to reload.")]
    public float reloadDuration = 6f;

    [Tooltip("In seconds, the time it takes between when the Shoot() function of a given gun is called and when a bullet actually leaves the barrel.")]
    public float shootDelay = 0.1f;

    [Tooltip("In meters per second, the speed of the projectile the gun shoots.")]
    public float bulletSpeed = 200f;

    [Tooltip("In shots per second (Bursts per second if using burst mode). FireRate includes both the time it takes for a gun to charge as well as to discharge.")]
    public float fireRate = 1f;

    [Header("Fire Mode Settings")]
    [Tooltip("The type of shooting the gun will use. Single is one shot and then cooldown, Burst is a few shots fired closely together and then cooldown")]
    public FireMode fireMode = FireMode.Single;

    [Header("Bloom & Precisione")]
    [Tooltip("La dimensione base del Bloom (spread) dell'arma.")]
    public float baseBloomSpread = 0.01f;

    [Header("Crosshair Dynamic Bloom")]
    [Tooltip("L'ammontare di kick (allargamento) visivo della crosshair per ogni sparo.")]
    public float crosshairShootBloomKick = 50.0f;

    [Header("Charge Gun Settings")]
    public float sphereRadius = 0.3f;
    public float penetrationFalloff = 0.75f;
    public int maxPenetrations = 5;

    private void OnValidate()
    {
        totalAmmo = Mathf.Max(0, totalAmmo);
        magazineSize = Mathf.Max(1, magazineSize);
        reloadDuration = Mathf.Max(0f, reloadDuration);
        shootDelay = Mathf.Max(0f, shootDelay);
        bulletSpeed = Mathf.Max(0f, bulletSpeed);

        fireRate = Mathf.Max(0.0001f, fireRate);

        float fireCycleTime = 1 / fireRate;

        if (shootDelay >= fireCycleTime)
        {
            shootDelay = fireCycleTime - 0.0001f;
        }
        sphereRadius = Mathf.Max(0.01f, sphereRadius);
        penetrationFalloff = Mathf.Clamp01(penetrationFalloff);
        maxPenetrations = Mathf.Max(1, maxPenetrations);
    }
}