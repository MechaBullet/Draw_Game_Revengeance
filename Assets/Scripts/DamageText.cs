using UnityEngine;
using System.Collections;

public class DamageText : MonoBehaviour {

	public Color color = new Color(0.8f,0.8f,0f,1.0f);
	public float scroll = 0.05f; // scrolling velocity
	public float duration = 1.5f; // time to die
	public float alpha;

	void Start(){
		guiText.material.color = color; // set text color
		alpha = 1;
	}

	void Update(){
		if (alpha>0){
			Vector3 textPos = transform.position;
			textPos.y += scroll * Time.deltaTime;
			transform.position = textPos;
			alpha -= Time.deltaTime/duration;
			Color textColor = guiText.material.color;
			textColor.a = alpha;
			guiText.material.color = textColor;
		} else {
			Destroy(gameObject); // text vanished - destroy itself
		}
	}
}
