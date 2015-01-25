using UnityEngine;
using System.Collections;

public class PlayerInfo : MonoBehaviour
{
	//Player stats
	public float maxHealth = 100;
	public float health;

	public GameObject playerPrefab;
	public GameObject ragdollPrefab;

	public Stats stats;
	private int kills;
	private int deaths;

	GameObject[] spawnPoints;
	Transform ragdollSpawnPoint;
	Transform spawnPoint;

	// Use this for initialization
	void Awake () {
		health = maxHealth;
		spawnPoints = GameObject.FindGameObjectsWithTag("Spawn");
		NewCharacter(Stats.Archetype.Guardian);
	}
	//If this is the player object your network is linked to, give controls
    void Update()
    {
		if(health < 100) Dead();
    }

	void NewCharacter(Stats.Archetype archetype) {
		stats = new Stats(1, archetype);
	}
	
	public void Damage(float damage) {
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

	private void OnGUI() {
			float width = 100;
			float height = 50;
			float x = (Screen.width / 2) - (width/2);
			float y = Screen.height - 100 - (height/2);
			GUI.Label(new Rect(x, y, width, height), "Health: " + health);
	}
}
