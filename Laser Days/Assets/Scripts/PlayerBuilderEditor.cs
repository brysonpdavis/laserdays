using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(PlayerBuilderScript))]
public class PlayerBuilderEditor : Editor
{


    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        PlayerBuilderScript myScript = (PlayerBuilderScript)target;
        if (GUILayout.Button("Build Player"))
        {
            myScript.BuildPlayer(myScript.spawnSelect);

        }
    }
}