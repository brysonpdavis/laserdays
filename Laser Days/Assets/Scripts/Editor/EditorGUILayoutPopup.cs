using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


// Creates an instance of a primitive depending on the option selected by the user.
public class EditorGUILayoutPopup : EditorWindow
{
    public Object[] options;
    public IList<string> names;
    string clone = "Clon";
    public int index = 0;

    //[MenuItem("DeselectPlay/Editor GUILayout Popup usage")]
    [MenuItem("RustForms/Instantiator")]
    static void Init()
    {
        EditorWindow window = GetWindow(typeof(EditorGUILayoutPopup));
        window.Show();
    }

    void OnGUI()
    {
        options = Resources.LoadAll("EnvironmentTest");//= new string[] { "Cube", "Sphere", "Plane" };
        names = new List<string>();
        foreach (var g in options)
        {
            //Debug.Log(g.name);
            names.Add(g.name);
        }

        string[] namesArray = new string[names.Count];
        names.CopyTo(namesArray, 0);


        index = EditorGUILayout.Popup(index, namesArray);

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


    }

    void NewObject(bool destroy)
    {
        Vector3 tempPosition = new Vector3(0f, 0f, 0f);
        Quaternion tempRotation = new Quaternion(0f, 0f, 0f, 0f);

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
            Selection.activeObject = Instantiate(options[index], tempPosition, tempRotation, parent);
        }

        else Selection.activeObject = Instantiate(options[index], tempPosition, tempRotation);
    }
}