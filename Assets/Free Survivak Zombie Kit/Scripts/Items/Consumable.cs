using UnityEngine;

/* An Item that can be consumed. So far that just means regaining health */

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Consumable")]
public class Consumable : Item {

	public int healthGain;		// How much Health?

    public override void Use()
	{
        VitalsController vitals = GameObject.FindGameObjectWithTag("Player").GetComponent<VitalsController>();

        if (healthGain > 0)
            vitals.Increase(healthGain);
                

		RemoveFromInventory();
	}

}
