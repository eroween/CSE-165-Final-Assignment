using UnityEngine;
using System.Collections;

public abstract class PlanetController : MonoBehaviour {
    public abstract void select();
    public GameObject ParentSystem;
    public GameObject System;
    public GameObject MiniSystem;
}
