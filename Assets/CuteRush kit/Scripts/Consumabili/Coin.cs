using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : PickableItem<int>
{

    public void Start()
    {
        Value = UnityEngine.Random.Range(10, 50);
    }

    public override void ApplyEffect(PlayerController player)
    {
        player.addGold(Value);
    }


}
