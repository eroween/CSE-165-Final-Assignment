using UnityEngine;
using System.Collections;

public class UnZoomButton : StylusButton
{
    public override void select()
    {
        ZoomController.UnZoom();
    }
}