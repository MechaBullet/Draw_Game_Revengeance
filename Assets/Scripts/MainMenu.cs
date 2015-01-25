using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour {
	GameObject[] campfires;
	Vector3 spawnPoint;
	// Use this for initialization
	void Start () {
		StartNewGame();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void StartNewGame() {
		campfires = GameObject.FindGameObjectsWithTag("Campfire");
		spawnPoint = campfires[Random.Range(0, campfires.Length)].transform.position;
		transform.root.position = new Vector3(spawnPoint.x, spawnPoint.y + 5, spawnPoint.z - 10);
		transform.root.RotateAround(spawnPoint, Vector3.up, Random.Range(-360, 360));
	}
}
