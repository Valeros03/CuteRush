using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mags : PickableItem<int>
{
    
    public override void ApplyEffect(PlayerController player)
    {
        player.addAmmo();
    }
}
