using UnityEngine;
using System.Collections.Generic;

/***** ABOUT *****
The class to extrude a plane along a single bezier, or along a bezier group (i.e the splinecreator)

Nick Vanheer
*****************/
public class BezierShapeExtruder : MonoBehaviour
{
    public GameObject UseMeshObject;

    public bool DontLinkMeshGameObject;

    public bool UpwardsExtrusion = false;
    public float ExtrudeWidth = 1;

    private SplineCreator SplineCreator;
    private bool HasSplineComponent;

    public void ExtrudeAroundPoints(Mesh mesh, List<SimpleBezier> splines)
    {
        int Segments = 0;
        int EdgeLoops = 0;
        int VertCount = 0;
        int TriIndexCount = 0;

        int change = 0;

        foreach (var spline in splines)
        {
            int curSegments = spline.SplinePoints.Count - 1 - change;
            int curEdgeLoops = spline.SplinePoints.Count - change;
            int curVertCount = curEdgeLoops * 2; //number of total vertices
            int curTriIndexCount = (6 * curSegments); //2 triangles per segment so 6 index locations

            Segments += curSegments;
            EdgeLoops += curEdgeLoops;
            VertCount += curVertCount;
            TriIndexCount += curTriIndexCount;
        }

        int[] triangleIndices = new int[TriIndexCount];
        Vector3[] vertices = new Vector3[VertCount];
        int vIndex = 0;
        int iIndex = 0;
        int cCount = 0;

        int splineIndex = 0;
        foreach (var spline in splines)
        {
            //we're not calculating uvs, only vertices and indices
            float height = ExtrudeWidth;

            for (int i = 0; i < spline.SplinePoints.Count; i++)
            {
                //if we're at the first point of every spline except the first skip that, cause that's already been added by the previous spline
                if (splineIndex > 0 && i == 0)
                    continue; //account for this in indices too? -> not needed


                if (UpwardsExtrusion)
                {
                    Vector3 norm = new Vector3(0, height, 0);
                    vertices[vIndex] = new Vector3(spline.SplinePoints[i].Position.x, spline.SplinePoints[i].Position.y, spline.SplinePoints[i].Position.z); vIndex++;
                    vertices[vIndex] = new Vector3(spline.SplinePoints[i].Position.x, spline.SplinePoints[i].Position.y + norm.y, spline.SplinePoints[i].Position.z); vIndex++;
                }
                else
                {
                    Vector3 norm = spline.SplinePoints[i].RawRotation.normalized * height;
                    vertices[vIndex] = new Vector3(spline.SplinePoints[i].Position.x - norm.x, spline.SplinePoints[i].Position.y /* - norm.z */, spline.SplinePoints[i].Position.z - norm.z); vIndex++;
                    vertices[vIndex] = new Vector3(spline.SplinePoints[i].Position.x + norm.x, spline.SplinePoints[i].Position.y /* + norm.y */, spline.SplinePoints[i].Position.z + norm.z); vIndex++;
                }
            }

            //indices
            int segments = spline.SplinePoints.Count - 1 - change;
            int edgeLoops = spline.SplinePoints.Count - change;
            int vertCount = edgeLoops * 2;

            for (int i = 0; i < segments; i++)
            {
                //4 vertices
                int vIndexA = iIndex;
                int vIndexB = iIndex + 1;
                int vIndexC = iIndex + 2;
                int vIndexD = iIndex + 3;

                triangleIndices[cCount] = vIndexA; cCount++;
                triangleIndices[cCount] = vIndexB; cCount++;
                triangleIndices[cCount] = vIndexC; cCount++;

                triangleIndices[cCount] = vIndexC; cCount++;
                triangleIndices[cCount] = vIndexB; cCount++;
                triangleIndices[cCount] = vIndexD; cCount++;

                iIndex += 2;
            }

            splineIndex++;
        }

        //
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangleIndices;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        //mesh.uv = uvs;
    }

    private Mesh CreateNewExtrudeMeshObject()
    {
        Mesh mesh;
        UseMeshObject = new GameObject();
        UseMeshObject.name = "ExtrudedPath";
        MeshRenderer renderer = UseMeshObject.AddComponent<MeshRenderer>();

        MeshFilter mf = UseMeshObject.AddComponent<MeshFilter>();
        if (mf.sharedMesh == null)
            mf.sharedMesh = new Mesh();
        mesh = mf.sharedMesh;

        Material newMat = Resources.Load("Materials/GrayMaterial", typeof(Material)) as Material;

        if(newMat != null)
            renderer.material = newMat;

        return mesh;
    }

    public void ExtrudeAroundPointsButtonPressed()
    {
        Mesh mesh = null;

        //
        if (DontLinkMeshGameObject || UseMeshObject == null)
            mesh = CreateNewExtrudeMeshObject();
        else
            mesh = UseMeshObject.GetComponent<MeshFilter>().sharedMesh;
        
        //get the current spline component if there's any
        SplineCreator = GetComponent<SplineCreator>();

        if (SplineCreator != null)
            ExtrudeAroundPoints(mesh, SplineCreator.Beziers);
        else
            Debug.LogError("A SplineCreator component with splines needs to be attached to this GameObject in order to extrude a path");
    }

    public void SetSplineCreator(SplineCreator creator)
    {
        SplineCreator = creator;
        HasSplineComponent = false;
    }

    public void ClearMeshButtonPress()
    {
        if (UseMeshObject == null)
            return;

        MeshFilter mf = UseMeshObject.GetComponent<MeshFilter>();
        if (mf.sharedMesh == null)
            mf.sharedMesh = new Mesh();
        Mesh mesh = mf.sharedMesh;

        mesh.Clear();
    }
}
