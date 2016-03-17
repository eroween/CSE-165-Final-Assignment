using UnityEngine;
using System.Collections;

//Stylus3: An even simpler stylus rendered with only a LineRenderer component, 
//			the main Stylus GameObject has no rendered GameObject children .


public class Stylus3 : MonoBehaviour
{
	
	private LineRenderer lr;

	private GameObject contactPoint;

	private Quaternion initialRotation = new Quaternion();
	private Vector3 initialPosition = new Vector3();
	private ZSCore _zsCore;
	private ZSCore.TrackerTargetType _targetType = ZSCore.TrackerTargetType.Primary;

    bool clicked = false, pressed = false;
    StylusButton btn = null;
    PlanetController pln = null;

    protected void Start()
	{
		_zsCore = GameObject.Find ("ZSCore").GetComponent<ZSCore> ();
		_zsCore.Updated += new ZSCore.CoreEventHandler (OnCoreUpdated);
		initialRotation = transform.rotation;
		initialPosition = transform.position;
		
		contactPoint = GameObject.Find("ContactPoint");
		contactPoint.SetActive(false);

		lr = gameObject.GetComponent<LineRenderer>();
	}
	
	public void Update(){
		RaycastHit hit;
		if (Physics.Raycast (transform.position,
		                     transform.forward,
		                     out hit)){
			lr.SetPosition(0,hit.point);
			lr.SetPosition(1,transform.position);
			contactPoint.SetActive(true);
			contactPoint.transform.position = hit.point;
            btn = hit.collider.gameObject.GetComponent<StylusButton>();
            pln = hit.collider.gameObject.GetComponent<PlanetController>();
		}
		else {
			lr.SetPosition(0,(transform.position
			                  +transform.forward)*10f);
			lr.SetPosition(1,transform.position);
			contactPoint.SetActive(false);
		}

        if (clicked == false && pressed == true)
        {
            if (btn)
            {
                clicked = true;
                btn.select();
            }
            else if (pln)
            {
                clicked = true;
                pln.select();
            }
        }
        if (pressed == true)
            MoveCellestialBody();
        pln = null;
        btn = null;
    }

    private void MoveCellestialBody()
    {
        OrbitController oc = GameObject.Find(pln.name + " Orbit").GetComponent<OrbitController>();
        Vector3 lp = transform.position;
        Vector3 lv = transform.forward;
        Vector3 pCenter = oc.GetOrbitCenter();
        Vector3 pNormal = oc.GetOrbitNormal();
        Vector3 pDir = oc.GetOrbitReferenceDirection();


    }

    private void OnCoreUpdated(ZSCore sender)
    {
        UpdateStylusPose();
        if (_zsCore.IsTrackerTargetButtonPressed(ZSCore.TrackerTargetType.Primary, 0))
        {
            pressed = true;
        }
        else
        {
            pressed = false;
            clicked = false;
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


