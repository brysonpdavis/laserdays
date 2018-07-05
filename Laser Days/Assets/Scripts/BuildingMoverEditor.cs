using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(BuildingMoverScript))]
public class BuildingMoverEditor : Editor
{


    public override void OnInspectorGUI()
    {

                 
        //GUILayout.SelectionGrid(selectedAction, actionLabels, 3, GUILayout.Width(width), GUILayout.Height(height));
        DrawDefaultInspector();
        BuildingMoverScript myScript = (BuildingMoverScript)target;

        if (GUILayout.Button("Move Forward"))
        {myScript.MoveForward();}

        if (GUILayout.Button("Move Backward"))
        {myScript.MoveBackward();}

        if (GUILayout.Button("Move Right"))
        { myScript.MoveRight(); }

        if (GUILayout.Button("Move Left"))
        { myScript.MoveLeft(); }

        //empty slot
        if (GUILayout.Button("--------"))
        { }


        if (GUILayout.Button("Rotate Clockwise"))
        { myScript.RotateClockwise(); }


        if (GUILayout.Button("Rotate Counter"))
        { myScript.RotateCounter(); }

        //empty slot
        if (GUILayout.Button("--------"))
        { }

        if (GUILayout.Button("Move Up"))
        { myScript.MoveUp(); }

        if (GUILayout.Button("Move Down"))
        { myScript.MoveDown(); }
    }



}