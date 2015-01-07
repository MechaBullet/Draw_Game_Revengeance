#pragma strict

var speed : float = 10.0;
var minimumY = 188.401 - 60;
var maximumY = 188.401 + 60; 
var rotationY = 0;

function Start () {
}

function Update(){ 
		rotationY += Input.GetAxis("Mouse Y") * speed;
		rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);
				
		transform.localEulerAngles = new Vector3(0, -rotationY, 0);
} 