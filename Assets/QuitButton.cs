using UnityEngine;
using System.Collections;
using System;

public class QuitButton : StylusButton {

    public override void select(){
        try
        {
#if UNITY_EDITOR_WIN
            if (Application.isEditor)
            {
                UnityEditor.EditorApplication.isPlaying = false;
            }
            else {
                Application.Quit();
            }
#else
			Application.Quit ();
#endif
        }
        catch (Exception e)
        {
            Debug.Log(e.Message + " from " + e.StackTrace);
        }
    }
}
