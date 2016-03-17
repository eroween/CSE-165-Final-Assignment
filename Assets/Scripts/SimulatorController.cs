using UnityEngine;
using System.Collections;

public class SimulatorController : MonoBehaviour
{
    public GameObject starSystem;
    [Space(18)]
    public float timeMult = 1;
    public float generalScaleMult = 1;
    [Space(18)]
    public float ellipseWidthMult = 1;
    public Material ellipseMaterial;
    [Space(18)]
    public float starSizeMult = 1;
    public float planetSizeMult = 1;
    public float smallBodySizeMult = 1;
    public float moonSizeMult = 1;
    public float smallMoonSizeMult = 1;
    [Space(18)]
    public float planetDistanceMult = 1;
    public float smallBodyDistanceMult = 1;
    public float moonDistanceMult = 1;
    public float smallMoonDistanceMult = 1;


    void OnValidate()
    {
        Update();
        if (!starSystem)
            starSystem = this.gameObject;
        OrbitController oc = starSystem.GetComponent<OrbitController>();
        if (oc) oc.OnValidate();
        foreach (Transform child in starSystem.transform)
        {
            oc = child.GetComponent<OrbitController>();
            if (oc) oc.OnValidate();
        }
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        OrbitController.timeMult  = timeMult;
        OrbitController.ellipseWidthMult = ellipseWidthMult;
        OrbitController.ellipseMaterial = ellipseMaterial;

        OrbitController.starSizeMult  = starSizeMult * generalScaleMult;
        OrbitController.planetSizeMult  = planetSizeMult * generalScaleMult;
        OrbitController.planetDistanceMult = planetDistanceMult * generalScaleMult;
        OrbitController.moonSizeMult  = moonSizeMult * generalScaleMult;
        OrbitController.moonDistanceMult = moonDistanceMult * generalScaleMult;
        OrbitController.smallBodySizeMult = smallBodySizeMult * generalScaleMult;
        OrbitController.smallBodyDistanceMult = smallBodyDistanceMult * generalScaleMult;
        OrbitController.smallMoonSizeMult = smallMoonSizeMult * generalScaleMult;
        OrbitController.smallMoonDistanceMult = smallMoonDistanceMult * generalScaleMult;
 
    }

}
