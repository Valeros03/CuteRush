using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
public class PlayerInteraction : MonoBehaviour
{
    [Header("Riferimenti")]
    [SerializeField] private InventoryPlayer inventory;

    private PlayerInput input;
    private VitalsController vitals;
    private AudioPlayerController audioPlayer;
    private PlayerCombat combat;

    private InteractableItem currentInteractable;

    private void Awake()
    {
        input = GetComponent<PlayerInput>();
        vitals = GetComponent<VitalsController>();
        audioPlayer = GetComponent<AudioPlayerController>();
        combat = GetComponent<PlayerCombat>();
    }

    private void OnEnable()
    {
        input.OnInteract += HandleInteract;
        input.OnHeal += HandleHeal;
    }

    private void OnDisable()
    {
        input.OnInteract -= HandleInteract;
        input.OnHeal -= HandleHeal;
    }

    private void HandleInteract()
    {
        if (currentInteractable != null)
        {
            currentInteractable.Interact(this);
        }
    }

    private void HandleHeal()
    {
        if (vitals != null && inventory != null)
        {
            if (vitals.currentHealth < vitals.maxHealth && inventory.removeMedkit())
            {
                vitals.UseMedikit();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        InteractableItem interactable = other.GetComponent<InteractableItem>();
        if (interactable != null)
        {
            currentInteractable = interactable;
        }

        IPickable pickable = other.GetComponent<IPickable>();
        if (pickable != null)
        {
            pickable.Pickup(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        InteractableItem interactable = other.GetComponent<InteractableItem>();
        if (interactable != null && interactable == currentInteractable)
        {
            currentInteractable = null;
        }
    }


    public void addGold(int value)
    {
        if (audioPlayer != null) audioPlayer.PlayGoldSound();
        if (inventory != null) inventory.addCoin(value);
    }

    public bool addMedkit()
    {
        if (audioPlayer != null) audioPlayer.PlayPickupSound();
        if (inventory != null) return inventory.addMedkit();
        return false;
    }

    public bool addGrenade()
    {
        if (audioPlayer != null) audioPlayer.PlayPickupSound();
        if (inventory != null) return inventory.addGrenade();
        return false;
    }

    public bool addAmmo()
    {
        if (audioPlayer != null) audioPlayer.PlayPickupSound();
        if (combat != null) return combat.AddAmmoToGun();
        return false;
    }
    public bool removeGrenade()
    {
        if (inventory != null)
        {
            return inventory.removeGrenade();
        }
        return false;
    }

    public bool UseAcidoBorico()
    {
        if (inventory != null)
        {
            return inventory.UseAcidoBorico();
        }
        return false;
    }
}