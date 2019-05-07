using UnityEngine;

[ExecuteInEditMode]
public class DecalGizmo : MonoBehaviour
{
    private void DrawGizmo(bool selected)
    {
        var col =  new Color(1.0f, 0.4f, 0.1f, 1.0f);
        col.a = selected ? 0.3f : 0.7f;
        Gizmos.color = col;
        Gizmos.matrix = transform.localToWorldMatrix;
        Vector3 newSize = new Vector3(1.0f, 1.0f, 0.5f);
        Vector3 newCenter = new Vector3(0.0f, 0.0f, -0.25f);
        Gizmos.DrawCube(newCenter, newSize);
        col.a = selected ? 1.0f : 0.5f;
        Gizmos.color = col;
        Gizmos.DrawWireCube(newCenter, newSize);
    }

    public void OnDrawGizmos()
    {
        DrawGizmo(false);
    }
    public void OnDrawGizmosSelected()
    {
        DrawGizmo(true);
    }
}
