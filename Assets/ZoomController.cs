using UnityEngine;
using System.Collections;

public class ZoomController : MonoBehaviour {

    static public PlanetController selected;
    static public PlanetController actual;

    void Start()
    {
        actual = GameObject.Find("Sun").GetComponent<SunController>();
        selected = actual;
    }

    static public void Zoom()
    {
        if (selected == actual)
            return;
        actual.System.active = false;
        if (selected.System)
            selected.System.active = true;
        if (selected.MiniSystem)
            selected.MiniSystem.SetActive(true);
        actual = selected;
    }

    static public void UnZoom()
    {
        if (actual.ParentSystem) {
            actual.ParentSystem.SetActive(true);
            if (actual.MiniSystem)
                actual.MiniSystem.SetActive(false);
            actual.System.SetActive(false);
            actual = GameObject.Find("Sun").GetComponent<SunController>();
        }
    }

}
