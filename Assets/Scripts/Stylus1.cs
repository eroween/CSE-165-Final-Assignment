using UnityEngine;
using System.Collections;

//Stylus1: Demonstrates how to build a custom stylus composed of multiple GameObject primitives
//			Shows how to update position in 6 degrees of freedom with zSpace's OnCoreUpdated()
//			Obviously you could create any stylus model you want by replacing the Stem and Tip 
//			with your own GameObjects. If you look at the scale of the Stylus Stem, you can see 
//			it is .08 along the z-axis, which makes it 8cm in length. The stem is centered at the 
//			zSpace Stylus tip, so I move it -4cm along the z-axis to have it fully extend out from 
//			the stylus tip into the screen. So if you create your own stem, then just move it along 
//			half its length to get the same effect. I also added the green Stylus Tip, just to 
//			clarify how it is positioned to respect a natural 6 degree of freedom movement. 

public class Stylus1 : MonoBehaviour
{
	private Quaternion initialRotation = new Quaternion();
	private Vector3 initialPosition = new Vector3();
	private ZSCore _zsCore;
	private ZSCore.TrackerTargetType _targetType = ZSCore.TrackerTargetType.Primary;

	public void Start(){
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

		transform.rotation = Quaternion.LookRotation(pose.GetColumn(2), 
		                                             pose.GetColumn(1))
														* initialRotation;

	}
}
