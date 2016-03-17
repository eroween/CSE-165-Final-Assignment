using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OrbitController : MonoBehaviour {

    [System.Serializable]
    public struct OrbitPosition
    {
        public GameObject orbitObject;
        public Vector3 orbitPoint;
    }
    [System.Serializable]
    public struct OrbitShape
    {
        public float apoapsis;
        public float periapsis;

        public float eccentricity;
    };
    [System.Serializable]
    public struct OrbitAngle
    {
        public float eclipticInclination;
        public float ascendingNodeLongitude;
        public float periapsisArgument;
    };
    [System.Serializable]
    public struct BodySize
    {
        public float equatorialRadiusWidth;
        public float equatorialRadiusDepth;
        public float polarRadius;
    };

    /*
    ** Ressources
    */
    //Introduction
    //https://www.youtube.com/watch?v=82p-DYgGFjI
    //https://www.youtube.com/watch?v=CEH2HyVnKQM
    //Ellipse equation
    //https://en.wikipedia.org/wiki/ellipse
    //Orbit variables
    //https://en.wikipedia.org/wiki/Orbital_elements
    //https://en.wikipedia.org/wiki/Orbital_inclination
    //Rotation variables
    //https://en.wikipedia.org/wiki/Axial_tilt

    /*
    ** Static Variables
    */
    public static float timeMult = 1;
    public static float starSizeMult = 1;
    public static float planetSizeMult = 1;
    public static float moonSizeMult = 1;
    public static float smallBodySizeMult = 1;
    public static float smallMoonSizeMult = 1;
    public static float planetDistanceMult = 1;
    public static float moonDistanceMult = 1;
    public static float smallBodyDistanceMult = 1;
    public static float smallMoonDistanceMult = 1;
    public static float ellipseWidthMult = 1;

    /*
    ** Orbit Variables
    */
    public OrbitPosition orbitPosition;
    public OrbitShape orbitShape;
    public OrbitAngle orbitAngle;
    [Space(18)]
    public float orbitPeriod;
    public float orbitEccentricAnomaly;
    //Private Orbit correction variables
    public bool updated { get; private set; }
    private OrbitController fatherOrbit;
    //Private Orbit calculation variables
    private float orbitCenterOffset;
    private Vector3 orbitCenter;
    Quaternion orbitOrientation;
    private float orbitSemiMajorAxis;
    private float orbitSemiMinorAxis;

    /*
    ** Rotation variables
    */
    [Space(18)]
    public GameObject rotationBody;
    public BodySize rotationBodySize;
    public float rotationAxialTilt;
    public float rotationPeriod;
    public float rotationState;

    /*
    ** Ellipse variables
    */
    [Space(18)]
    public bool displayEllipse = true;
    private GameObject ellipseObject;
    private LineRenderer ellipseLine;
    public int ellipseResolution = 100;
    public float ellipseWidth = 1.0f;
    public static Material ellipseMaterial;

    /*
    ** Time variables
    */
    private static bool updating = false;
    private static float deltaTime;
    private static bool timeWarping = false;
    private bool warpScheduled = false;
    private float warpAngle;
    private bool isWarper = false;
    private bool hasWarped = false;

    /*
    ** Information Variables
    */
    [System.Serializable]
    public enum BodyType { Star, Planet, Moon, SmallBody, SmallMoon };
    [Space(18)]
    public BodyType bodyType;// { get; private set; }


    //Called by the Unity Editor
    public void OnValidate()
    {
        //Essentail Corrections
        orbitShape.eccentricity = Mathf.Clamp(orbitShape.eccentricity, 0.0f, 1.0f);
        //Non Essential Corrections
        orbitEccentricAnomaly -= 360.0f * Mathf.Floor(orbitEccentricAnomaly / 360.0f);
        orbitAngle.ascendingNodeLongitude -= 360.0f * Mathf.Floor(orbitAngle.ascendingNodeLongitude / 360.0f);
        orbitAngle.eclipticInclination -= 360.0f * Mathf.Floor(orbitAngle.eclipticInclination / 360.0f);
        orbitAngle.periapsisArgument -= 360.0f * Mathf.Floor(orbitAngle.periapsisArgument / 360.0f);
        rotationAxialTilt -= 360.0f * Mathf.Floor(rotationAxialTilt / 360.0f);

        /*
        ** Updates Position to reflect starting point
        */
        float tmpOrbitPeriod = orbitPeriod; orbitPeriod = 0;
        float tmpRotationPeriod = rotationPeriod; rotationPeriod = 0;
        updated = false;
        Update();
        orbitPeriod = tmpOrbitPeriod;
        rotationPeriod = tmpRotationPeriod;
        OrbitController oc;
        foreach (Transform child in transform)
        {
            oc = child.GetComponent<OrbitController>();
            if (oc) oc.OnValidate();
        }
    }

    // Use this for initialization
    private void Start () {
        BuildEllipse();
    }

    private void BuildEllipse()
    {
        //Build Ellipse object
        ellipseObject = new GameObject(this.name + " Ellipse");
        ellipseLine = ellipseObject.AddComponent<LineRenderer>();
        ellipseLine.material = ellipseMaterial;
        ellipseLine.useWorldSpace = false;
        ellipseLine.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        ellipseLine.receiveShadows = false;
    }

    // Update is called once per frame
    public void Update () {
        if (updated)
            return;
        updating = true;

        UpdateTime();
        UpdatesVariables();
        UpdateEllipse();
        UpdateRotation();
        UpdateOrbit();

        updated = true;

        //TEMPORARY CONTROLS
        if (this.name == "Earth Orbit" && Input.GetKeyUp("up"))
        {
            WarpOrbitPosition(this.orbitEccentricAnomaly + 15f);
        }
        else if (this.name == "Earth Orbit" && Input.GetKeyUp("down"))
        {
            WarpOrbitPosition(this.orbitEccentricAnomaly - 15f);
        }
        else if (this.name == "Earth Orbit" && Input.GetKey(KeyCode.Space))
        {
            WarpOrbitPosition(this.orbitEccentricAnomaly);
        }
    }

    private void UpdateTime()
    {
        if (timeWarping)
            hasWarped = true;
        else
            deltaTime = Time.deltaTime * timeMult;
    }

    //This function should be called only once if the orbit system was static
    private void UpdatesVariables()
    {
        //Set size to 1
        this.transform.localScale = Vector3.one;
        //Update fatherOrbit in case of change
        if (this.transform.parent)
            fatherOrbit = this.transform.parent.GetComponent<OrbitController>();
        //Update ellispe variables
        if (ellipseResolution < 3)
            ellipseResolution = 3;
        if (ellipseWidth < 0)
            ellipseWidth = 0;

        //Get correct simulation mult
        float distMult;
        float sizeMult;
        switch (bodyType)
        {
            case BodyType.Star: {
                    sizeMult = starSizeMult;
                    distMult = 1;
                } break;
            case BodyType.Planet: {
                    sizeMult = planetSizeMult;
                    distMult = planetDistanceMult;
                } break;
            case BodyType.Moon: {
                    sizeMult = moonSizeMult;
                    distMult = moonDistanceMult;
                } break;
            case BodyType.SmallBody: {
                    sizeMult = smallBodySizeMult;
                    distMult = smallBodyDistanceMult;
                } break;
            case BodyType.SmallMoon: {
                    sizeMult = smallMoonSizeMult;
                    distMult = smallMoonDistanceMult;
                } break;
            default: {
                    distMult = 1;
                    sizeMult = 1;
                } break;
        }

        //Update variables in case of public variables changes
        orbitCenterOffset = (orbitShape.apoapsis * distMult - orbitShape.periapsis * distMult) / 2f;
        orbitCenter = (orbitPosition.orbitObject) ? orbitPosition.orbitObject.transform.localPosition : orbitPosition.orbitPoint;
        orbitCenter.x -= orbitCenterOffset;
        orbitSemiMajorAxis = (orbitShape.periapsis * distMult + orbitShape.apoapsis * distMult) / 2f;
        orbitSemiMinorAxis = orbitSemiMajorAxis * Mathf.Sqrt(1.0f - Mathf.Pow(orbitShape.eccentricity, 2f));
        orbitOrientation = Quaternion.AngleAxis(orbitAngle.periapsisArgument, Vector3.down);
        orbitOrientation *= Quaternion.AngleAxis(orbitAngle.eclipticInclination, Quaternion.Inverse(orbitOrientation) * Vector3.forward);
        orbitOrientation *= Quaternion.AngleAxis(orbitAngle.ascendingNodeLongitude, Quaternion.Inverse(orbitOrientation) * Vector3.down);

        //If has father and rotates itself, apply orbit orientation correction
        if (fatherOrbit && !fatherOrbit.rotationBody)
        {
            //But first update father
            if (fatherOrbit.updated == false)
                fatherOrbit.Update();
            //Apply oposite of father rotation (we use Vector3.down by conversion, therefore apply rotation on Vector3.up for opposite)
            orbitOrientation *= Quaternion.AngleAxis(fatherOrbit.rotationState, Quaternion.Inverse(orbitOrientation) * Vector3.up);
        }

        //Update rotation body in case of simulation mult changes
        if (rotationBody)
        {
            rotationBody.transform.localPosition = (orbitPosition.orbitObject) ? orbitPosition.orbitObject.transform.localPosition : orbitPosition.orbitPoint;
            rotationBody.transform.localScale = Vector3.up * rotationBodySize.polarRadius * sizeMult +
                                        Vector3.right * rotationBodySize.equatorialRadiusDepth * sizeMult +
                                        Vector3.forward * rotationBodySize.equatorialRadiusWidth * sizeMult;
        }
        else
        {
            this.transform.localScale = Vector3.up * rotationBodySize.polarRadius * sizeMult +
                                        Vector3.right * rotationBodySize.equatorialRadiusDepth * sizeMult +
                                        Vector3.forward * rotationBodySize.equatorialRadiusWidth * sizeMult;
        }
    }

    private void UpdateEllipse() {
        if (!ellipseObject)
            return;

        //Update Ellipse Display
        ellipseObject.SetActive(displayEllipse);
        if (!displayEllipse)
            return;

        //Update Line Width
        ellipseLine.SetWidth(ellipseWidth * ellipseWidthMult, ellipseWidth * ellipseWidthMult);
        //IN CASE OF CHANGE : Set current parent
        ellipseObject.transform.SetParent(this.transform.parent, false);

        //Allocate Ellipse points
        ellipseLine.SetVertexCount(ellipseResolution + 1);

        //Create Ellispe points
        Vector3[] positions = new Vector3[ellipseResolution + 1];
        for (int i = 0; i <= ellipseResolution; i++)
        {
            float step = (float)i / (float)ellipseResolution * 360.0f;
            positions[i].x = orbitCenter.x + (orbitSemiMajorAxis * Mathf.Cos(step * Mathf.Deg2Rad));
            positions[i].z = orbitCenter.z + (orbitSemiMinorAxis * Mathf.Sin(step * Mathf.Deg2Rad));
            positions[i].y = orbitCenter.y;
            positions[i] = orbitOrientation * positions[i];
        }
        //Set Ellipse points
        ellipseLine.SetVertexCount(ellipseResolution + 1);
        for (int i = 0; i <= ellipseResolution; i++)
            ellipseLine.SetPosition(i, positions[i]);
    }

    private void UpdateOrbit() {
        Vector3 newPosition;

        //Increment angle
        if (orbitPeriod != 0)
            orbitEccentricAnomaly += (360.0f / orbitPeriod) * deltaTime;

        //Orbit on x z plane
        newPosition.x = orbitCenter.x + (orbitSemiMajorAxis * Mathf.Cos(orbitEccentricAnomaly * Mathf.Deg2Rad));
        newPosition.z = orbitCenter.z + (orbitSemiMinorAxis * Mathf.Sin(orbitEccentricAnomaly * Mathf.Deg2Rad));
        newPosition.y = orbitCenter.y;
        newPosition = orbitOrientation * newPosition;

        //Apply new position
        this.transform.localPosition = newPosition;

        //Avoid eccentricAnomaly overflow
        orbitEccentricAnomaly -= 360.0f * Mathf.Floor(orbitEccentricAnomaly / 360.0f);
    }

    private void UpdateRotation() {
        //Apply orbit orientation and tilt to local system
        this.transform.localRotation = orbitOrientation * Quaternion.AngleAxis(rotationAxialTilt, Vector3.forward);

        //Increment rotation
        if (rotationPeriod != 0)
            rotationState += 360f / rotationPeriod * deltaTime;

        //Commit new rotation
        if (rotationBody)
            rotationBody.transform.localRotation = Quaternion.AngleAxis(rotationState, Vector3.down);
        else
            this.transform.localRotation *= Quaternion.AngleAxis(rotationState, Vector3.down);

        //Avoid bodyRotationAngle overflow
        rotationState -= 360f * Mathf.Floor(rotationState / 360f);
    }

    // Late Update is called once per frame after all the Updates
    private void LateUpdate()
    {
        updating = false;
        updated = false;

        //Update Warp
        WarpStop();
        if (warpScheduled)
            WarpStart();
    }

    public void WarpOrbitPosition(float angle)
    {
        if (orbitPeriod == 0)
            return;
        warpScheduled = true;
        warpAngle = angle;
        if (updating == false)
            WarpStart();
    }

    private void WarpStart()
    {
        warpScheduled = false;
        float angleDiffStep = warpAngle - orbitEccentricAnomaly;
        //Requiered deltaTime to do the angleDiffStep
        deltaTime = (orbitPeriod / 360.0f) * angleDiffStep;
        //Apply requieredDeltaTime for this round of updates only
        timeWarping = true;
        isWarper = true;
        hasWarped = false;
    }

    private void WarpStop()
    {
        if (isWarper && hasWarped)
        {
            timeWarping = false;
            isWarper = false;
            hasWarped = false;
        }
    }

    public Vector3 GetOrbitCenter()
    {
        return orbitCenter;
    }

    public Vector3 GetOrbitNormal()
    {
        return orbitOrientation * Vector3.up;
    }

    public Vector3 GetOrbitReferenceDirection()
    {
        return orbitOrientation * Vector3.right;
    }

    /*public string GetInfo()
    {
        string info = "toto";

        info += "Physical characteristics\n"
        if (rotationBodySize.equatorialRadiusDepth == rotationBodySize.equatorialRadiusWidth)
        {

        }
        else
        {
            info += "Dimensions " + rotationBodySize.equatorialRadiusWidth + rotationBodySize.equatorialRadiusWidth
        }

        return info;
    }*/

}
