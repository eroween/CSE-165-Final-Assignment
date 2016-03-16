using UnityEngine;
using System.Collections;
using System;

public class CameraScript : MonoBehaviour
{

	void Start ()
	{
		Cursor.visible = false;
		transform.position = new Vector3(0f,.345f,-.222f);
		//transform.position = new Vector3(0f,100.345f,-100.222f);
		transform.LookAt (Vector3.zero);
		//ZSCore zs = GameObject.Find ("ZSCore").GetComponent<ZSCore> ();
		//zs.SetWorldScale(5f);
	}

	void ExitApp ()
	{
		try {
			#if UNITY_EDITOR_WIN
				if (Application.isEditor) {
					UnityEditor.EditorApplication.isPlaying = false;
				} else {
					Application.Quit ();
				}
			#else
			Application.Quit ();
			#endif
		} catch (Exception e) {
			Debug.Log (e.Message + " from " + e.StackTrace);
		}
	}

}
