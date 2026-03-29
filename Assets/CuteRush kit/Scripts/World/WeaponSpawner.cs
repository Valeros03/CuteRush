using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WeaponSpawnData
{
    public string weaponName;
    public GameObject realWeaponPrefab;
    public GameObject hologramPrefab;
}

public class WeaponSpawner : InteractableItem
{
    [Header("Configurazione Piattaforma")]
    public float respawnTime = 90f;
    public Transform spawnPoint;
    public List<WeaponSpawnData> availableWeapons;

    private GameObject currentHologram;
    private WeaponSpawnData currentWeaponData;

    private bool isReady = false;

    void Start()
    {
        PrepareNewWeapon();
    }

    private void PrepareNewWeapon()
    {
        currentWeaponData = ChooseNextWeapon();

        if (currentWeaponData.hologramPrefab != null)
        {
            currentHologram = Instantiate(currentWeaponData.hologramPrefab, spawnPoint.position, spawnPoint.rotation);
            currentHologram.transform.SetParent(transform);
        }

        isReady = true;
    }

    public override void Interact(PlayerInteraction player = null)
    {
        if (!isReady) return;


        GiveWeaponToPlayer(currentWeaponData.realWeaponPrefab);

        if (currentHologram != null)
        {
            Destroy(currentHologram);
        }

        HideInteraction();

        isReady = false;
        StartCoroutine(RespawnSequence());
    }

    private IEnumerator RespawnSequence()
    {
        yield return new WaitForSeconds(respawnTime);

        PrepareNewWeapon();
    }

    private WeaponSpawnData ChooseNextWeapon()
    {

        PlayerCombat combat = FindObjectOfType<PlayerCombat>();
        string currentWeaponName = combat != null ? combat.GetCurrentWeaponName() : "";

        List<WeaponSpawnData> filteredWeapons = new List<WeaponSpawnData>();

        foreach (var weaponData in availableWeapons)
        {
    
            if (weaponData.realWeaponPrefab.name != currentWeaponName)
            {
                filteredWeapons.Add(weaponData);
            }
        }

        if (filteredWeapons.Count == 0)
        {
            filteredWeapons = availableWeapons;
        }

        int randomIndex = Random.Range(0, filteredWeapons.Count);
        return filteredWeapons[randomIndex];
    }

    private void GiveWeaponToPlayer(GameObject realWeaponPrefab)
    {
        PlayerCombat combat = FindObjectOfType<PlayerCombat>();
        if (combat != null)
        {
            combat.EquipWeapon(realWeaponPrefab);
        }
    }

    public override void OnTriggerEnter(Collider other)
    {
        if (isReady && other.gameObject.CompareTag("Player"))
        {
            ShowInteraction();
        }
    }

    public override void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            HideInteraction();
        }
    }
}