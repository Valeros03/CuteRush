using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mags : PickableItem<int>
{
    
    public override void ApplyEffect(PlayerInteraction player)
    {
        if (!player.addAmmo())
        {
            player.addGold(5);
        }
    }
}
