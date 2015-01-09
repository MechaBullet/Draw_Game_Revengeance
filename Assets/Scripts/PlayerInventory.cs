using UnityEngine;
using System.Collections;

public class PlayerInventory : MonoBehaviour {
	public GameObject playerCam;
	private RaycastHit hit;
	public Material highlightMat;

	private Material origMat;
	private Material newMat;
	public GameObject oldFocus;
	public GameObject currentFocus;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out hit, 10)) {
			if(hit.transform.root.tag == "Item" && hit.transform.gameObject != oldFocus) {
				currentFocus = hit.transform.gameObject;
				foreach (Transform child in hit.transform) {
					if(child.gameObject.renderer) {
						origMat = child.renderer.material;
						child.renderer.material = highlightMat;
						//child.renderer.material = origMat
					}
				}
			}
		}

		if(oldFocus != currentFocus) {
			foreach(Transform child in oldFocus.transform) {
				child.renderer.material = origMat;
			}
		}
		oldFocus = currentFocus;
	}

	void PickUpItem(GameObject item) {

	}
}
