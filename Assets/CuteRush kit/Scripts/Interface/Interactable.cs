using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Interactable
{
    void Interact(PlayerController player);
    void OnTriggerEnter(Collider collider);
}
