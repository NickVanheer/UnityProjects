using UnityEngine;
using System.Collections;
using UnityEditor;

/***** ABOUT *****
The custom editor class which will appear on the SplineCreator component

Nick Vanheer
*****************/

[CustomEditor(typeof(SplineCreator))]
public class SplineCreatorEditor : Editor {

    float interpolationSteps = 12;
    public override void OnInspectorGUI()
    {
        GUILayout.Space(10);
        if (GUILayout.Button("Add spline"))
            ((SplineCreator)target).AddBezier();

        GUILayout.Space(2);
        if (GUILayout.Button("Clear"))
            ((SplineCreator)target).Clear();

        GUILayout.Label(new GUIContent("Interpolation steps: "));
        float value = GUILayout.HorizontalSlider(interpolationSteps, 2, 18);

        if (value != interpolationSteps)
        {
            ((SplineCreator)target).SetGlobalInterpolationSteps((int)value);
            interpolationSteps = value;
        }

        base.OnInspectorGUI();
    }
}
