using System;
using System.Collections;
using UnityEditor;
using UnityEditor.Animations;
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
        public Action onGunReloadStart;

        /// <summary>
        /// Called as soon as the gun starts it's shooting procedure, if the gun is ready to be fired
        /// </summary>
        public Action onGunShootingStart;

        [Header("Settings")]
        public Transform firePoint;          // punto da cui parte il raycast
        public LayerMask hitLayers;          // layer colpibili (es. Enemy)

        [Header("Effects")]
        public LineRenderer tracer;          // opzionale: linea visiva
        public float tracerDuration = 0.05f; // tempo in cui la linea resta visibile
        private float tracerTimer;

        //UI Settings
        private GameObject WeaponUI;
        private Text bulletNumberUI;

        private PlayerController player;

        void Start()
        {
            currentBulletCount = stats.magazineSize;
            currentMagLeft = stats.totalAmmo;
            player = GetComponentInParent<PlayerController>();

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


        public void Shoot() { 

            if (!gameObject.activeSelf || !enabled) { return; }
            if (currentBulletCount > 0 && !isReloading && !IsInShotCooldown) 
            { 
                
                onGunShootingStart?.Invoke();


                foreach (var particleSystem in gunParticleSystems) { particleSystem.Play(); }

                IsInShotCooldown = true;
                currentBulletCount--;

                if (stats.fireMode == FireMode.Single) 
                {

                    
                    Invoke(nameof(SpawnBullet), stats.shootDelay);
                    StartCoroutine(nameof(ResetGunShotCooldown));
                    

                } else if (stats.fireMode == FireMode.Auto) 
                {
                    
                    StartCoroutine(nameof(FireBulletAuto));

                } else if (stats.fireMode == FireMode.charge)
                {
                    StartCoroutine(nameof(FireInCharge));

                }


            }else if (currentBulletCount <= 0)
            {
                Reload();
            }
        }

        private void SpawnBullet()
        {
            RaycastHit hit;
            Camera cam = Camera.main;
            Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); // centro dello schermo

            Vector3 targetPoint;
            gameObject.GetComponent<AudioGunController>().PlayShoot();
            
            if (Physics.Raycast(ray, out hit, stats.range, hitLayers))
            {
                targetPoint = hit.point;
            }
            else
            {
                targetPoint = ray.GetPoint(stats.range);
            }

            Vector3 direction = (targetPoint - firePoint.position).normalized;
            direction = Quaternion.Euler(UnityEngine.Random.Range(-0.5f, 0.5f), UnityEngine.Random.Range(-0.5f, 0.5f), 0) * direction;


            
            if (Physics.Raycast(firePoint.position, direction, out hit, stats.range, hitLayers))
            {
                EnemyAi enemy = hit.collider.GetComponentInParent<EnemyAi>();
                Vector3 shotDir = (hit.point - firePoint.position).normalized;
                if (enemy != null)
                {
                    enemy.TakeDamage(stats.damage, shotDir, hit.point);
                   
                }

                if (tracer != null)
                {
                    tracer.enabled = true;
                    Vector3 start = firePoint.position;
                    Vector3 offset = Vector3.zero;
                    if ((targetPoint - start).magnitude < 5f)
                        offset = (cam.transform.position - start).normalized * 0.1f; // sposta un po' il tracer
                    tracer.SetPosition(0, start + offset);
                    tracer.SetPosition(1, targetPoint);
                    tracerTimer = tracerDuration;
                }
            }
            else
            {
                if (tracer != null)
                {
                    tracer.enabled = true;
                    Vector3 start = firePoint.position;
                    Vector3 offset = Vector3.zero;
                    if ((targetPoint - start).magnitude < 5f)
                        offset = (cam.transform.position - start).normalized * 0.1f; // sposta un po' il tracer
                    tracer.SetPosition(0, start + offset);
                    tracer.SetPosition(1, targetPoint);
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
        private IEnumerator FireBulletAuto()
        {

            yield return new WaitForSeconds(1 / stats.fireRate - stats.shootDelay);
            SpawnBullet();
            IsInShotCooldown = false;
            
        }

        private IEnumerator FireInCharge()
        {

            yield return new WaitForSeconds(stats.shootDelay);
            SpawnBullet();
            StartCoroutine(nameof(ResetGunShotCooldown));

        }

        private void EquipGranade()
        {
            player.SwitchToGranade();
        }

    }

    

}