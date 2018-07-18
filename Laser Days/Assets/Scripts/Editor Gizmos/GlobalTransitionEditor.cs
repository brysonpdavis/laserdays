using UnityEngine;
using System.Collections.Generic;
using UnityEditor;


[CustomEditor(typeof(GlobalTransitionScript))]
public class GlobalTransitionEditor : Editor {

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        GlobalTransitionScript myScript = (GlobalTransitionScript)target;
       
        if (GUILayout.Button("See All"))
        {
            myScript.GlobalAll();

        }

        if (GUILayout.Button("See Real"))
        {
            myScript.GlobalReal();

        }

        if (GUILayout.Button("See Laser"))
        {
            myScript.GlobalLaser();

        }


    }

}
