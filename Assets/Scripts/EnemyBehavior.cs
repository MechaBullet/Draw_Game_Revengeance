using UnityEngine;
using System.Collections;

public class EnemyBehavior : MonoBehaviour {

	public float maxHealth = 100;
	private float health;
	public GameObject ragdoll;
	private Color origColor;
	public GameObject characterMesh;
	public Transform ptsPrefab;

	// Use this for initialization
	void Start () {
		health = maxHealth;
		origColor = characterMesh.renderer.material.color;
	}
	
	// Update is called once per frame
	void Update () {
		if(health < 1) Dead();
	}

	public void Damage(float damage) {
		//characterMesh.renderer.material.color = Color.red;
		health -= damage;
		Vector3 v = Camera.main.WorldToViewportPoint(transform.position);
		//DamageText(damage, v.x, v.y);
	}

	void DamageText(float points, float x, float y){
		x = Mathf.Clamp(x,0.05f,0.95f); // clamp position to screen to ensure
		y = Mathf.Clamp(y,0.05f,0.9f); // the string will be visible
		GameObject gui = Instantiate(ptsPrefab,new Vector3(x,y,0),transform.rotation) as GameObject;
		gui.guiText.text = points.ToString();
	}

	void Dead() {
		Instantiate(ragdoll, transform.position, transform.rotation);
		Destroy(this.gameObject);
	}
}
