using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour {

	#region Singleton

	public static Inventory instance;

	void Awake ()
	{
		instance = this;
	}

	#endregion

	public delegate void OnItemChanged();
	public OnItemChanged onItemChangedCallback;

	public int space = 10;	// Amount of item spaces

	// Our current list of items in the inventory
	public List<Item> items = new List<Item>();

    public int gold;

    private void Update()
    {
        GameObject canvas = GameObject.Find("Canvas");
        if (canvas == null) return;

        Transform inventoryTransform = canvas.transform.Find("Inventory");
        if (inventoryTransform == null) return;

        GameObject inventoryUI = inventoryTransform.gameObject;
        if (!inventoryUI.activeSelf) return;

        Transform goldTransform = inventoryUI.transform.Find("Gold/Value");
        if (goldTransform == null) return;

        Text goldText = goldTransform.GetComponent<Text>();
        if (goldText == null) return;

        goldText.text = gold.ToString();
    }

    // Add a new item if enough room
    public void Add (Item item)
	{
		if (item.showInInventory) {
			if (items.Count >= space) {
				Debug.Log ("Not enough room.");
				return;
			}

			items.Add (item);

			if (onItemChangedCallback != null)
				onItemChangedCallback.Invoke ();
		}
	}

	// Remove an item
	public void Remove (Item item)
	{
		items.Remove(item);

		if (onItemChangedCallback != null)
			onItemChangedCallback.Invoke();
	}

}
