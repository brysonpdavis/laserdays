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
                renderChange.material.SetInt("_onHover", 1);
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
                renderChange.material.SetInt("_onHover", 0);
            }
        }
    }

    public void OnHold()
    {

        Renderers = GetComponentsInChildren<Renderer>();
        if (Renderers.Length > 0)
        {
            foreach (Renderer renderChange in Renderers)
            {
                renderChange.material.SetInt("_onHold", 1);
            }
        }
    }



    public void OnDrop()
    {

        Renderers = GetComponentsInChildren<Renderer>();

        if (Renderers.Length > 0)
        {
            foreach (Renderer renderChange in Renderers)
            {
                renderChange.material.SetInt("_onHold", 0);
            }
        }
    }

}
