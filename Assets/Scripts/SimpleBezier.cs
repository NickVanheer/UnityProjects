using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/***** ABOUT *****
The class to display one bezier curve

Nick Vanheer
*****************/

[Serializable]
public class BezierParameters
{
    public Vector3 StartPoint;//p0
    public Vector3 StartControlPoint; //p1
    public Vector3 EndPoint; //p3
    public Vector3 EndControlPoint; //p2
}

public class SimpleBezier : MonoBehaviour {
    
    public BezierParameters Parameters;
    public int InterpolationSteps = 20;

    public List<OrientedPoint> SplinePoints = new List<OrientedPoint>();

    public bool IsFlat = false;

    public SimpleBezier linkedWith;
    public bool HideEndPoint = false;
    public bool VisualizeNormals = false;
    public bool VisualizeInterpolationSteps = false;

    public float WireSmallRadius = 0.1f;
    Transform oldTransform;
    public SplineCreator LinkedSplineCreator;

	void Start () {
        CalculateSplinePoints();
        oldTransform = transform;
        MoveToPosition(transform.position.x, transform.position.z);
    }

	void Update () { }

    #region Public Methods
    public Vector3 CalculateSplinePoint(float t)
    {
        var r2 = Mathf.Pow((1 - t), 3) * Parameters.StartPoint; //p0
        r2 += 3 * Mathf.Pow((1 - t), 2) * t * Parameters.StartControlPoint; //p1
        r2 += 3 * (1 - t) * Mathf.Pow(t, 2) * Parameters.EndControlPoint; //p2
        r2 += Mathf.Pow(t, 3) * Parameters.EndPoint; //p3

        if (IsFlat)
            r2.Set(r2.x, 0, r2.z);

        //return point
        return r2;
    }

    public void CalculateSplinePoints()
    {
        SplinePoints.Clear();

        float step = 1.0f / (InterpolationSteps - 1);

        for (int i = 0; i < InterpolationSteps; ++i)
        {
            var t = i * step;
            var point = CalculateSplinePoint(t);
            var normal = CalculateNormal(t);

            OrientedPoint p = new OrientedPoint(point, normal);

            SplinePoints.Add(p);
        }
    }

    public void Offset(int x, int z)
    {
        this.Parameters.StartPoint += new Vector3(x, 0, z);
        this.Parameters.EndPoint += new Vector3(x, 0, z);

        this.Parameters.StartControlPoint += new Vector3(x, 0, z);
        this.Parameters.EndControlPoint += new Vector3(x, 0, z);
    }

    public void MoveToPosition(float x, float z)
    {
        this.Parameters.StartPoint = new Vector3(x, 0, z);
        this.Parameters.EndPoint += new Vector3(x, 0, z);

        this.Parameters.StartControlPoint += new Vector3(x, 0, z);
        this.Parameters.EndControlPoint += new Vector3(x, 0, z);
    }

    public void Move(float x, float z)
    {
        this.Parameters.StartPoint = new Vector3(x, 0, z);
    }

    public void SetLinked(SimpleBezier previous)
    {
        linkedWith = previous;

        previous.HideEndPoint = true;
        previous.Parameters.EndPoint = Parameters.StartPoint;
        previous.CalculateSplinePoints();
        CalculateSplinePoints();
    }

    public void SetCreator(SplineCreator creator)
    {
        LinkedSplineCreator = creator;
    }

    public void RotateControlPointsX()
    {
        if(IsFlat)
            IsFlat = false;

        Vector3 newStart = Helpers.RotatePointAroundPivot(Parameters.StartControlPoint, Parameters.StartPoint, new Vector3(90, 0, 0));
        Vector3 newEnd = Helpers.RotatePointAroundPivot(Parameters.EndControlPoint, Parameters.EndPoint, new Vector3(90, 0, 0));

        this.Parameters.StartControlPoint = newStart;
        this.Parameters.EndControlPoint = newEnd;
    }
    #endregion

    private void OnDrawGizmos()
    {
        if (IsFlat)
        {
            Parameters.StartPoint.Set(Parameters.StartPoint.x, 0, Parameters.StartPoint.z);
            Parameters.EndPoint.Set(Parameters.EndPoint.x, 0, Parameters.EndPoint.z);
            Parameters.StartControlPoint.Set(Parameters.StartControlPoint.x, 0, Parameters.StartControlPoint.z);
            Parameters.EndControlPoint.Set(Parameters.EndControlPoint.x, 0, Parameters.EndControlPoint.z);
        }

        DrawBezier();
        DrawControlPoints();

        if(VisualizeInterpolationSteps)
            DrawInterpolationSteps();

        if(VisualizeNormals)
            DrawNormals();

        DrawAdditional();
    }

    private void DrawBezier()
    {
        Gizmos.color = Color.white;
        if (SplinePoints.Count == 0)
            CalculateSplinePoints();

        for (int i = 0; i < InterpolationSteps - 1; ++i)
        {
            if (SplinePoints.Count - 1 > i)
                Gizmos.DrawLine(SplinePoints[i].Position, SplinePoints[i + 1].Position);
        }
    }

    private void DrawInterpolationSteps()
    {
        Gizmos.color = Color.green;
        for (int i = 0; i < InterpolationSteps; ++i)
        {
            if (SplinePoints.Count > i)
                Gizmos.DrawWireSphere(SplinePoints[i].Position, WireSmallRadius / 2);
        }
    }

    private void DrawNormals()
    {
        Gizmos.color = Color.red;
        for (int i = 0; i < InterpolationSteps; ++i)
        {
            if (SplinePoints.Count > i)
                Gizmos.DrawLine(SplinePoints[i].Position, SplinePoints[i].Position + (SplinePoints[i].RawRotation.normalized * 1));
        }
    }

    private void DrawAdditional()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(Parameters.EndPoint, WireSmallRadius);   
    }

    private void DrawControlPoints()
    {
        //Control points
        Gizmos.color = Color.magenta;

        Gizmos.DrawWireSphere(Parameters.StartControlPoint, WireSmallRadius);
        Gizmos.DrawWireSphere(Parameters.EndControlPoint, WireSmallRadius);

        //Line to control points
        Gizmos.color = Color.white;
        Gizmos.DrawLine(Parameters.StartPoint, Parameters.StartControlPoint);
        Gizmos.DrawLine(Parameters.EndPoint, Parameters.EndControlPoint);
    }

    private float CalculateBezierDerivative(float t, float a, float b, float c, float d)
    {
        a = 3 * (b - a);
        b = 3 * (c - b);
        c = 3 * (d - c);
        return a * Mathf.Pow(1 - t, 2) + 2 * b * (1 - t) * t + 3 * c * Mathf.Pow(t, 2);
    }

    Vector3 getTangent(float s)
    {
        float t = 1 - s;

        return (3 * t * t * (Parameters.StartControlPoint - Parameters.StartPoint) +
                 6 * s * t * (Parameters.EndControlPoint - Parameters.StartControlPoint) +
                 3 * s * s * (Parameters.EndPoint - Parameters.EndControlPoint));
    }

    private Vector3 CalculateNormal(float t)
    {
        Vector3 firstDerivative = getTangent(t);
        Vector3 normal = Vector3.Cross(firstDerivative, new Vector3(0, 1, 0)); // Z = forward
        //normal.normalise();

        return normal;
    }

}
