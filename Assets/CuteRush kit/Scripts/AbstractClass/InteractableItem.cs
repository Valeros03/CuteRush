using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractableItem : MonoBehaviour, Interactable
{
    public string interactText = "Interagisci";
    [Tooltip("Se falso, l'oggetto non reagirà al giocatore")]
    public bool isInteractable = true;

    public virtual void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            ShowInteraction();
        }
    }
    public virtual void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            HideInteraction();
        }

    }

    public virtual void ShowInteraction()
    {
        UIEvents.TriggerInteract(interactText);
    }

    public virtual void HideInteraction()
    {
        UIEvents.TriggerHideInteract();
    }
    public abstract void Interact(PlayerInteraction player = null);

  
}
