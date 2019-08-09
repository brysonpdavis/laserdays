using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultipleTransition : Transition
{
    private List<Material> _materials;

    protected override void Awake()
    {
        _materials = new List<Material>();

        base.Awake();
    }

    protected override void Start()
    {
        if (sharedMaterial)
        {
            foreach (var mat in _materials)
            {
                if (!Toolbox.Instance.sharedMaterials.Contains(mat))
                    Toolbox.Instance.sharedMaterials.Add(mat);
            }
        }
    }

    protected override void CheckForInstance()
    {
        // If the gameobject is on Real, Laser, or TransitionOnly
        // Check if it is interactable to see if the material should be its own instance
        // In any other case, the material should be shared

        if (gameObject.layer == 10 || gameObject.layer == 11 || gameObject.layer == 27)
        {
            if (GetComponent<ReticleObject>())
            {
                AddInstanceMaterials();
            }
            else
            {
                AddSharedMaterials();
            }
        } 
        // If it is marked shared, then make it shared 
        // For now, do the same if its not marked shared, but not on shared layers
        else if (shared)
        {
            //material = mRenderer.sharedMaterial;
            sharedMaterial = true;
            shared = true;
        } else 
        {
            //material = mRenderer.sharedMaterial;
            sharedMaterial = true;
            shared = true;
        }
    }
    
    private void AddSharedMaterials()
    {
        foreach (Material mat in mRenderer.sharedMaterials)
        {
            _materials.Add(mat);
        }
    }

    private void AddInstanceMaterials()
    {
        foreach (Material mat in mRenderer.materials)
        {
            _materials.Add(mat);
        }
    }
    
    public override void MaterialSetStart (float value)
    {
        foreach (var mat in _materials)
        {
            mat.SetFloat("_TransitionState", value);
        }
    }
}
