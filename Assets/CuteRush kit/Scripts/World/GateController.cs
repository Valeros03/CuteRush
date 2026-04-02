using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateController : InteractableItem
{
    [SerializeField] private Animator animator;
    [SerializeField] private AudioClip open;
    [SerializeField] private AudioClip close;
    [SerializeField] private AudioSource source;

    private bool bOpen;
    public bool isOpenable;
    public override void Interact(PlayerInteraction player = null)
    {
        toggleGate();
    }
    public override void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(GameConstants.PLAYER_TAG))
        {
            if(bOpen)
               interactText = "chiudere il cancello";
            else
                interactText = "aprire il cancello";

            ShowInteraction();
        }
    }

    public override void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag(GameConstants.PLAYER_TAG))
        {
            HideInteraction();
        }
    }

    public void CloseGate()
    {
        bOpen = false;
        animator.SetTrigger(GameConstants.ANIM_CLOSE);
    }
    


    private void toggleGate()
    {
        if (!isOpenable)
        {
            UIEvents.SendNotification("Il cancello non può essere aperto", Color.red);
            return;
        }

        if(bOpen)
        {
            bOpen = false;
            source.clip = close;
            source.Play();
            animator.SetTrigger(GameConstants.ANIM_CLOSE);
            
        }
        else
        {
            bOpen=true;
            source.clip = open;
            source.Play();
            animator.SetTrigger(GameConstants.ANIM_OPEN);
            
        }
    }
}
