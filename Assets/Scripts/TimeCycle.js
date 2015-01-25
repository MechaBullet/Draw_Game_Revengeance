#pragma strict

var cycleSpeed : float;

function Start () {
	InvokeRepeating("Cycle", cycleSpeed * 2, cycleSpeed);
}

function Cycle() {
	var targetAngle = Quaternion.Euler(transform.eulerAngles.x + 1, transform.eulerAngles.y, transform.eulerAngles.z);
	transform.rotation = Quaternion.Lerp(transform.rotation, targetAngle, Time.deltaTime * 2);
}