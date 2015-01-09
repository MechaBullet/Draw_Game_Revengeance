using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour {
	//Inventory Stuff
	public List<GameObject> storedItems = new List<GameObject>();
	private int[] equippedWeapons = new int[3];
	private GameObject rightHandWeapon;
	public Transform rightHandSlot;
	private int currentWeapon;

	public GameObject playerCam;
	private RaycastHit hit;
	public Material highlightMat;

	private Material origMat;
	private Material newMat;
	private GameObject oldFocus;
	private GameObject currentFocus;

	public GameObject revolver;
	public GameObject leverRifle;
	// Use this for initialization
	void Start() {

	}
	// Update is called once per frame
	void Update () {
		if(Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out hit, 10)) {
			currentFocus = hit.transform.gameObject;
			if(hit.transform.root.tag == "Item") {
				if(currentFocus != oldFocus) {
					Debug.Log("Focused " + currentFocus.name);
					foreach (Transform child in hit.transform) {
						if(child.gameObject.renderer) {
							if(oldFocus) {
								foreach (Transform oldChild in oldFocus.transform) {
									if(oldChild.renderer)
										oldChild.renderer.material = origMat;
								}
							}
							origMat = child.renderer.material;
							child.renderer.material = highlightMat;
						}
					}
				}
				if (Input.GetKeyDown(KeyCode.E)) PickUpItem(currentFocus);
				oldFocus = currentFocus;
			}
			else if(oldFocus) {
				foreach (Transform oldChild in oldFocus.transform) {
					if(oldChild.renderer)
						oldChild.renderer.material = origMat;
				}
			}
		}
	}
	
	void PickUpItem(GameObject item) {
		storedItems.Add(item);
		Debug.Log("Picked up " + item.name);
		Destroy(item);
	}

	void equipWeapon(int weaponType, int slot) {
		equippedWeapons[slot] = weaponType;
		//0 = Revolver
		//1 = Rifle
	}

	void switchWeapon(int weaponType) {
		switch(weaponType) {
		case 0 : revolver.SetActive(true);
			break;
		case 1 : leverRifle.SetActive(true);
			break;
		}
	}
}
