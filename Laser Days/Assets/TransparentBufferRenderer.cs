using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;
using System.Collections.Generic;

// Renders transparent gemoetry information into CameraGbuffer1
// So that there can be outlines on trasparent stuff 

public class TransparentBufferGroup
{
    [HideInInspector] public Dictionary<Shader, Shader> shaderPairs;

    private void Awake()
    {
        shaderPairs = buildShaderPairs();
    }

    static TransparentBufferGroup m_Instance;

    static public TransparentBufferGroup instance
    {
        get
        {
            if (m_Instance == null)
                m_Instance = new TransparentBufferGroup();
                m_Instance.shaderPairs = m_Instance.buildShaderPairs();
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

    public bool CheckForReplacment(Shader matShader, out Shader matchingShader)
    {
        bool contains = shaderPairs.ContainsKey(matShader);
        matchingShader = null;

        if(contains)
        {
            matchingShader = shaderPairs[matShader];
        }

        return contains;
    }


    private Dictionary<Shader, Shader> buildShaderPairs()
    {
        Dictionary<Shader, Shader> dict = new Dictionary<Shader, Shader>();
        dict.Add(Shader.Find("Crosshatch/Goop-Trigger"), Shader.Find("Crosshatch/OutlineBuffer/Goop-Trigger"));
        dict.Add(Shader.Find("Crosshatch/CompletionCrystal"), Shader.Find("Crosshatch/OutlineBuffer/Crystal"));
        dict.Add(Shader.Find("Custom/CrystalCore"), Shader.Find("Crosshatch/OutlineBuffer/Crystal"));
        dict.Add(Shader.Find("Crosshatch/Glass-Shared"), Shader.Find("Crosshatch/OutlineBuffer/Glass-Shared"));
        dict.Add(Shader.Find("Crosshatch/Glass-Single"), Shader.Find("Crosshatch/OutlineBuffer/Glass-Single"));
        dict.Add(Shader.Find("Crosshatch/Goop-PlantVariant"), Shader.Find("Crosshatch/OutlineBuffer/Goop-PlantVariant"));
        return dict;
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

        //Check if the buffer has already been added to the camera
        //if it has, clear it and move on
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
