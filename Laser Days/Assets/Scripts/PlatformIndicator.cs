using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformIndicator : MonoBehaviour {

    private Material RenderMat;

    void Awake()
    {
        RenderMat = GetComponent<Renderer>().material;
    }

	
    public void SetColors(Color a, Color b)
    {
        RenderMat.SetColor("_PassiveColor", a);
        RenderMat.SetColor("_ActiveColor", b);
    }

    public void On () 
    {
        RenderMat.SetInt("_isActive0", 1);
        RenderMat.SetInt("_isActive1", 1);
	}

    public void Off ()
    {
        RenderMat.SetInt("_isActive0", 0);
        RenderMat.SetInt("_isActive1", 0);
    }
}
