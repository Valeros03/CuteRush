using System.Numerics;
using UnityEngine;

public class MeleeEnemy : Enemy
{
    protected override void PerformAttack()
    {
        if (isPlayerInAttackArea && player != null)
        {
            player.GetComponent<VitalsController>()?.Decrease(attackDamage);
        }
    }
}
