using UnityEngine;
using System.Collections;

public class EnemyBehavior : MonoBehaviour {

	public float maxHealth = 100;
	private float health;
	public GameObject ragdoll;
	private Color origColor;
	public GameObject characterMesh;
	public GameObject ptsPrefab;

	// Use this for initialization
	void Start () {
		health = maxHealth;
		origColor = characterMesh.renderer.material.color;
	}
	
	// Update is called once per frame
	void Update () {
		if(health < 1) Dead();
	}

	public void Damage(float damage, RaycastHit hitPoint) {
		//characterMesh.renderer.material.color = Color.red;
		health -= damage;
		Vector3 v = Camera.main.WorldToViewportPoint(transform.position);
		StatusText(damage.ToString());
	}

	void StatusText(string s){
		Vector3 v = Camera.main.WorldToViewportPoint(transform.position);
		float x = v.x;
		float y = v.y + 0.1f;
		x = Mathf.Clamp(x,0.05f,0.95f); // clamp position to screen to ensure
		y = Mathf.Clamp(y,0.05f,0.9f); // the string will be visible
		GameObject gui = Instantiate(ptsPrefab,new Vector3(x,y,0),Quaternion.identity) as GameObject;
		gui.guiText.text = s;
	}

	void Dead() {
		StatusText("Dead");
		Instantiate(ragdoll, transform.position, transform.rotation);
		Destroy(this.gameObject);
	}
}
