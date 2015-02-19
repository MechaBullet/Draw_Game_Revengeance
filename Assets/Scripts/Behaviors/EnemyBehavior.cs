using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;

public class EnemyBehavior : MonoBehaviour {
	public int challengeRating;
	public float maxHealth = 100;
	public bool roamingEnabled = false;
	private Animator animator;
	private float health;
	private GameObject characterMesh;
	private GameObject ptsPrefab;
	private ItemDatabase database;
	private int securityLevel;
	private bool searching = false;
	private bool tracking = false;
	private Transform player;
	private Vector3 oldPos;
	public float velocity;
	public GameObject weaponSlot;
	public Shooting shooting;
	private float speed;
	public float attackCutoff;

	// Use this for initialization
	void Awake () {
		animator = transform.GetChild(0).GetComponent<Animator>();
		player = GameObject.FindGameObjectWithTag("Player").transform;
		ptsPrefab = Resources.Load("Prefabs/DamageText") as GameObject;
		health = maxHealth;
		database = GameObject.FindGameObjectWithTag("ItemDatabase").GetComponent<ItemDatabase>();
		if(weaponSlot) shooting = weaponSlot.GetComponentInChildren<Shooting>();
		attackCutoff = shooting.GetComponentInChildren<Shooting>().range;

	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if(health < 1) Dead();
		Scan();
		NavigatePath();
		velocity = Vector3.Distance(oldPos, transform.position);
		animator.SetFloat("Velocity", velocity);
		oldPos = transform.position;
	}

	public void Damage(float damage, RaycastHit hitPoint) {
		//characterMesh.renderer.material.color = Color.red;%
		health -= damage;
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
		DropRandomLoot();
		Destroy(this.gameObject);
	}

	void DropRandomLoot() {
		GameObject loot = Instantiate(Resources.Load(database.items[Random.Range(0, database.items.Count)].itemPrefab), transform.position + Vector3.up * 4, transform.rotation) as GameObject;
		loot.rigidbody.AddForce(Vector3.up * Random.Range(400, 800) + Vector3.forward * Random.Range(-200, -100) + Vector3.right * Random.Range(-100, 100));
		loot.rigidbody.AddTorque(new Vector3(Random.Range (0, 100), Random.Range (0, 100), Random.Range (0,100)));
	}

	//============================================================= AI =================================================
	//The point to move to
	public Vector3 targetPosition;
	private Seeker seeker;
	private CharacterController controller;
	//The calculated path
	public Path path;
	//The AI's speed per second
	//The max distance from the AI to a waypoint for it to continue to the next waypoint
	public float nextWaypointDistance = 3;
	//The waypoint we are currently moving towards
	private int currentWaypoint = 0;

	public void Start () {
		seeker = GetComponent<Seeker>();
		controller = GetComponent<CharacterController>();
		//Start a new path to the targetPosition, return the result to the OnPathComplete function
	}

	public void StartPath(Vector3 target) {
		seeker.StartPath (transform.position, target, OnPathComplete);
	}

	public void OnPathComplete (Path p) {
		if (!p.error) {
			path = p;
			//Reset the waypoint counter
			currentWaypoint = 0;
		}
	}
	public float angularSpeed = 10;
	public void NavigatePath () {
		if (path == null) {
			//We have no path to move after yet
			return;
		}
		if (currentWaypoint >= path.vectorPath.Count) {
			//Path Complete
			return;
		}
		//Direction to the next waypoint
		Vector3 dir = (path.vectorPath[currentWaypoint]-transform.position).normalized;
		dir *= speed * Time.fixedDeltaTime;
		controller.SimpleMove (dir);
		//===============================================================//
		Vector3 lookPoint = (Vector3)path.path[currentWaypoint].position - transform.position;
		lookPoint = new Vector3(lookPoint.x, 0, lookPoint.z);
		Quaternion targetRotation = Quaternion.LookRotation(lookPoint);
		// Smoothly rotate towards the target point.
		collider.transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, angularSpeed * Time.deltaTime);
		//Check if we are close enough to the next waypoint
		//If we are, proceed to follow the next waypoint
		if (Vector3.Distance (transform.position,path.vectorPath[currentWaypoint]) < nextWaypointDistance) {
			currentWaypoint++;
			return;
		}
	}
	//====================================================================================================================//
	public int patrolRange = 60;
	public float detectionCutoff = 80;
	private Vector3 trackedPosition;

	private void Scan() {
		Vector3 targetDir = player.position - transform.position;
		float distance = Vector3.Distance(transform.position, player.position);
		Vector3 forward = transform.forward;
		float angle = Vector3.Angle(targetDir, forward);
		//If enemy is in cone of vision or extremely close
		if(angle <= 110 || distance < detectionCutoff / 2.5) {
			//Start moving toward player
			securityLevel = 1;
			//Or if the player is in detection range
			if(distance < detectionCutoff) {
				//Start closing in on the player
				securityLevel = 2;
					//And if they are in range to attack
					if(distance < attackCutoff) {
						//Start attacking them
						securityLevel = 3;
				}
			}
		}
		//Otherwise, do whatever
		else securityLevel = 0;
		Process();
	}
	
	private void Process() {
		speed = 350;
		//Turn off switches when not on the proper alert level
		if(securityLevel != 0) {
			if(IsInvoking("Wander")) CancelInvoke("Wander");
		}
		if(securityLevel != 1) searching = false;
		if(securityLevel != 2 && securityLevel != 1) tracking = false;
		//If the wandering switch has been turned off, stop wandering
		switch(securityLevel) {
		case 0:
			if(roamingEnabled) Patrolling();
			break;
		case 1:
			Searching(player);
			break;
		case 2:
			speed = 600;
			Tracking(player);
			break;
		case 3:
			speed = 600;
			Attacking(player);
			break;
		}
	}

	void Wander() {
		if(Random.Range(0,100) <= 80) {
			Vector3 patrolDestination = transform.position + new Vector3(Random.Range(-patrolRange,patrolRange), 0, Random.Range(-patrolRange,patrolRange));
			StartPath(patrolDestination);
		}
	}
	
	private void Patrolling() {
		if(!IsInvoking("Wander"))
			InvokeRepeating("Wander", 0, 10.0f);
	}

	private void Searching(Transform target) {
		RaycastHit hit;
		Vector3 rayDirection = target.position - transform.position;
		if (Physics.Raycast(transform.position, rayDirection, out hit, detectionCutoff * 1.5f)) {
			if (hit.transform == target && !searching && !tracking) {
				searching = true;
				//Enemy can see the player
				float searchRange = patrolRange/2.5f;
				Vector3 searchPoint = new Vector3(target.position.x + Random.Range(-searchRange,searchRange), target.position.y, target.position.z + Random.Range(-searchRange, searchRange));
				StartPath(searchPoint);
			} else {
				searching = false;
				Patrolling();
			}
		}
	}

	void Attacking(Transform target) {
		if (!shooting.firing) {
			transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
			StartCoroutine(shooting.Fire());
			animator.Play("Shoot");
		}
		else Tracking(player);
	}

	void Tracking(Transform target) {
		if(!tracking) {
			trackedPosition = target.position;
			tracking = true;
			StartPath(target.position);
		}
		else if(tracking && Vector3.Distance(target.position, trackedPosition) > 5) {
			trackedPosition = target.position;
			StartPath(trackedPosition);
		}
	}
}