using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;
using System.Collections.Generic;

// Renders transparent gemoetry information into CameraGbuffer1
// So that there can be outlines on trasparent stuff 

public class TransparentBufferGroup
{
    static TransparentBufferGroup m_Instance;
    static public TransparentBufferGroup instance
    {
        get
        {
            if (m_Instance == null)
                m_Instance = new TransparentBufferGroup();
            return m_Instance;
        }
    }

    internal HashSet<TransparentBufferObject> m_Objects = new HashSet<TransparentBufferObject>();

    public void AddObject(TransparentBufferObject o)
    {
        RemoveObject(o);
        m_Objects.Add(o);

    }
    public void RemoveObject(TransparentBufferObject o)
    {
        m_Objects.Remove(o);

    }
}

[RequireComponent(typeof(Camera))]
public class TransparentBufferRenderer : MonoBehaviour
{
    private Camera cam;
    CommandBuffer buff;
    private Material tempMat;

    private Dictionary<Camera, CommandBuffer> m_Cameras = new Dictionary<Camera, CommandBuffer>(); 

    private void Update()
    {
        cam = Camera.main;

        if(!cam){
            return;
        }

        buff = null;

        if (m_Cameras.ContainsKey(cam))
        {
            buff = m_Cameras[cam];
            buff.Clear();
        }
        else
        {
            buff = new CommandBuffer();
            buff.name = "Transparent Buffer Object";
            m_Cameras[cam] = buff;

            // Render right after opaque and alpha test 
            cam.AddCommandBuffer(CameraEvent.AfterGBuffer, buff);
        }
      
        var group = TransparentBufferGroup.instance;

        // Only seemed to work when writing to both Gbuffers, but only assigning value to GBuffer1
        RenderTargetIdentifier[] mrta = { BuiltinRenderTextureType.GBuffer0, BuiltinRenderTextureType.GBuffer1};

        buff.SetRenderTarget(mrta, BuiltinRenderTextureType.CameraTarget);
        foreach (var o in group.m_Objects)
        {
            if (o.replaceShader)
            {
                tempMat = new Material(o.m_Renderer.sharedMaterial);
                tempMat.shader = o.replaceShader;
                buff.DrawRenderer(o.m_Renderer, tempMat);
            }
        }

    }
}
