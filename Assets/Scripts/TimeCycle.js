#pragma strict

var cycleSpeed : float = 5;

function Start () {
	InvokeRepeating("RotateSun", cycleSpeed * 2, cycleSpeed);
}

function RotateSun() {
	transform.Rotate(-0.025, 0, 0);
}