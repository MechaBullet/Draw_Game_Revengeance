using UnityEngine;
using System.Collections;

public class EnemyBehavior : MonoBehaviour {

	public float maxHealth = 100;
	private float health;
	public GameObject ragdoll;
	private Color origColor;
	public GameObject characterMesh; 

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
		characterMesh.renderer.material.color = Color.red;
		health -= damage;
		StartCoroutine(RevertColor());
	}

	IEnumerator RevertColor() {
		yield return new WaitForSeconds(0.2f);
		characterMesh.renderer.material.color = origColor;
	}

	void Dead() {
		Instantiate(ragdoll, transform.position, transform.rotation);
		Destroy(this.gameObject);
	}
}
