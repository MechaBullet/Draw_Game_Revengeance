var topColors = [new Color(), new Color(), new Color(), new Color()];
var bottomColors = [new Color(), new Color(), new Color(), new Color()];
var duration : float = 5;
var timeCycle : Transform;
var topColor : Color;
var bottomColor : Color;
private var newTopColor : Color;
private var newBottomColor : Color;
private var oldTopColor : Color;
private var oldBottomColor : Color;
var gradientPlane;
private var gradientLayer = 7;
private var gradientCam;;
private var t = 0.0f;
private var time = 0f;
 
function Awake () {
	newTopColor = topColors[0];
	oldTopColor = topColors[0];
	newBottomColor = bottomColors[0];
	oldBottomColor = bottomColors[0];
	topColor = topColors[0];
	bottomColor = bottomColors[0];
	gradientLayer = Mathf.Clamp(gradientLayer, 0, 31);
	if (!camera) {
		Debug.LogError ("Must attach GradientBackground script to the camera");
		return;
	}
	DrawPlane();
}

function DrawPlane() {
	camera.clearFlags = CameraClearFlags.Depth;
	camera.cullingMask = camera.cullingMask & ~(1 << gradientLayer);
	gradientCam = new GameObject("Gradient Cam", Camera).camera;
	gradientCam.depth = camera.depth-1;
	gradientCam.cullingMask = 1 << gradientLayer;
 
	var mesh = new Mesh();
	mesh.vertices = [Vector3(-100, .577, 1), Vector3(100, .577, 1), Vector3(-100, -.577, 1), Vector3(100, -.577, 1)];
	mesh.colors = [topColor, topColor, bottomColor, bottomColor];
	mesh.triangles = [0, 1, 2, 1, 3, 2];
 
	var mat = new Material("Shader \"Vertex Color Only\"{Subshader{BindChannels{Bind \"vertex\", vertex Bind \"color\", color}Pass{}}}");
	gradientPlane = new GameObject("Gradient Plane", MeshFilter, MeshRenderer);
	(gradientPlane.GetComponent(MeshFilter) as MeshFilter).mesh = mesh;
	gradientPlane.renderer.material = mat;
	gradientPlane.layer = gradientLayer;
}

function Update() {
	CheckTime();
	
	if (t < 1){ // while t below the end limit...
		// increment it at the desired rate every update:
		t += Time.deltaTime/duration;
	}
}
//Orbit Rotation:
//0 = Sunrise
//90 = Noon
//180 = Sunset
//270 = Midnight
function CheckTime() {
	time = Mathf.Round(timeCycle.rotation.eulerAngles.x);
	if (time == 330) {
		t = 0;
		oldTopColor = topColor;
		oldBottomColor = bottomColor;
		newTopColor = topColors[0];
		newBottomColor = bottomColors[0];
	}
	if (time == 10) {
		t = 0;
		oldTopColor = topColor;
		oldBottomColor = bottomColor;
		newTopColor = topColors[1];
		newBottomColor = bottomColors[1];
	}
	if (time == 150) {
		t = 0;
		oldTopColor = topColor;
		oldBottomColor = bottomColor;
		newTopColor = topColors[2];
		newBottomColor = bottomColors[2];
	}
	if (time == 190) {
		t = 0;
		oldTopColor = topColor;
		oldBottomColor = bottomColor;
		newTopColor = topColors[3];
		newBottomColor = bottomColors[3];
	}
	topColor = Color.Lerp(oldTopColor, newTopColor, t);
	bottomColor = Color.Lerp(oldBottomColor, newBottomColor, t);
	(gradientPlane.GetComponent(MeshFilter) as MeshFilter).mesh.colors = [topColor, topColor, bottomColor, bottomColor];
}