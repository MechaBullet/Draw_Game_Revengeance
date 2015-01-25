using UnityEngine;
using System.Collections;

public class CamTilt : MonoBehaviour {
	private Transform player;

	private float rotationZ;
	private float rotationY;

	private Quaternion original;
	private Vector3 current;
	private Vector3 previous;

	// Use this for initialization
	void Start () {
		player = transform.root;
		original = transform.localRotation;
		current = player.transform.localPosition;
		previous = player.transform.localPosition;
	}
	
	// Update is called once per frame
	void Update () {
		current = player.transform.position;

		//rotationY = ((current.y - previous.y) / Time.deltaTime * -1);
		rotationZ = ((current.z - previous.z) / Time.deltaTime * -1);
		transform.localRotation = original * Quaternion.Euler(0, 0, rotationZ);

		previous = player.transform.localPosition;
	}
}
