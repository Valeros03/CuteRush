using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Totem : InteractableItem
{
    [SerializeField] private GameObject fire;
    public override void Interact()
    {
        fire.SetActive(false);
        GetComponentInParent<SpawnManager>().StopSpawn();
    }

}
