using UnityEngine;

public class MedkitSpawner : ItemSpawner
{
    protected override bool TryGiveItem(GameObject player)
    {
        PlayerInteraction pc = player.GetComponent<PlayerInteraction>();

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