using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractableItem : MonoBehaviour, Interactable
{

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
        UIManager.Instance.ShowInteract("Interagire");
    }

    public virtual void HideInteraction()
    {
        UIManager.Instance.HideInteract();
    }
    public abstract void Interact();

  
}
