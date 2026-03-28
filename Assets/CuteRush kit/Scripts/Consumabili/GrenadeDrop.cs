using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeDrop : PickableItem<int>
{

    public override void ApplyEffect(PlayerInteraction player)
    {
        if (!player.addGrenade())
        {
            player.addGold(10);
        }
    }
}
