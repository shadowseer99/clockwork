using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(BGBuilderScript))]
public class BGBuilderEditor : Editor
{
    
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        BGBuilderScript myScript = (BGBuilderScript)target;

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