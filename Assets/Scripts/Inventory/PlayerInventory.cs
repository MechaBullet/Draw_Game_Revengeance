using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour {
	//Inventory Stuff
	//public List<GameObject> storedItems = new List<GameObject>();
	//private int[] equippedWeapons = new int[3];
	private GameObject rightHandWeapon;
	public Transform rightHandSlot;

	public bool showReticle;
	public Texture2D reticleSprite;

	public GameObject playerCam;
	private RaycastHit hit;
	public Material highlightMat;

	private Material origMat;
	private Material newMat;
	private GameObject oldFocus;
	private GameObject currentFocus = null;
	//Inventory GUI
	public GUISkin skin;
	//Inventory Variables
	public int slotsX;
	public int slotsY;
	public List<Item> inventory = new List<Item>();
	public List<Item> slots = new List<Item>();
	public Item[] weapons = new Item[3];
	private ItemDatabase database;
	private bool draggingItem;
	private bool fromEquipment;
	private Item draggedItem;
	private int draggedIndex;
	//Weapon Slot Variables

	public GameObject revolver;
	public GameObject leverRifle;

	bool viewInventory;
	bool showTooltip;
	string tooltip;
	
	// Use this for initialization
	void Start() {
		for (int i = 0; i < (slotsX * slotsY); i++)
		{
			slots.Add(new Item());
			inventory.Add (new Item());
		}
		for(int a = 0; a < weapons.Length; a++) {
			weapons[a] = new Item();
		}
		showTooltip = false;
		database = GameObject.FindGameObjectWithTag("ItemDatabase").GetComponent<ItemDatabase>();
	}
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.I)) viewInventory = !viewInventory;
		else if(Input.GetKeyDown(KeyCode.Alpha1)) SwitchWeapon(0);
		else if(Input.GetKeyDown(KeyCode.Alpha2)) SwitchWeapon(1);
		else if(Input.GetKeyDown(KeyCode.Alpha3)) SwitchWeapon(2);
		DetectItems();
	}

	void OnGUI() {
		GUI.DrawTexture(new Rect(Screen.width/2 - 4, Screen.height/2 - 4, 8, 8), reticleSprite);
		Screen.showCursor = false;
		tooltip = "";
		GUI.skin = skin;
		if(viewInventory) {
			Screen.showCursor = true;
			DrawInventory();
			DrawEquipment();
			DrawStats();
			if(showTooltip == true && tooltip != "") {
				GUI.Box(new Rect(Event.current.mousePosition.x + 20, Event.current.mousePosition.y + 20, 200, 50), tooltip);
			}
			if(draggingItem) {
				GUI.DrawTexture(new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 64, 64), draggedItem.itemIcon);
			}
		}
		else if(currentFocus != null && showTooltip == true) {
			if(currentFocus.GetComponent<ItemInfo>() != null) {
				tooltip = CreateToolTip(database.items[currentFocus.GetComponent<ItemInfo>().id]);
				GUI.Box(new Rect(Mathf.Clamp(playerCam.camera.WorldToScreenPoint(currentFocus.transform.position).x, Screen.width/2 - 300, Screen.height/2 + 300), 
				                 Mathf.Clamp(playerCam.camera.WorldToScreenPoint(currentFocus.transform.position).y, Screen.height/2 - 40, Screen.height/2 + 40), 200, 50), tooltip);
			}
		}
	}

	void DrawInventory() {
		int i = 0;
		Event e = Event.current;
		//Inventory
		for(int x = 0; x < slotsX; x++) {
			for(int y = 0; y < slotsY; y++) {
				Rect slotRect = new Rect(Screen.height/2 - (y * 64), Screen.height/2 - (x * 64), 64, 64);
				GUI.Box(slotRect, "", skin.GetStyle("Slot"));
				slots[i] = inventory[i];
				if(slots[i].itemName != null) {
					GUI.DrawTexture(slotRect, slots[i].itemIcon);
					if(slotRect.Contains(e.mousePosition)) {
						tooltip = CreateToolTip(slots[i]);
						showTooltip = true;
						//Drag and Drop
						if(Input.GetMouseButton(0) == true && e.type == EventType.mouseDrag && !draggingItem) {
							draggingItem = true;
							draggedIndex = i;
							draggedItem = slots[i];
							inventory[i] = new Item();
						}
						if(e.type == EventType.mouseUp && draggingItem) {
							if(fromEquipment) {
								Debug.Log("Moved Item from Equipment to Inventory");
								inventory[FindEmptySlot()] = inventory[i];
								inventory[i] = draggedItem;
							}
							else {
								Debug.Log("Moved item");
								inventory[draggedIndex] = inventory[i];
								inventory[i] = draggedItem;
							}
							draggingItem = false;
							draggedItem = null;
							fromEquipment = false;
						}
					}
				}
				else {
					if(slotRect.Contains(e.mousePosition)) {
						if (e.type == EventType.mouseUp && draggingItem) {
							inventory[i] = draggedItem;
							draggingItem = false;
							draggedItem = null;
							fromEquipment = false;
						}
					}
				}
				i++;
			}
		}
	}

	void DrawEquipment() {
		Event e = Event.current;
		Item.Slot slot;
		//Equipment Slots
		for(int z = 0; z < weapons.Length; z++) {
			Rect equipRect = new Rect ((Screen.width/2 + (64 * slotsY))/2, Screen.height/2 - (64 * (z +2)), 64, 64);
			GUI.Box(equipRect, "", skin.GetStyle("Slot"));
			switch(z) {
				case 0:
					slot = Item.Slot.Primary;
					break;
				case 1:
					slot = Item.Slot.Secondary;
					break;
				case 2:
					slot = Item.Slot.Melee;
					break;
				default: 
					slot = Item.Slot.Primary;
					break;
			}
			if(weapons[z].itemName != null) {
				GUI.DrawTexture(equipRect, weapons[z].itemIcon);
				if(equipRect.Contains(e.mousePosition)) {
					tooltip=CreateToolTip(weapons[z]);
					showTooltip = true;
					//Drag and Drop
					if(Input.GetMouseButton(0) == true && e.type == EventType.mouseDrag && !draggingItem) {
						draggingItem = true;
						draggedIndex = z;
						draggedItem = weapons[z];
						fromEquipment = true;
						weapons[z] = new Item();
					}
					if(e.type == EventType.mouseUp && draggingItem && draggedItem.itemType == Item.ItemType.Weapon) {
						if(draggedItem.slot == slot) {
							Debug.Log("Moved item");
							inventory[FindEmptySlot()] = weapons[z];
							weapons[z] = draggedItem;
							draggingItem = false;
							draggedItem = null;
							fromEquipment = false;
						}
					}
				}

			}
			else {
				if(equipRect.Contains(e.mousePosition)) {
					if (e.type == EventType.mouseUp && draggingItem && draggedItem.itemType == Item.ItemType.Weapon) {
						if(draggedItem.slot == slot) {
							weapons[z] = draggedItem;
							draggingItem = false;
							draggedItem = null;
							fromEquipment = false;
						}
					}
				}
			}
        }
	}

	void DrawStats() {
		PlayerInfo info = gameObject.GetComponent<PlayerInfo>() as PlayerInfo;
		string statText = "Class: " + info.stats.playerClass.ToString() + "\n" +
				"Health: " + info.health + " / " + info.maxHealth + "\n" +
				"Level: " + info.stats.level + "\n" +
				"Experience: " + info.stats.experience + "\n" +
				"=================================" + "\n" +
				"Dexterity: " + info.stats.dexterity + "\n" +
				"Cunning: " + info.stats.cunning + "\n" +
				"Resolve: " + info.stats.resolve + "\n";
						  

		GUI.Box(new Rect(Screen.width/2 + 100, Screen.height/2 - 200, 200, 400), statText);
	}

	string CreateToolTip(Item item) {
		//Color markup = "<color = #FFF>String</color> , \n"
		tooltip = item.itemName + "\n <color=#BBB>" + item.itemDesc + "</color>";
		return tooltip;
	}

	void DetectItems() {
		if(oldFocus) {
			if(oldFocus.tag == "Item") {
				foreach (Transform oldChild in oldFocus.transform) {
					if(oldChild.renderer)
						oldChild.renderer.material = origMat;
				}
			}
		}
		//////////////////////////////////////Picking up items//////////////////////////////////
		if(Physics.CapsuleCast(playerCam.transform.position, playerCam.transform.position, 2, playerCam.transform.forward, out hit, 10)) {
			currentFocus = hit.transform.gameObject;
			if(currentFocus.tag == "Item") {
				showTooltip = true;
				currentFocus = hit.transform.gameObject;
				foreach (Transform child in currentFocus.transform) {
					if(child.renderer) {
						if(child.renderer.material != highlightMat) {
							origMat = child.renderer.material;
							child.renderer.material = highlightMat;
						}
					}
				}
			}
			if (Input.GetKeyDown(KeyCode.E)) PickUpItem(currentFocus);
		}
		else currentFocus = null;
		oldFocus = currentFocus;
	}

	void PickUpItem(GameObject item) {
		int id = item.GetComponent<ItemInfo>().id;
		inventory[FindEmptySlot()] = database.items[id];
		Debug.Log("Picked up " + item.name);
		Destroy(item);
	}

	int FindEmptySlot() {
		for(int i = 0; i < inventory.Count; i++) {
			if(inventory[i].itemID == 0) {
				return i;
			}
		}
		return 0;
	}

	void RemoveItem(int id)
	{
		for(int i = 0; i < inventory.Count; i++) {
			if(inventory[i].itemID == id) {
				inventory[i] = new Item();
				break;

			}
		}
	}

	bool InventoryContains(int id) {
		bool result = false;
		for (int i = 0; i < inventory.Count; i++) {
			result = inventory[i].itemID == id;
			if (result) break;
		}
		return result;
	}

	void SwitchWeapon(int slot) {
		leverRifle.SetActive(false);
		revolver.SetActive(false);

		Item selectedWeapon = weapons[slot];
		if (slot == 0) {
			switch(selectedWeapon.weaponBase) {
				case Item.Base.LeverRifle :
					leverRifle.SetActive(true);
					break;
				default : break;
			}
		}
		else if (slot == 1) {
			switch(selectedWeapon.weaponBase) {
				case Item.Base.Revolver :
					revolver.SetActive(true);
					break;
				default : break;
			}
		}
		else {

		}
	}
}
