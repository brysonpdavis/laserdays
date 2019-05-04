using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Adds this renderer to command buffer render group - script on camera 
// Needs the proper shader that corresponds to the shader on object - ie Goop -> Goop
// TODO Check for these pairings, and don't add object if not, throw error

[ExecuteInEditMode]
public class TransparentBufferObject : MonoBehaviour
{

    public Shader replaceShader;
    [HideInInspector] public Renderer m_Renderer;

    public void OnEnable()

    {
        if (replaceShader)
        {
            m_Renderer = GetComponent<Renderer>();
            TransparentBufferGroup.instance.AddObject(this);
        }
    }

    public void Start()
    {
        if (replaceShader)
        {
            m_Renderer = GetComponent<Renderer>();
            TransparentBufferGroup.instance.AddObject(this);
        }
    }

    public void OnDisable()
    {
        TransparentBufferGroup.instance.RemoveObject(this);
    }

}

