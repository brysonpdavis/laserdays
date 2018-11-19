using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PerlinSpawnerScript))]
public class PerlinSpawnerEditor : Editor {

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        PerlinSpawnerScript myScript = (PerlinSpawnerScript)target;

        if (GUILayout.Button("Spawn"))
        {
            myScript.Spawn();

        }

    }
}
