using UnityEngine;
using UnityEditor;
using System.Collections;

/***** ABOUT *****
The custom editor class which will appear on the BezierShapeExtruder component

Nick Vanheer
*****************/

[CustomEditor(typeof(BezierShapeExtruder))]
public class SplineShapeExtruderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Space(20);

        if (GUILayout.Button("Extrude"))
        {
            ((BezierShapeExtruder)target).ExtrudeAroundPointsButtonPressed();
        }

        GUILayout.Space(5);
        if (GUILayout.Button("Clear mesh"))
        {
            ((BezierShapeExtruder)target).ClearMeshButtonPress();
        }
    }
}
