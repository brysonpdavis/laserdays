using UnityEditor;
using UnityEngine;
public class DeselectMenu : MonoBehaviour
{
    // Validated menu item.
    // Add a menu item named "Log Selected Transform Name" to MyMenu in the menu bar.
    // We use a second function to validate the menu item
    // so it will only be enabled if we have a transform selected.

    // Add a menu item named "Do Something with a Shortcut Key" to MyMenu in the menu bar
    // and give it a shortcut (ctrl-g on Windows, cmd-g on macOS).
    [MenuItem("RustForms/Deselect All and Play %g")]
    static void DoSomethingWithAShortcutKey()
    {
        Debug.Log("Deselecting and Starting");
        Selection.objects = new UnityEngine.Object[0];
        EditorApplication.ExecuteMenuItem("Edit/Play");
    }

}