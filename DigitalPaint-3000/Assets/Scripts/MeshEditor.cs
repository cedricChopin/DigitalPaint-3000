using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(BezierCurvePath))]

public class MeshEditor : Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        BezierCurvePath myTarget = (BezierCurvePath)target;
        if(GUILayout.Button("Refresh Mesh")){
            myTarget.PointMesh();
        }

    }

}
