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
    public bool isOpenable; //gestita dall'esterno serve a gestire il timer prima di poter rientrare
    public override void Interact()
    {
        toggleGate();
    }
    public override void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if(bOpen)
                UIManager.Instance.ShowInteract("chiudere il cancello");
            else
                UIManager.Instance.ShowInteract("aprire il cancello");
        }
    }

    public void CloseGate() //chiamata dall'esterno quando il player esce dal gate
    {
        bOpen = false;
        animator.SetTrigger("Close");
    }
    


    private void toggleGate()
    {
        if (!isOpenable)
        {
            UIManager.Instance.ShowNotOpenable();
            return;
        }

        if(bOpen)
        {
            bOpen = false;
            source.clip = close;
            source.Play();
            animator.SetTrigger("Close");
            
        }
        else
        {
            bOpen=true;
            source.clip = open;
            source.Play();
            animator.SetTrigger("Open");
            
        }
    }
}
