using UnityEngine;
using System.Collections;

public class Shooting : MonoBehaviour {
	//For assigning prefabs and transforms
	public Texture2D crosshairImage;
	public GameObject raycastObject;
	public Transform origin;
	public GameObject bullet;
	public GameObject camera;
	bool reloading, firing;
	//Gun Stats
	public int range, maxClip, damage;
	public float recoil, bulletSpread, fireRate, reloadTime;
	int bullets;

	// Use this for initialization
	void Start () {
		bullets = maxClip;
	}
	
	// Update is called once per frame
	void Update () {
		if(this.transform.root.gameObject.networkView.isMine) {
			if(Input.GetMouseButtonDown(0) && bullets > 0 && !firing) {
				StartCoroutine(Fire());
			}
			else if(Input.GetKeyDown(KeyCode.R) && bullets < maxClip && !reloading) {
				StartCoroutine(Reload(reloadTime));
			}
		}
	}

	void OnGUI()
	{
		if(this.transform.root.gameObject.networkView.isMine) {
			float xMin = (Screen.width / 2) - (crosshairImage.width / 2);
			float yMin = (Screen.height / 2) - (crosshairImage.height / 2);
			GUI.DrawTexture(new Rect(xMin, yMin, crosshairImage.width, crosshairImage.height), crosshairImage);
			if (reloading) GUI.Label(new Rect(xMin-100, yMin+300, 200, 100), "Reloading");
			GUI.Box(new Rect(Screen.width-200, Screen.height-100, 200, 100), bullets.ToString());
		}
	}

	void CheckForHit(Vector3 displacement){
		bullets -= 1;
		Debug.Log(bullets);
		RaycastHit hit;
		Vector3 fwd = raycastObject.transform.TransformDirection(Vector3.forward);
		if(Physics.Raycast(raycastObject.transform.position, /*new Vector3(fwd.x * displacement.x, fwd.y * displacement.y, fwd.z * displacement.z)*/ fwd, out hit, range)) {
			if(hit.transform.root.tag == "Player") {
				PlayerInfo player = hit.transform.root.GetComponent("PlayerInfo") as PlayerInfo;
				player.Damage(damage);
			}
		}
	}

	IEnumerator Reload(float time) {
		reloading = true;
		yield return new WaitForSeconds(time);
		bullets = maxClip;
		reloading = false;
	}

	// Fire a bullet 
	IEnumerator Fire() {
		firing = true;
		float xRand = Random.Range(-bulletSpread, bulletSpread);
		float yRand = Random.Range(-bulletSpread, bulletSpread);
		Quaternion spread = Quaternion.Euler(xRand, yRand, origin.transform.rotation.z);
		Network.Instantiate(bullet, origin.transform.position, origin.transform.rotation, 0);
		Vector3 spreadVector = new Vector3(xRand, yRand, 1);
		CheckForHit(spreadVector);
		yield return new WaitForSeconds(fireRate);
		firing = false;
	}

	void Recoil() {

	}
}
