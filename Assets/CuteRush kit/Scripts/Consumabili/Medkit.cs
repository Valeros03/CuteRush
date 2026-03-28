using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Medkit : PickableItem<float>
{
    
    public override void ApplyEffect(PlayerInteraction player)
    {
        player.addMedkit();
    }

    
}
