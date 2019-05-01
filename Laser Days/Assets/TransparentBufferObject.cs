using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TransparentBufferObject : MonoBehaviour
{

    public Material m_Material;
    public Renderer m_Renderer;

    public void OnEnable()

    {
        m_Renderer = GetComponent<Renderer>();
        TransparentBufferGroup.instance.AddObject(this);
    }

    public void Start()
    {
        m_Renderer = GetComponent<Renderer>();
        TransparentBufferGroup.instance.AddObject(this);
    }

    public void OnDisable()
    {
        TransparentBufferGroup.instance.RemoveObject(this);
    }

    private void Update()
    {

    }

    private void DrawGizmo(bool selected)
    {
        var col = new Color(0.0f, 0.7f, 1f, 1.0f);
        col.a = selected ? 0.3f : 0.1f;
        Gizmos.color = col;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawCube(Vector3.zero, Vector3.one);
        col.a = selected ? 0.5f : 0.2f;
        Gizmos.color = col;
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
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

