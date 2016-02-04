using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(WallBuilderScript))]
public class WallBuilderEditor : Editor
{
    
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        WallBuilderScript myScript = (WallBuilderScript)target;

        if (GUILayout.Button("Build Object"))
        {
            myScript.BuildObject();
        }
        if (GUILayout.Button("Kill Children"))
        {
            myScript.KillChildren();
        }
        SceneView.RepaintAll();
    }
}