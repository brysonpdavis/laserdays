using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(PlayerBuilderScript))]
#if UNITY_EDITOR

public class PlayerBuilderEditor : Editor
{


    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        PlayerBuilderScript myScript = (PlayerBuilderScript)target;

        if (GUILayout.Button("See Spawnpoints"))
        {
            myScript.BuildList();

        }

        if (GUILayout.Button("Build Player"))
        {
            myScript.BuildPlayer(myScript.spawnSelect);

        }

        //if (GUILayout.Button("Reset Player Charge"))
       // {
        //    myScript.ResetCharge();

       // }
    }


}
#endif
