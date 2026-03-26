using UnityEngine;

public class MagSpawner : ItemSpawner
{
    protected override bool TryGiveItem(GameObject player)
    {
        GunBase playerGun = player.GetComponentInChildren<GunBase>();

        if (playerGun != null)
        {
            bool itemTaken = false;

            for (int i = 0; i < amountToGive; i++)
            {
                if (playerGun.addMag())
                {
                    itemTaken = true;
                }
            }

            return itemTaken;
        }

        return false;
    }
}