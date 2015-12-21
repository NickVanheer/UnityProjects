using UnityEditor;
using UnityEngine;

/***** ABOUT *****
The window class for the copy tool, 
which you can dock and find under the Spline/Copy along spline submenu.

Nick Vanheer
*****************/
public class SplineCopyWindow : EditorWindow {

    SplineCreator splinepath;
    GameObject gameObject;
    private int amount = 10;

    [MenuItem("Spline/Copy along spline")]
    static void ShowWindow()
    {
        var window = EditorWindow.GetWindow(typeof(SplineCopyWindow));
        window.name = "Copy along spline";
    }

    void OnGUI()
    {
        GUILayout.Label("Spline path object copy tool", EditorStyles.boldLabel);
        splinepath = EditorGUILayout.ObjectField("Path", splinepath, typeof(SplineCreator), true) as SplineCreator;
        gameObject = EditorGUILayout.ObjectField("Object", gameObject, typeof(GameObject), true) as GameObject;

        if (splinepath == null || gameObject == null)
            GUI.enabled = false;
        else
            GUI.enabled = true;

        amount = EditorGUILayout.IntField("Amount", amount); 

        GUILayout.Space(5);
        
        if (GUILayout.Button("Generate"))
        {
           float deltaT = 1.0f / amount;

            //calculate for every point along every spline evenly
            foreach (var spline in splinepath.Beziers)
            {
                for (int i = 0; i < amount; i++)
                {
                    var t = i * deltaT;
                    var point = spline.CalculateSplinePoint(t);

                    GameObject.Instantiate(gameObject, point, Quaternion.identity);
                }
            }
        }
    }
}
