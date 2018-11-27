﻿using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


// Creates an instance of a primitive depending on the option selected by the user.
public class WallEditorPopup : EditorWindow
{
    public Object[] options;
    public IList<string> names;
    string clone = "Clon";
    public int index = 0;
    public Material material;

    //[MenuItem("DeselectPlay/Editor GUILayout Popup usage")]
    [MenuItem("RustForms/WallEditor")]
    static void Init()
    {
        EditorWindow window = GetWindow(typeof(WallEditorPopup));
        window.Show();
    }

    void OnGUI()
    {
        options = Resources.LoadAll("Wall");//= new string[] { "Cube", "Sphere", "Plane" };
        names = new List<string>();
        foreach (var g in options)
        {
            //Debug.Log(g.name);
            names.Add(g.name);
        }

        string[] namesArray = new string[names.Count];
        names.CopyTo(namesArray, 0);


        index = EditorGUILayout.Popup(index, namesArray);

       
        GUILayout.BeginHorizontal();
        material = (Material)EditorGUILayout.ObjectField("material", material, typeof(Material));
        GUILayout.EndHorizontal();

        //builds new object at the same transform as selected obj
        if (GUILayout.Button("Create"))
        {
            NewObject(false);
        }

        //builds new object at same transform as selected object, 
        //if the object being replaced is in the list, it removes it
        if (GUILayout.Button("Replace"))
        {
            NewObject(true);
        }

        if (GUILayout.Button("RotateY+90"))
        {
            GameObject current = (GameObject)Selection.activeObject;
            Vector3 rot = new Vector3(0f, 90f, 0f);
            current.GetComponent<Transform>().Rotate(rot);
        }

        if (GUILayout.Button("RotateY+180"))
        {
            GameObject current = (GameObject)Selection.activeObject;
            Vector3 rot = new Vector3(0f, 180f, 0f);
            current.GetComponent<Transform>().Rotate(rot);
        }

        if (GUILayout.Button("RotateX+90"))
        {
            GameObject current = (GameObject)Selection.activeObject;
            Vector3 rot = new Vector3(90f, 0f, 0f);
            current.GetComponent<Transform>().Rotate(rot);
        }

        if (GUILayout.Button("RotateZ+90"))
        {
            GameObject current = (GameObject)Selection.activeObject;
            Vector3 rot = new Vector3(0f, 0f, 90f);
            current.GetComponent<Transform>().Rotate(rot);
        }

        if (GUILayout.Button("+X"))
        {
            GameObject current = (GameObject)Selection.activeObject;
            Vector3 mov = new Vector3(4f, 0f, 0f);
            current.GetComponent<Transform>().Translate(mov);
        }

        if (GUILayout.Button("-X"))
        {
            GameObject current = (GameObject)Selection.activeObject;
            Vector3 mov = new Vector3(-4f, 0f, 0f);
            current.GetComponent<Transform>().Translate(mov);
        }

        if (GUILayout.Button("+Y"))
        {
            GameObject current = (GameObject)Selection.activeObject;
            Vector3 mov = new Vector3(0f, 4f, 0f);
            current.GetComponent<Transform>().Translate(mov);
        }

        if (GUILayout.Button("-Y"))
        {
            GameObject current = (GameObject)Selection.activeObject;
            Vector3 mov = new Vector3(0f, -4f, 0f);
            current.GetComponent<Transform>().Translate(mov);
        }

        if (GUILayout.Button("+Z"))
        {
            GameObject current = (GameObject)Selection.activeObject;
            Vector3 mov = new Vector3(0f, 0f, 4f);
            current.GetComponent<Transform>().Translate(mov);
        }

        if (GUILayout.Button("-Z"))
        {
            GameObject current = (GameObject)Selection.activeObject;
            Vector3 mov = new Vector3(0f, 0f, -4f);
            current.GetComponent<Transform>().Translate(mov);
        }

    }

    void NewObject(bool destroy)
    {
        Vector3 tempPosition = new Vector3(0f, 0f, 0f);
        Quaternion tempRotation = new Quaternion(0f, 0f, 0f, 0f);

        GameObject temp = (UnityEngine.GameObject)options[index];
        GameObject newGuy = new GameObject();
        newGuy = temp;
        //newGuy.GetComponent<Renderer>().material = material;

        if (Selection.objects.Length > 0)
        {
            Transform parent = Selection.activeGameObject.GetComponent<Transform>().parent;
            tempPosition = Selection.activeGameObject.GetComponent<Transform>().position;
            tempRotation = Selection.activeGameObject.GetComponent<Transform>().rotation;
            string currentName = Selection.activeGameObject.name;
            currentName = currentName.Replace("(Clone)", "");

            Debug.Log(currentName);
            if (names.Contains(currentName) && destroy)
            {
                DestroyImmediate(Selection.activeGameObject);
            }
            Selection.activeObject = Instantiate(newGuy, tempPosition, tempRotation, parent);
        }

        else Selection.activeObject = Instantiate(newGuy, tempPosition, tempRotation);


        if (material)
            Selection.activeGameObject.GetComponent<Renderer>().material = material;
    }
}