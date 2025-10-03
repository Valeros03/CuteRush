using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;


namespace TheDeveloperTrain.SciFiGuns
{

    public class Gun : MonoBehaviour
    {
        /// <summary>
        /// The particle systems for the gun, if any
        /// </summary>
        [Tooltip("The particle systems for the gun, if any")]
        public ParticleSystem[] gunParticleSystems;

        /// <summary>
        /// The Transform point of the muzzle, AKA where the bullet prefab is spawned. the bullet inherits its rotation as well
        /// </summary>
        [Tooltip("The Transform point of the muzzle, AKA where the bullet prefab is spawned. the bullet inherits its rotation as well")]
        [SerializeField] private Transform muzzleTransform;

        public GunStats stats;

        /// <summary>
        /// The number of bullets currently left, before the gun has to reload
        /// </summary>
        [HideInInspector] public int currentBulletCount;
        private int currentMagLeft;

        [HideInInspector] public bool isReloading = false;
        public bool IsInShotCooldown { get; private set; } = false;

        /// <summary>
        /// Called when the bullet is actually created, AKA after the shoot delay.
        /// </summary>
        public Action onBulletShot;

        public Action onLastBulletShotInBurst;

        public Action onGunReloadStart;

        /// <summary>
        /// Called as soon as the gun starts it's shooting procedure, if the gun is ready to be fired
        /// </summary>
        public Action onGunShootingStart;

        [Header("Settings")]
        public int damage = 10;
        public float range = 100f;           // distanza massima
        public Transform firePoint;          // punto da cui parte il raycast
        public LayerMask hitLayers;          // layer colpibili (es. Enemy)

        [Header("Effects")]
        public LineRenderer tracer;          // opzionale: linea visiva
        public float tracerDuration = 0.05f; // tempo in cui la linea resta visibile
        private float tracerTimer;

        //UI Settings
        private GameObject WeaponUI;
        private Text bulletNumberUI;

        void Start()
        {
            currentBulletCount = stats.magazineSize;
            currentMagLeft = stats.totalAmmo;

            if (WeaponUI == null)
            {
                WeaponUI = GameObject.Find("Canvas/WeaponUI");
                Debug.Log(WeaponUI);
            }

            if (bulletNumberUI == null && WeaponUI != null)
            {
                bulletNumberUI = WeaponUI.transform.Find("Ammo").GetComponent<Text>();
            }
        }

        void Update()
        {
            if (WeaponUI.activeSelf)
            {
                bulletNumberUI.text = string.Format("Bullets {0}/{1}", currentBulletCount, currentMagLeft);
              
            }

            if (tracer != null && tracer.enabled)
            {
                tracerTimer -= Time.deltaTime;
                if (tracerTimer <= 0f)
                {
                    tracer.enabled = false;
                }
            }
        }

        public void Shoot()
        {
            if(!gameObject.activeSelf) { return; }

            if (currentBulletCount > 0 && !isReloading && !IsInShotCooldown)
            {
                IsInShotCooldown = true;
                onGunShootingStart?.Invoke();
                foreach (var particleSystem in gunParticleSystems)
                {
                    particleSystem.Play();
                }
                if (stats.fireMode == FireMode.Single)
                {
                    currentBulletCount--;

                    Invoke(nameof(SpawnBullet), stats.shootDelay);
                    StartCoroutine(nameof(ResetGunShotCooldown));


                    if (currentBulletCount == 0)
                    {
                        Reload();
                    }
                }
                else if (stats.fireMode == FireMode.Burst)
                {
                    StartCoroutine(nameof(FireBulletsInBurst));
                }
            }

        }

        private void SpawnBullet()
        {
            RaycastHit hit;


            // spara un raggio davanti
            if (Physics.Raycast(firePoint.position, firePoint.forward * -1, out hit, range, hitLayers))
            {
                

                // se il nemico ha uno script con TakeDamage
                EnemyAi enemy = hit.collider.GetComponentInParent<EnemyAi>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage);
                }

                // disegna il tracer fino al punto colpito
                if (tracer != null)
                {
                   
                    tracer.enabled = true;
                    tracer.SetPosition(0, firePoint.position);
                    tracer.SetPosition(1, hit.point);
                    tracerTimer = tracerDuration;
                }
            }
            else
            {
                // tracer che va "in aria" se non colpisce nulla
                if (tracer != null)
                {
                    
                    tracer.enabled = true;
                    tracer.SetPosition(0, firePoint.position);
                    tracer.SetPosition(1, firePoint.position + firePoint.forward*-1 * range);
                    tracerTimer = tracerDuration;
                }
            }
            onBulletShot?.Invoke();
        }

        public void Reload()
        {
            if (!gameObject.activeSelf) { return; }
            StartCoroutine(nameof(ReloadGun));
        }

        private IEnumerator ReloadGun()
        {
            if (!isReloading)
            {
                onGunReloadStart?.Invoke();
                isReloading = true;
                yield return new WaitForSeconds(stats.reloadDuration);
                if (currentMagLeft != 0)
                {
                    if (currentMagLeft - (stats.magazineSize - currentBulletCount) >= 0)
                    {
                        currentMagLeft -= (stats.magazineSize - currentBulletCount);
                        currentBulletCount = stats.magazineSize;
                    }
                    else
                    {
                        currentBulletCount += currentMagLeft;
                        currentMagLeft = 0;
                    }
                }
                isReloading = false;
            }
        }
        private IEnumerator ResetGunShotCooldown()
        {
            yield return new WaitForSeconds(1 / stats.fireRate - stats.shootDelay);
            IsInShotCooldown = false;
        }
        private IEnumerator FireBulletsInBurst()
        {
            yield return new WaitForSeconds(stats.shootDelay);

            for (int i = 0; i < stats.burstCount; i++)
            {
                SpawnBullet();
                currentBulletCount--;
                if (currentBulletCount == 0)
                {
                    Reload();
                    break;
                }
                onBulletShot?.Invoke();
                yield return new WaitForSeconds(stats.burstInterval);

            }
            onLastBulletShotInBurst?.Invoke();
            yield return new WaitForSeconds(1 / stats.fireRate - (stats.shootDelay + (stats.burstCount * stats.burstInterval)));
            IsInShotCooldown = false;
        }

    }

}