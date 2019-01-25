using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


// Creates an instance of a primitive depending on the option selected by the user.
public class PlantEditorPopup : EditorWindow
{
    public Object[] options;
    public IList<string> names;
    string clone = "Clon";
    public int index = 0;
    public Material material;

    //[MenuItem("DeselectPlay/Editor GUILayout Popup usage")]
    [MenuItem("RustForms/PlantEditor")]
    static void Init()
    {
        EditorWindow window = GetWindow(typeof(PlantEditorPopup));
        window.Show();
    }

    void OnGUI()
    {
        options = Resources.LoadAll("Plants");//= new string[] { "Cube", "Sphere", "Plane" };
        names = new List<string>();
        foreach (var g in options)
        {
            //Debug.Log(g.name);
            names.Add(g.name);
        }

        string[] namesArray = new string[names.Count];
        names.CopyTo(namesArray, 0);


        index = EditorGUILayout.Popup(index, namesArray);

        //menu for entering the opening the material
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
    }


    void ClampRotation(Quaternion rotation, GameObject current)
    {
        var vec = current.transform.eulerAngles;
        vec.x = Mathf.Round(vec.x * 100f) / 100f;
        //Debug.Log(vec.x);
        vec.y = Mathf.Round(vec.y * 100f) / 100f;
        vec.y = Mathf.Round(vec.z * 100f) / 100f;

        current.transform.rotation = rotation;
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

            if (currentName.Contains(" "))
                currentName = currentName.Remove(currentName.IndexOf(" "));

            Debug.Log(currentName);

            if (names.Contains(currentName) && destroy)
            {
                DestroyImmediate(Selection.activeGameObject);
            }
            Selection.activeObject = PrefabUtility.InstantiatePrefab(newGuy);// tempPosition, tempRotation, parent);
            Selection.activeGameObject.transform.position = tempPosition;
            Selection.activeGameObject.transform.rotation = tempRotation;
            Selection.activeGameObject.transform.parent = parent;
        }

        else
        {
            Selection.activeObject = Instantiate(newGuy, tempPosition, tempRotation);
            Selection.activeGameObject.transform.position = tempPosition;
            Selection.activeGameObject.transform.rotation = tempRotation;
        }



        if (material)
            Selection.activeGameObject.GetComponent<Renderer>().material = material;
    }
}