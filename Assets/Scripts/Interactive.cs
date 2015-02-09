using UnityEngine;
using System.Collections;

public class Interactive : MonoBehaviour {
	ItemDatabase database;
	private bool used;
	void Start() {
		database = GameObject.FindGameObjectWithTag("ItemDatabase").GetComponent<ItemDatabase>();
		used = false;
	}

	public void Interact() {
		if(gameObject.name == "Chest" && !used) {
			gameObject.GetComponent<Animator>().SetTrigger("Open");
			StartCoroutine(DropRandomLoot(0.5f));
			used = true;
		}
	}

	IEnumerator DropRandomLoot(float time) {
		yield return new WaitForSeconds(time);
		GameObject loot = Instantiate(Resources.Load(database.items[Random.Range(0, database.items.Count)].itemPrefab), transform.position + Vector3.up * 4, 
		                              Quaternion.Euler(new Vector3(Random.Range(0,360), Random.Range(0,360), Random.Range(0, 360)))) as GameObject;
		loot.rigidbody.AddForce(Vector3.up * Random.Range(600, 1000) + Vector3.forward * Random.Range(-200, -100) + Vector3.right * Random.Range(-100, 100));
		loot.rigidbody.AddTorque(Random.Range (0, 200), Random.Range (0, 200), Random.Range(0,200));
	}
}
