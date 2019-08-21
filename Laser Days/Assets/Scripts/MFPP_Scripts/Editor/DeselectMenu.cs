using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.ImageEffects;
using System.Reflection;

public class DeselectMenu : MonoBehaviour
{
    // Validated menu item.
    // Add a menu item named "Log Selected Transform Name" to MyMenu in the menu bar.
    // We use a second function to validate the menu item
    // so it will only be enabled if we have a transform selected.

    // Add a menu item named "Do Something with a Shortcut Key" to MyMenu in the menu bar
    // and give it a shortcut (ctrl-g on Windows, cmd-g on macOS).
    static Material myMaterial;

    private static double renameTime;
    private static GameObject go;


    [MenuItem("RustForms/Deselect All and Play %g")]
    static void DoSomethingWithAShortcutKey()
    {
        Debug.Log("Deselecting and Starting");
        Selection.objects = new UnityEngine.Object[0];
        EditorApplication.ExecuteMenuItem("Edit/Play");
    }

    [MenuItem("RustForms/Group %#g")]
    static void Group()
    {

        GameObject parent = new GameObject();
        parent.name = "New Group";
        parent.transform.parent = Selection.gameObjects[0].transform.parent;
        parent.transform.position = Selection.activeTransform.position;

        foreach (GameObject g in Selection.gameObjects)
        {
            g.transform.parent = parent.transform;
        }

        EditorApplication.update += EngageRenameMode;
        renameTime = EditorApplication.timeSinceStartup + 0.4d;
        EditorApplication.ExecuteMenuItem("Window/Hierarchy");
        Selection.activeGameObject = parent;

    }

    private static void EngageRenameMode()
    {
        if (EditorApplication.timeSinceStartup >= renameTime)
        {
            EditorApplication.update -= EngageRenameMode;
            var type = typeof(EditorWindow).Assembly.GetType("UnityEditor.SceneHierarchyWindow");
            var hierarchyWindow = EditorWindow.GetWindow(type);
            var rename = type.GetMethod("RenameGO", BindingFlags.Instance | BindingFlags.NonPublic);
            rename.Invoke(hierarchyWindow, null);
        }
    }



    [MenuItem("RustForms/Copy Material %#w")]
    static void CopyMaterial()
    {
        GameObject current = (GameObject)Selection.activeObject;

        if (current.GetComponent<Renderer>().sharedMaterial)
            myMaterial = current.GetComponent<Renderer>().sharedMaterial;
    }

    [MenuItem("RustForms/Paste Material %w")]
    static void PasteMaterial()
    {
        GameObject[] selectedMaterials = Selection.gameObjects;

        foreach (GameObject current in selectedMaterials)
        {
            if (myMaterial && current.GetComponent<Renderer>().sharedMaterial)
                current.GetComponent<Renderer>().sharedMaterial = myMaterial;
        }
    
    }

    [MenuItem("RustForms/Random Y %#e")]
    static void RandomY()
    {
        GameObject[] current = Selection.gameObjects;//(GameObject)Selection.activeObject;
        foreach (GameObject g in current)
        {
            g.transform.eulerAngles = new Vector3(g.transform.rotation.x, Random.Range(0, 359), g.transform.rotation.z);
        }
                                        
    }
    
    [MenuItem("RustForms/RobbyCross %r")]
    static void RobbyCross()
    {
        EdgeDetection ed = Camera.main.GetComponent<EdgeDetection>();
        if (ed)
        {
            ed.mode = EdgeDetection.EdgeDetectMode.RobertsCrossDepthNormals;
        }
    }

    [MenuItem("RustForms/NormalNormals %t")]
    static void NormalNormals()
    {
        EdgeDetection ed = Camera.main.GetComponent<EdgeDetection>();
        if (ed)
        {
            ed.mode = EdgeDetection.EdgeDetectMode.TriangleDepthNormals;
        }
    }


}