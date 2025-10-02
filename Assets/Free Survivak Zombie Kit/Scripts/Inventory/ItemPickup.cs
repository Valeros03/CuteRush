using UnityEngine;

public class ItemPickup : MonoBehaviour {

	public Item item;   // Item to put in the inventory if picked up

    void OnTriggerStay(Collider playerCollider)
    {
        if (playerCollider.gameObject.CompareTag("Player"))
        {

            if (Input.GetKeyDown(KeyCode.P))
            {
                PickUp();
            }
        }
    }

	// Pick up the item
	void PickUp ()
	{
        //Qua serve la logica per determinare se è salute o munizioni o granate
		Destroy(gameObject);	// Destroy item from scene
	}

}
