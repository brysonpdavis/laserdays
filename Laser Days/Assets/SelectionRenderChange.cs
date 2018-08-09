using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[DisallowMultipleComponent]

public class SelectionRenderChange : MonoBehaviour {

    public Renderer[] Renderers;

    public void SwitchRenderersOn()
    {

        Renderers = GetComponentsInChildren<Renderer>();
        if (Renderers.Length > 0)
        {
            foreach (Renderer renderChange in Renderers)
            {
                renderChange.material.SetInt("_IsSelected", 1);
            }
        }
    }



    public void SwitchRenderersOff()
    {

        Renderers = GetComponentsInChildren<Renderer>();

        if (Renderers.Length>0)
        {
            foreach (Renderer renderChange in Renderers)
            {
                renderChange.material.SetInt("_IsSelected", 0);
            }
        }
    }


}
