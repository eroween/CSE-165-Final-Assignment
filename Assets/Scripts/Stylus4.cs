using UnityEngine;
using System.Collections;

//Stylus4: Push objects around with a collider in the stylus tip. 
//			Note: Due to the small scale of objects, it was necessary to set 
//					ProjectSettings/Physics/MinPenetrationForPenalty to 0.0001 
//					to prevent items sinking in to each other.

public class Stylus4 : MonoBehaviour
{
	
	private Quaternion initialRotation = new Quaternion();
	private Vector3 initialPosition = new Vector3();
	private ZSCore _zsCore;
	private ZSCore.TrackerTargetType _targetType = ZSCore.TrackerTargetType.Primary;

	protected void Start ()
	{
		_zsCore = GameObject.Find ("ZSCore").GetComponent<ZSCore> ();
		_zsCore.Updated += new ZSCore.CoreEventHandler (OnCoreUpdated);
		initialRotation = transform.rotation;
		initialPosition = transform.position;
	}
	
	/// called by ZSCore after each input update.
	private void OnCoreUpdated (ZSCore sender)
	{
		UpdateStylusPose ();
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

