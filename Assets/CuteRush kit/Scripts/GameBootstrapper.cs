using UnityEngine;

public class GameBootstrapper : MonoBehaviour
{
    [Header("Player Setup")]
    [SerializeField] private GameObject player;

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

        if (vitals == null || inventory == null || combat == null)
        {
            return;
        }

        
        if (UIManager.Instance != null)
        {
            UIManager.Instance.StartGameSequence(inventory, vitals, combat);
        }

        vitals.Init();
        inventory.Init();
        combat.Init();


    }
}