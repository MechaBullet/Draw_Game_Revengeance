using UnityEngine;
using System.Collections;

public class EnableCamera : MonoBehaviour {
	public Camera cam;
	public GameObject parent;

	// Use this for initialization
	void Start () {
		if(parent.networkView.isMine) {
			cam.enabled = true;
		}
		else cam.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
