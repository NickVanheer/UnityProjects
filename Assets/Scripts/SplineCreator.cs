using UnityEngine;
using System.Collections.Generic;

/***** ABOUT *****
The class to create a spline consisting of multiple beziers. 
This component should be attached to an empty gameobject, 
buttons to add a spline will be shown in the Editor view

Nick Vanheer
*****************/

public class SplineCreator : MonoBehaviour {

    public List<SimpleBezier> Beziers = new List<SimpleBezier>();
    private GameObject bezierPrefab;

    public bool VisualizeNormals = false;
    public bool VisualizeInterpolationSteps = true;

    void Start () { }
	
	void Update () { }

    void OnDrawGizmos()
    {
        foreach (var bezier in Beziers)
        {
            bezier.VisualizeNormals = this.VisualizeNormals;
            bezier.VisualizeInterpolationSteps = this.VisualizeInterpolationSteps;
        }
    }

    public void Clear()
    {
        foreach (var bezier in Beziers)
        {
            Helpers.SafeDestroyGameObject(bezier);
        }

        Beziers.Clear();
    }

    public void SetGlobalInterpolationSteps(int steps)
    {
        foreach (var bezier in Beziers)
        {
            bezier.InterpolationSteps = steps;
            bezier.CalculateSplinePoints();
        }
    }

    //TODO: Improve (was added last minute as a hack to show a cool video that auto-updates the spline, but really could be implemented better)
    public void UpdateExtrudedMesh()
    {
        BezierShapeExtruder extruder = GetComponent<BezierShapeExtruder>();

        if(extruder != null && extruder.UseMeshObject != null && !extruder.DontLinkMeshGameObject)
        {
            extruder.ExtrudeAroundPointsButtonPressed();
        }
    }

    public void AddBezier()
    {
        if (bezierPrefab == null)
            bezierPrefab = Resources.Load("Prefabs/Bezier") as GameObject;

        GameObject bezierObject = Instantiate<GameObject>(bezierPrefab);
        bezierObject.transform.parent = this.transform;

        if (Beziers.Count == 0)
        {
            //create list and add object as first bezier curve
            Beziers = new List<SimpleBezier>();

            SimpleBezier bezier = bezierObject.GetComponent<SimpleBezier>();
            Beziers.Add(bezier);

            bezier.SetCreator(this);
            bezier.CalculateSplinePoints();
        }
        else
        {
            SimpleBezier previous = Beziers[Beziers.Count - 1];
            //add as second item, connect start point with end point of previous spline
            SimpleBezier bezier = bezierObject.GetComponent<SimpleBezier>();
            Beziers.Add(bezier);

            //use the same interpolation steps
            bezier.InterpolationSteps = previous.InterpolationSteps;

            bezier.MoveToPosition(previous.Parameters.EndPoint.x, previous.Parameters.EndPoint.z);
            bezier.SetLinked(previous);
            bezier.SetCreator(this);
        }
        

        


    }
}
