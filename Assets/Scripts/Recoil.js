#pragma strict

var recoilSpeed : float = 6; // Speed to move camera

function Start () {
}

function Update () {
	if (Input.GetMouseButtonDown(0)){
		recoilBack();
	}
	if (Input.GetMouseButtonUp(0)){
		recoilForward();
	}
}
// Move current weapon to zoomed in position smoothly over time
function MoveToPosition(newPosition : Vector3, time : float){
	var elapsedTime : float = 0;
	var startingPos = transform.position;
	while (elapsedTime < time){
		transform.position = Vector3.Lerp(startingPos, newPosition, (elapsedTime / time));
		elapsedTime += Time.deltaTime;
		yield;
	}
}
function recoilBack(){
	// Start coroutine to move the camera up smoothly over time
	var zoomOutOffset = Vector3(0, 0, 0.5);
	var zoomOutWorldPosition = transform.TransformDirection( zoomOutOffset );
	// Move the camera smoothly
	StartCoroutine(MoveToPosition(transform.position + zoomOutWorldPosition, recoilSpeed));
}
function recoilForward(){
	// Start coroutine to move the camera down smoothly over time
	var zoomInOffset = Vector3(0, 0, -0.5);
	var zoomInWorldPosition = transform.TransformDirection( zoomInOffset );
	// Move the camera smoothly
	StartCoroutine(MoveToPosition(transform.position + zoomInWorldPosition, recoilSpeed));
}