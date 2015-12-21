using UnityEngine;
using System.Collections;

/***** ABOUT *****
Some general helper classes/structs

Nick Vanheer
*****************/
public class Helpers {

    //z direction of point
	public static Vector3 GetTangent(Vector3[] pts, float t)
    {
        float omt = 1f - t;
        float omt2 = omt * omt;
        float t2 = t * t;

        Vector3 tangent =
            pts[0] * (-omt2) +
            pts[1] * (3 * omt2 - 2 * omt) +
            pts[2] * (-3 * t2 + 2 * t) +
            pts[3] * (t2);
        
        return tangent.normalized;
    }

    public static  Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)  
    {
        Vector3 dir = point - pivot; // get point direction relative to pivot
        dir = Quaternion.Euler(angles) * dir; // rotate it
        point = dir + pivot; // calculate rotated point
        return point; // return it
    }

    public static Vector3 GetNormal2D(Vector3[] pts, float t)
    {
        Vector3 tng = GetTangent(pts, t);
        return new Vector3(-tng.y, tng.x, 0f); // just return but rotated 90 degrees
    }

    //you need a reference vector up because the tangent can be the same but the other axises can be different
    public static Vector3 GetNormal3D(Vector3[] pts, float t, Vector2 up)
    {
        Vector3 tng = GetTangent(pts, t);
        Vector3 binormal = Vector3.Cross(up, tng).normalized;

        return Vector3.Cross(tng, binormal);
    }

    //handy to have a Quaternion for Unity
    Quaternion GetOrientation2D(Vector3[] pts, float t)
    {
        Vector3 tng = GetTangent(pts, t);
        Vector3 nrm = GetNormal2D(pts, t);
        return Quaternion.LookRotation(tng, nrm);
    }

    Quaternion GetOrientation3D(Vector3[] pts, float t, Vector3 up)
    {
        Vector3 tng = GetTangent(pts, t);
        Vector3 nrm = GetNormal3D(pts, t, up);
        return Quaternion.LookRotation(tng, nrm);
    }

    Vector3 GetPoint(Vector3[] pts, float t)
    {
        float omt = 1f - t;
        float omt2 = omt * omt;
        float t2 = t * t;
        return pts[0] * (omt2 * omt) +
                pts[1] * (3f * omt2 * t) +
                pts[2] * (3f * omt * t2) +
                pts[3] * (t2 * t);
    }

    public static T SafeDestroy<T>(T obj) where T : Object
    {
        if (Application.isEditor)
            Object.DestroyImmediate(obj);
        else
            Object.Destroy(obj);

        return null;
    }

    public static T SafeDestroyGameObject<T>(T component) where T : Component
    {
        if (component != null)
            SafeDestroy(component.gameObject);
        return null;
    }

}

public class ExtrudeShape
{
    public Vector2[] verts;
    public Vector2[] normals;
    public float[] us;
    
    public int[] lines = new int[] { 
            0,1,
            2,3,
            3,4,
            4,5
        };
}

public struct OrientedPoint
{
    public Vector3 Position;
    public Quaternion rotation;
    public Vector3 RawRotation;


    public OrientedPoint(Vector3 position, Quaternion rotation)
    {
        this.Position = position;
        this.rotation = rotation;
        this.RawRotation = rotation.eulerAngles;
    }

    public OrientedPoint(Vector3 position, Vector3 rotation)
    {
        this.Position = position;
        this.rotation = Quaternion.identity;
        this.RawRotation = rotation;
    }

    public Vector3 LocalToWorld(Vector3 point)
    {
        return Position + rotation * point;
    }

    public Vector3 WorldToLocal(Vector3 point)
    {
        return Quaternion.Inverse(rotation) * (point - Position);
    }

    public Vector3 LocalToWorldDirection(Vector3 dir)
    {
        return rotation * dir;
    }
}