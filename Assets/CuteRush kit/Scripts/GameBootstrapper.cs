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
        upgrades.Init(curr.playerUpgrades);
        inventory.Init(curr.grenadeCount, curr.medikitCount);
        combat.Init();

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
}