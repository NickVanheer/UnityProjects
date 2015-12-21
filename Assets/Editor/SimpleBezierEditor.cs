using UnityEditor;
using UnityEngine;

/***** ABOUT *****
The custom editor class which will appear on the SimpleBezier component.
Also handles drawing of the movement handles.

Nick Vanheer
*****************/

[CustomEditor(typeof(SimpleBezier))]
public class SimpleBezierEditor : Editor {
   
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Space(20);
        if (GUILayout.Button("Refresh"))
        {
            ((SimpleBezier)target).CalculateSplinePoints();

            EditorUtility.SetDirty(((SimpleBezier)target));
        }
    }

    void OnSceneGUI()
    {
        var simpleSpline = (SimpleBezier)target;

        simpleSpline.Parameters.StartControlPoint = Handles.FreeMoveHandle(simpleSpline.Parameters.StartControlPoint, Quaternion.identity, 0.2f, Vector3.zero, Handles.CircleCap);
        simpleSpline.Parameters.EndControlPoint = Handles.FreeMoveHandle(simpleSpline.Parameters.EndControlPoint, Quaternion.identity, 0.2f, Vector3.zero, Handles.CircleCap);

        Vector3 originalStart = simpleSpline.Parameters.StartPoint;
        Vector3 originalEnd = simpleSpline.Parameters.EndPoint;

        Vector3 moveStart = simpleSpline.Parameters.StartPoint;
        Vector3 moveEnd = simpleSpline.Parameters.EndPoint; 

        moveStart = Handles.FreeMoveHandle(simpleSpline.Parameters.StartPoint, Quaternion.identity, 0.2f, Vector3.zero, Handles.RectangleCap);

        if(!simpleSpline.HideEndPoint)
            moveEnd = Handles.FreeMoveHandle(simpleSpline.Parameters.EndPoint, Quaternion.identity, 0.2f, Vector3.zero, Handles.RectangleCap);

        Vector3 startDelta = moveStart - originalStart;
        Vector3 endDelta = moveEnd - originalEnd;

        simpleSpline.Parameters.StartPoint += startDelta;
        simpleSpline.Parameters.StartControlPoint += startDelta;

        simpleSpline.Parameters.EndPoint += endDelta;
        simpleSpline.Parameters.EndControlPoint += endDelta;

        if (startDelta.magnitude > 0)
        {
            if (simpleSpline.linkedWith != null && simpleSpline.linkedWith != simpleSpline)
            {
                simpleSpline.linkedWith.Parameters.EndPoint = simpleSpline.Parameters.StartPoint;
                simpleSpline.linkedWith.CalculateSplinePoints();
                EditorUtility.SetDirty(simpleSpline.linkedWith);
            }
        }


        if (GUI.changed)
        {
            simpleSpline.CalculateSplinePoints();
            EditorUtility.SetDirty(simpleSpline);

            //over 9000 hack for proof but it works
            if (simpleSpline.LinkedSplineCreator != null)
            {
                simpleSpline.LinkedSplineCreator.UpdateExtrudedMesh();
            }
            
            //there was a change, so if we enabled live updating, try to update the mesh too

        }
    }

}


