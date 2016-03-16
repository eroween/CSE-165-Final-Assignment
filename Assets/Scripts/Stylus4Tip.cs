using UnityEngine;
using System.Collections;

public class Stylus4Tip : MonoBehaviour
{

	void OnCollisionEnter(Collision other){
		GetComponent<Renderer>().material.color = Color.red;
	}
	void OnCollisionStay(Collision other){
		//Debug.Log ("colliding");
	}
	void OnCollisionExit(Collision other){
		GetComponent<Renderer>().material.color = Color.green;
	}
	
}

