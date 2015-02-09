using UnityEngine;
using System.Collections;

public class Shooting : MonoBehaviour {
	//For assigning prefabs and transforms
	public Texture2D crosshairImage;
	public GameObject raycastObject;
	public GameObject bullet;
	//public GameObject cameraObject;
	bool reloading, firing;
	public bool isMelee = false;
	public bool showCrosshair = true;
	//Gun Stats
	public int range, maxClip, damage;
	public float recoil, bulletSpread, fireRate, reloadTime;
	int bullets;
	bool isPlayer;
	Effects[] effects;
	public Transform origin;

	public enum Effects{
		Flaming, Shocking, Freezing
	}

	// Use this for initialization
	void Start () {
		if(transform.root.tag == "Player") {
			isPlayer = true;
			origin = Camera.main.transform;
		}
		bullets = maxClip;
	}
	
	// Update is called once per frame.
	void Update () {
		if(Input.GetMouseButtonDown(1)) {
			if(Camera.main.fieldOfView == 60.0f) {
				StopCoroutine("LerpFoV");
				StartCoroutine(LerpFoV(40.0f));
			}
			else if(Camera.main.fieldOfView == 40.0f) {
				StopCoroutine("LerpFoV");
				StartCoroutine(LerpFoV(60.0f));
			}
		}

		if(isPlayer) {
			if(Input.GetMouseButtonDown(0) && bullets > 0 && !firing) {
				StartCoroutine(Fire());
			}
			else if(Input.GetKeyDown(KeyCode.R) && bullets < maxClip && !reloading) {
				StartCoroutine(Reload(reloadTime));
			}
		}
	}

	public void SetProperties(int damage, int clipSize, int range, float recoil, float bulletSpread, float fireRate, float reloadTime) {
		this.damage = damage;
		this.maxClip = clipSize;
		this.range = range;
		this.recoil = recoil;
		this.bulletSpread = bulletSpread;
		this.fireRate = fireRate;
		this.reloadTime = reloadTime;

	}
	void OnGUI()
	{
		if(showCrosshair && isPlayer && crosshairImage != null) {
			float xMin = (Screen.width / 2) - (crosshairImage.width / 2);
			float yMin = (Screen.height / 2) - (crosshairImage.height / 2);
			GUI.DrawTexture(new Rect(xMin, yMin, crosshairImage.width, crosshairImage.height), crosshairImage);
			if (reloading) GUI.Label(new Rect(xMin-100, yMin+300, 200, 100), "Reloading");
			GUI.Box(new Rect(Screen.width-200, Screen.height-100, 200, 100), bullets.ToString());
		}
	}

	void CheckForHit(Vector3 displacement){
		bullets -= 1;
		RaycastHit hit;
		if(Physics.Raycast(origin.position, origin.forward, out hit, range)) {
			if(hit.transform.root.tag == "Enemy") {
				EnemyBehavior enemyBehavior = hit.transform.gameObject.GetComponent("EnemyBehavior") as EnemyBehavior;
				enemyBehavior.Damage(damage, hit);
			}
		}
		Instantiate(bullet, origin.position, origin.rotation);
		firing = false;
	}

	IEnumerator Reload(float time) {
		reloading = true;
		yield return new WaitForSeconds(time);
		bullets = maxClip;
		reloading = false;
	}

	// Fire a bullet 
	IEnumerator Fire() {
		if(!isMelee) {
			firing = true;
			float xRand = Random.Range(-bulletSpread, bulletSpread);
			float yRand = Random.Range(-bulletSpread, bulletSpread);
			Quaternion spread = Quaternion.Euler(xRand, yRand, origin.rotation.z);
			Vector3 spreadVector = new Vector3(xRand, yRand, 1);
			CheckForHit(spreadVector);
			yield return new WaitForSeconds(fireRate);
		}
	}

	IEnumerator LerpFoV(float fov) {
		Debug.Log("Changing field of view to " + fov);
		// lerping a value in this way may take quite some time to reach the exact target value, so we will just stop lerping when the difference is small enough, i.e 0.05
		float dif = Mathf.Abs(Camera.main.fieldOfView - fov);
		while(dif > 0.2f) {
			float cameraFov = Mathf.Lerp(Camera.main.fieldOfView, fov, 0.2f);
			Camera.main.fieldOfView = cameraFov;
			GameObject.FindGameObjectWithTag("DistantCam").camera.fieldOfView = cameraFov;
			// update the difference
			dif = Mathf.Abs(Camera.main.fieldOfView - fov);
			yield return null;
		}
		if(dif <= 0.2f) {
			Camera.main.fieldOfView = fov;
			GameObject.FindGameObjectWithTag("DistantCam").camera.fieldOfView = fov;
			yield return null;
		}
	}

	void Recoil() {

	}
}
