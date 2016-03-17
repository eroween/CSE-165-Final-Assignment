using UnityEngine;
using System.Collections;

public class ZoomButton : StylusButton
{
    public override void select()
    {
        ZoomController.Zoom();
    }
}