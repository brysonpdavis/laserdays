using UnityEditor;
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

        if (GUILayout.Button("RotateY+90"))
        {
            GameObject current = (GameObject)Selection.activeObject;
            Undo.RecordObject(current.transform, "RotateY+90");
            Vector3 rot = new Vector3(0f, 90f, 0f);
            current.GetComponent<Transform>().Rotate(rot);
            //ClampRotation(current.transform.rotation, current);
        }

        if (GUILayout.Button("RotateY+180"))
        {
            GameObject current = (GameObject)Selection.activeObject;
            Undo.RecordObject(current.transform, "RotateY+180");
            Vector3 rot = new Vector3(0f, 180f, 0f);
            current.GetComponent<Transform>().Rotate(rot);
            //ClampRotation(current.transform.rotation, current);
        }

        if (GUILayout.Button("RotateX+90"))
        {
            GameObject current = (GameObject)Selection.activeObject;
            Undo.RecordObject(current.transform, "RotateX+90");
            Vector3 rot = new Vector3(90f, 0f, 0f);
            current.GetComponent<Transform>().Rotate(rot);
            //ClampRotation(current.transform.rotation, current);
        }

        if (GUILayout.Button("RotateZ+90"))
        {
            GameObject current = (GameObject)Selection.activeObject;
            Undo.RecordObject(current.transform, "RotateZ+90");
            Vector3 rot = new Vector3(0f, 0f, 90f);
            current.GetComponent<Transform>().Rotate(rot);
            //ClampRotation(current.transform.rotation, current);
        }

        ///movements
        if (GUILayout.Button("+X"))
        {
            GameObject current = (GameObject)Selection.activeObject;
            Undo.RecordObject(current.transform, "+X");
            Vector3 mov = new Vector3(4f, 0f, 0f);
            current.GetComponent<Transform>().Translate(mov);
        }

        if (GUILayout.Button("-X"))
        {
            GameObject current = (GameObject)Selection.activeObject;
            Undo.RecordObject(current.transform, "-X");
            Vector3 mov = new Vector3(-4f, 0f, 0f);
            current.GetComponent<Transform>().Translate(mov);
        }

        if (GUILayout.Button("+Y"))
        {
            GameObject current = (GameObject)Selection.activeObject;
            Undo.RecordObject(current.transform, "+Y");
            Vector3 mov = new Vector3(0f, 4f, 0f);
            current.GetComponent<Transform>().Translate(mov);
        }

        if (GUILayout.Button("-Y"))
        {
            GameObject current = (GameObject)Selection.activeObject;
            Undo.RecordObject(current.transform, "-Y");
            Vector3 mov = new Vector3(0f, -4f, 0f);
            current.GetComponent<Transform>().Translate(mov);
        }

        if (GUILayout.Button("+Z"))
        {
            GameObject current = (GameObject)Selection.activeObject;
            Undo.RecordObject(current.transform, "+Z");
            Vector3 mov = new Vector3(0f, 0f, 4f);
            current.GetComponent<Transform>().Translate(mov);
        }

        if (GUILayout.Button("-Z"))
        {
            GameObject current = (GameObject)Selection.activeObject;
            Undo.RecordObject(current.transform, "-Z");
            Vector3 mov = new Vector3(0f, 0f, -4f);
            current.GetComponent<Transform>().Translate(mov);
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
                Undo.DestroyObjectImmediate(Selection.activeGameObject);
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
        
        Undo.RegisterCreatedObjectUndo(Selection.activeGameObject, "Created New Object");
        
        



        if (material)
            Selection.activeGameObject.GetComponent<Renderer>().material = material;
    }
}