using UnityEngine;
using System.Collections;

public class PlayerInfo : MonoBehaviour
{
	float health = 100;
	private float lastSynchronizationTime = 0f;
	private float syncDelay = 0f;
	private float syncTime = 0f;
	private Vector3 syncStartPosition = Vector3.zero;
	private Vector3 syncEndPosition = Vector3.zero;
	private Quaternion syncStartRotation = Quaternion.identity;
	private Quaternion syncEndRotation = Quaternion.identity;
	private CharacterMotor motor;
	public GameObject playerModel;
	public GameObject playerPrefab;
	public GameObject ragdollPrefab;
	public GameObject postMortemPrefab;
	Color originalColor;
	
	// Use this for initialization
	void Awake () {
		Screen.showCursor = false;
		originalColor = playerModel.renderer.material.color;
		motor = GetComponent("CharacterMotor") as CharacterMotor;

	}
	//If this is the player object your network is linked to, give controls
    void Update()
    {
		if(networkView.isMine) fpsInput();
		else SyncedMovement();
    }

	public void Damage(float damage) {
		playerModel.renderer.material.color = Color.red;
		StartCoroutine(ToOriginalColor());
		health -= damage;
		Debug.Log("Player is at " + health + " health.");
		if (health < 1) Dead();
	}

	void Dead() {
		Transform spawnPoint;
		spawnPoint = this.transform;
		Debug.Log("Player is dead");
		Network.Instantiate(postMortemPrefab, spawnPoint.position + new Vector3(0, 5, 0), spawnPoint.rotation, 0);
		Network.Destroy(this.gameObject);
		Network.Instantiate(ragdollPrefab, spawnPoint.position, spawnPoint.rotation, 0);
		StartCoroutine(Respawn());
	}

	IEnumerator ToOriginalColor() {
		Debug.Log("Success");
		yield return new WaitForSeconds(0.5f);
		playerModel.renderer.material.color = originalColor;
	}

	IEnumerator Respawn() {
		yield return new WaitForSeconds(3);
		Network.Instantiate(playerPrefab, Vector3.up * 5, playerPrefab.transform.rotation, 0);
	}

	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
	{
		Vector3 syncPosition = Vector3.zero;
		Quaternion syncRotation = Quaternion.identity;

		if (stream.isWriting)
		{
			syncPosition = transform.position;
			stream.Serialize(ref syncPosition);

			/*syncVelocity = transform.velocity;
			stream.Serialize(ref syncVelocity);*/

			syncRotation = transform.rotation;
			stream.Serialize(ref syncRotation);

			/*syncAngularVelocity = transform.angularVelocity;
			stream.Serialize(ref syncAngularVelocity);*/
		}
		else
		{
			stream.Serialize(ref syncPosition);
			//stream.Serialize(ref syncVelocity);
			stream.Serialize(ref syncRotation);
			//stream.Serialize(ref syncAngularVelocity);
			syncTime = 0f;
			syncDelay = Time.time - lastSynchronizationTime;
			lastSynchronizationTime = Time.time;

			syncEndPosition = syncPosition;
			syncStartPosition = transform.position;

			syncEndRotation = syncRotation;
			syncStartRotation = transform.rotation;
		}
	}

	private void SyncedMovement() {
		syncTime += Time.deltaTime;
		transform.position = Vector3.Lerp (syncStartPosition, syncEndPosition, syncTime / syncDelay);
		transform.rotation = Quaternion.Lerp (syncStartRotation, syncEndRotation, syncTime / syncDelay);
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
