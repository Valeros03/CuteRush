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

// EREDITIAMO DA InteractableItem! Così la piattaforma gestisce l'UI in automatico.
public class WeaponSpawner : InteractableItem
{
    [Header("Configurazione Piattaforma")]
    public float respawnTime = 90f;
    public Transform spawnPoint;
    public List<WeaponSpawnData> availableWeapons;

    private GameObject currentHologram;
    private WeaponSpawnData currentWeaponData;

    // Ci dice se c'è un ologramma pronto per essere raccolto
    private bool isReady = false;

    void Start()
    {
        // All'avvio del gioco, prepariamo subito il primo ologramma
        PrepareNewWeapon();
    }

    private void PrepareNewWeapon()
    {
        currentWeaponData = ChooseNextWeapon();

        // Facciamo apparire l'ologramma
        if (currentWeaponData.hologramPrefab != null)
        {
            currentHologram = Instantiate(currentWeaponData.hologramPrefab, spawnPoint.position, spawnPoint.rotation);
            currentHologram.transform.SetParent(transform);
        }

        isReady = true;
    }

    // QUESTA È LA TUA FUNZIONE ASTRATTA! Viene chiamata quando il player preme il tasto di interazione.
    public override void Interact()
    {
        // Se non c'è nessuna arma pronta (es. timer in corso), non fare nulla
        if (!isReady) return;

        // 1. Diamo l'arma vera al player
        GiveWeaponToPlayer(currentWeaponData.realWeaponPrefab);

        // 2. Distruggiamo l'ologramma
        if (currentHologram != null)
        {
            Destroy(currentHologram);
        }

        // 3. Nascondiamo l'UI "Interagisci" (dato che abbiamo appena raccolto l'arma)
        HideInteraction();

        // 4. La piattaforma entra in cooldown (90 secondi)
        isReady = false;
        StartCoroutine(RespawnSequence());
    }

    private IEnumerator RespawnSequence()
    {
        // Aspettiamo 90 secondi (lascialo a 5 per fare i test veloci!)
        yield return new WaitForSeconds(respawnTime);

        // Finito il tempo, prepariamo un nuovo ologramma
        PrepareNewWeapon();
    }

    private WeaponSpawnData ChooseNextWeapon()
    {
        // Cerchiamo il player nella scena
        PlayerController player = FindObjectOfType<PlayerController>();
        string currentWeaponName = player != null ? player.GetCurrentWeaponName() : "";

        // Creiamo una lista vuota in cui metteremo solo le armi DIVERSE da quella attuale
        List<WeaponSpawnData> filteredWeapons = new List<WeaponSpawnData>();

        foreach (var weaponData in availableWeapons)
        {
            // Se l'arma corrente non è quella che stiamo analizzando, la aggiungiamo alla lista valida
            if (weaponData.realWeaponPrefab.name != currentWeaponName)
            {
                filteredWeapons.Add(weaponData);
            }
        }

        // Controllo di sicurezza: se per caso c'è solo un'arma nel gioco, 
        // usiamo la lista originale per non generare errori
        if (filteredWeapons.Count == 0)
        {
            filteredWeapons = availableWeapons;
        }

        // Peschiamo a caso dalla lista filtrata!
        int randomIndex = Random.Range(0, filteredWeapons.Count);
        return filteredWeapons[randomIndex];
    }

    private void GiveWeaponToPlayer(GameObject realWeaponPrefab)
    {
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            // Richiamiamo il nuovo metodo che abbiamo appena creato!
            player.EquipWeapon(realWeaponPrefab);
        }
    }

    // SOVRASCRIVIAMO I TRIGGER: Mostriamo l'UI "Interagire" SOLO se c'è un ologramma pronto
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