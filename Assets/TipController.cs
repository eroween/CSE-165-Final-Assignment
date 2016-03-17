using UnityEngine;
using System.Collections;

public class TipController : MonoBehaviour {

    private ZSCore _zsCore;

    GameObject go = null;
    StylusButton btn = null;

    bool clicked = false, pressed = false;

    // Use this for initialization
    void Start () {
        _zsCore = GameObject.Find("ZSCore").GetComponent<ZSCore>();
        _zsCore.Updated += new ZSCore.CoreEventHandler(OnCoreUpdated);
    }
	
	// Update is called once per frame
	void Update () {

        if (clicked == false && pressed == true)
        {
            if (btn)
            {
                clicked = true;
                btn.select();
            }
        }
	
	}

    void OnTriggerEnter(Collider other)
    {
        go = other.gameObject;
        btn = other.gameObject.GetComponent<StylusButton>();
    }

    void  OnTriggerExit(Collider other)
    {
        if (other.gameObject == go)
        {
            go = null;
            btn = null;
        }
    }

    private void OnCoreUpdated(ZSCore sender)
    {
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

}
