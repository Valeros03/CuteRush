using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateController : MonoBehaviour, Interactable
{
    [SerializeField] private Animator animator;
    private bool bOpen;
    public bool isOpenable; //gestita dall'esterno serve a gestire il timer prima di poter rientrare
    public void Interact(PlayerController player)
    {
        toggleGate();
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if(bOpen)
                UIManager.Instance.ShowInteract("chiudere il cancello");
            else
                UIManager.Instance.ShowInteract("aprire il cancello");
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            UIManager.Instance.HideInteract();
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
            animator.SetTrigger("Close");
        }
        else
        {
            bOpen=true;
            animator.SetTrigger("Open");
        }
    }
}
