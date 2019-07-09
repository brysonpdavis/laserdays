using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Adds this renderer to command buffer render group - script on camera 
[ExecuteInEditMode]
public class TransparentBufferObject : MonoBehaviour
{
    [HideInInspector] public Shader replaceShader;
    [HideInInspector] public Renderer m_Renderer;

    private void tryAdd()
    {
        if (GetComponent<Renderer>())
        {
            m_Renderer = GetComponent<Renderer>();

            Shader materialShader = m_Renderer.sharedMaterial.shader;
            Shader temp = null;

            //Check if this object's shader has a match in the list of replacable shaders      
            if (TransparentBufferGroup.Instance.CheckForReplacment(materialShader, out temp))
            {
                //Add this object to the list of objects to be rendered in the tranparent outline buffer
                replaceShader = temp;
                TransparentBufferGroup.Instance.AddObject(this);
            }  
            else 
            {
                Debug.Log("No matching shader found. Object not added to buffer.");
                Debug.Log(materialShader.name);
            }        
        } 
        else if (GetComponent<ParticleSystemRenderer>())
        {
            m_Renderer = GetComponent<ParticleSystemRenderer>();

            Shader materialShader = m_Renderer.sharedMaterial.shader;
            Shader temp = null;

            //Check if this object's shader has a match in the list of replacable shaders      
            if (TransparentBufferGroup.Instance.CheckForReplacment(materialShader, out temp))
            {
                //Add this object to the list of objects to be rendered in the tranparent outline buffer
                replaceShader = temp;
                TransparentBufferGroup.Instance.AddObject(this);
            }
            else
            {
                Debug.Log("No matching shader found. Object not added to buffer.");
                Debug.Log(materialShader.name);
            }
        }
        else
        {
            Debug.Log("No renderer or particle renderer found. Object not added to buffer.");
        }
    }

    public void OnEnable()
    {
        tryAdd();
    }

    public void Start()
    {
        tryAdd();
    }

    public void OnDisable()
    {
        TransparentBufferGroup.Instance.RemoveObject(this);
    }

}

