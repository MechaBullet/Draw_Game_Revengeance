using UnityEngine;
using System.Collections;

public class PlayerInfo : MonoBehaviour
{
	//Player stats
	public float maxHealth = 100;
	public float health;
	
	private CharacterMotor motor;
	public GameObject playerModel;
	public GameObject playerPrefab;
	public GameObject ragdollPrefab;
	Color originalColor;

	private int kills;
	private int deaths;

	GameObject[] spawnPoints;
	Transform ragdollSpawnPoint;
	Transform spawnPoint;

	// Use this for initialization
	void Awake () {
		health = maxHealth;
		Screen.showCursor = false;
		originalColor = playerModel.renderer.material.color;
		motor = GetComponent("CharacterMotor") as CharacterMotor;
		spawnPoints = GameObject.FindGameObjectsWithTag("Spawn");
	}
	//If this is the player object your network is linked to, give controls
    void Update()
    {
		fpsInput();
		if(health < 100) Dead();
    }
	
	public void Damage(float damage) {
		playerModel.renderer.material.color = Color.red;
		health -= damage;
		Debug.Log("Player is at " + health + " health.");
		if (health < 1) Dead();
	}

	private void Dead() {
		Transform ragdollSpawnPoint = this.transform;
		Debug.Log("Player is dead");
		deaths += 1;
		Instantiate(ragdollPrefab, ragdollSpawnPoint.position, ragdollSpawnPoint.rotation);
	}

	//Draw Crosshair
	private void OnGUI() {
			float width = 100;
			float height = 50;
			float x = (Screen.width / 2) - (width/2);
			float y = Screen.height - 100 - (height/2);
			GUI.Label(new Rect(x, y, width, height), "Health: " + health);
	}

	private void fpsInput() {
		// Get the input vector from keyboard or analog stick
		var directionVector = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
		
		if (directionVector != Vector3.zero) {
			// Get the length of the directon vector and then normalize it
			// Dividing by the length is cheaper than normalizing when we already have the length anyway
			var directionLength = directionVector.magnitude;
			directionVector = directionVector / directionLength;
			
			// Make sure the length is no bigger than 1
			directionLength = Mathf.Min(1, directionLength);
			
			// Make the input vector more sensitive towards the extremes and less sensitive in the middle
			// This makes it easier to control slow speeds when using analog sticks
			directionLength = directionLength * directionLength;
			
			// Multiply the normalized direction vector by the modified length
			directionVector = directionVector * directionLength;
		}
		
		// Apply the direction to the CharacterMotor
		motor.inputMoveDirection = transform.rotation * directionVector;
		motor.inputJump = Input.GetButton("Jump");
	}
}
