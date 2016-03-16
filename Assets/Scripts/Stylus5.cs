using UnityEngine;
using System.Collections;

//Stylus5: Demonstrates a snap-like characteristic where items close to the stylus tip turn black.
//			The collision mesh of the tip has been extended to x6. 
//			When user presses button, black object turns white. 

public class Stylus5 : MonoBehaviour
{
	
	private Quaternion initialRotation = new Quaternion();
	private Vector3 initialPosition = new Vector3();
	private ZSCore _zsCore;
	private ZSCore.TrackerTargetType _targetType = ZSCore.TrackerTargetType.Primary;
	
	public GameObject collidingWith; 
	
	protected void Start ()
	{
		_zsCore = GameObject.Find ("ZSCore").GetComponent<ZSCore> ();
		_zsCore.Updated += new ZSCore.CoreEventHandler (OnCoreUpdated);
		initialRotation = transform.rotation;
		initialPosition = transform.position;

		collidingWith = null;
	}
	
	/// called by ZSCore after each input update.
	private void OnCoreUpdated (ZSCore sender)
	{
		UpdateStylusPose ();
		if (collidingWith != null){
			if (_zsCore.IsTrackerTargetButtonPressed(ZSCore.TrackerTargetType.Primary, 0)){
				collidingWith.GetComponent<Renderer>().material.color = Color.white;
			}
			else{
				collidingWith.GetComponent<Renderer>().material.color = Color.black;
			}
		}
	}		
	private void UpdateStylusPose ()
	{
		Matrix4x4 pose = _zsCore.GetTrackerTargetWorldPose (_targetType);
		transform.position = new Vector3 (pose.m03 + initialPosition.x,
		                                  pose.m13 + initialPosition.y,
		                                  pose.m23 + initialPosition.z);
		transform.rotation = Quaternion.LookRotation(pose.GetColumn(2), pose.GetColumn(1))
			* initialRotation;
	}
}

