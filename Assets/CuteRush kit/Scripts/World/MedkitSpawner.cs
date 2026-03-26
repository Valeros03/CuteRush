using UnityEngine;

public class MedkitSpawner : ItemSpawner
{
    protected override bool TryGiveItem(GameObject player)
    {
        PlayerController pc = player.GetComponent<PlayerController>();

        if (pc != null)
        {
            bool itemTaken = false;

            for (int i = 0; i < amountToGive; i++)
            {
                if (pc.addMedkit())
                {
                    itemTaken = true;
                }
            }

            return itemTaken;
        }

        return false;
    }
}