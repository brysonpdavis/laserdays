using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;
using System.Collections.Generic;


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

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class TransparentBufferRenderer : MonoBehaviour
{
  

    private Camera cam;
    CommandBuffer buf;

    private Dictionary<Camera, CommandBuffer> m_Cameras = new Dictionary<Camera, CommandBuffer>(); 

    private void Start()
    {
        //cam = Camera.main;


    }

    private void Update()
    {


        cam = Camera.main;

        if(!cam){
            return;
        }

        CommandBuffer buff = null;

        if (m_Cameras.ContainsKey(cam))
        {
            buff = m_Cameras[cam];
            buff.Clear();
        }
        else
        {
            buff = new CommandBuffer();
            buff.name = "Transparent Geo Outlines";
            m_Cameras[cam] = buff;
            cam.AddCommandBuffer(CameraEvent.AfterGBuffer, buff);
        }
      
        var group = TransparentBufferGroup.instance;

        RenderTargetIdentifier[] mrta = { BuiltinRenderTextureType.GBuffer0, BuiltinRenderTextureType.GBuffer1};


        buff.SetRenderTarget(mrta, BuiltinRenderTextureType.CameraTarget);

        foreach (var o in group.m_Objects)
        {
            buff.DrawRenderer(o.m_Renderer, o.m_Material);
        }

    }
}
    /**
    public void OnWillRenderObject()
    {
        var act = gameObject.activeInHierarchy && enabled;
        if (!act)
        {
            OnDisable();
            return;
        }

        var cam = Camera.current;
        if (!cam)
            return;

        CommandBuffer buf = null;
        if (m_Cameras.ContainsKey(cam))
        {
            buf = m_Cameras[cam];
            buf.Clear();
        }
        else
        {
            buf = new CommandBuffer();
            buf.name = "Deferred decals";
            m_Cameras[cam] = buf;

            // set this command buffer to be executed just before deferred lighting pass
            // in the camera
            cam.AddCommandBuffer(CameraEvent.BeforeLighting, buf);
        }

        //@TODO: in a real system should cull decals, and possibly only
        // recreate the command buffer when something has changed.

        var system = DeferredDecalSystem.instance;

        // copy g-buffer normals into a temporary RT
        var normalsID = Shader.PropertyToID("_NormalsCopy");
        buf.GetTemporaryRT(normalsID, -1, -1);
        buf.Blit(BuiltinRenderTextureType.GBuffer2, normalsID);
        // render diffuse-only decals into diffuse channel
        RenderTargetIdentifier[] mrta = { BuiltinRenderTextureType.GBuffer0, BuiltinRenderTextureType.GBuffer1 };
        buf.SetRenderTarget(mrta, BuiltinRenderTextureType.CameraTarget);

        foreach (var decal in system.m_DecalsDiffuse)
        {
            buf.DrawMesh(m_CubeMesh, decal.transform.localToWorldMatrix, decal.m_Material);
        }

        // release temporary normals RT
        buf.ReleaseTemporaryRT(normalsID); 
    }
}
**/