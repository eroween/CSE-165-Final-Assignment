using UnityEngine;
using System.Collections;

public class Stylus5Tip : MonoBehaviour
{
	private Color saveColor;
	Stylus5 stylusGO;
	void Start(){
		stylusGO = (Stylus5)FindObjectOfType(typeof(Stylus5));
	}
	void OnCollisionEnter(Collision other){
		stylusGO.collidingWith = other.gameObject;
		saveColor = other.gameObject.GetComponent<Renderer>().material.color;
		GetComponent<Renderer>().material.color = Color.red;
	}
	void OnCollisionStay(Collision other){
		//Debug.Log ("colliding");
	}
	void OnCollisionExit(Collision other){
		stylusGO.collidingWith = null;
		GetComponent<Renderer>().material.color = Color.green;
		other.gameObject.GetComponent<Renderer>().material.color = saveColor;
	}
}

