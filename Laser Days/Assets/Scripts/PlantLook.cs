using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantLook : ReticleObject {

    public float colorChangeStep = 0.001f;
    bool colorChanging;
    
    public override void Start()
    {
        base.Start();
    }

    public override void OnHover()
    {
        colorChanging = true;
    }

    public override void OffHover()
    {
        colorChanging = false;

    }

    private void Update()
    {
        if(colorChanging)
        {
            var btd = Mathf.Clamp01(_material.GetFloat("_BaseToDeath") + colorChangeStep);
            SetMaterialFloatProp("_BaseToDeath", btd);
           
        } else
        {
            var btd = Mathf.Clamp01(_material.GetFloat("_BaseToDeath") - colorChangeStep);
            SetMaterialFloatProp("_BaseToDeath", btd);
        }
            

    }
}
