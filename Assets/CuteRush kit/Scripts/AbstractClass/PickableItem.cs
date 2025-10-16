using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PickableItem<T> : MonoBehaviour, IPickable
{
    public T Value { get; private set; }
    public virtual void OnPickup(PlayerController player)
    {
        ApplyEffect(player);
        Destroy(gameObject);
    }

    public abstract void ApplyEffect(PlayerController player);

}
