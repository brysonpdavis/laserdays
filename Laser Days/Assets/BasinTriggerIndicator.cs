using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasinTriggerIndicator : MonoBehaviour
{
    private Material RenderMat;

    private void Awake()
    {
        RenderMat = GetComponent<Renderer>().material;

        UnCollide();
        Deactivate();

    }

    public void Collide()
    {
        RenderMat.SetInt("_isCollide", 1);
    }

    public void UnCollide()
    {
        RenderMat.SetInt("_isCollide", 0);
    }

    public void Activate()
    {
        RenderMat.SetInt("_isActive0", 1);
        RenderMat.SetInt("_isActive1", 1);
    }

    public void Deactivate()
    {
        RenderMat.SetInt("_isActive0", 0);
        RenderMat.SetInt("_isActive1", 0);
    }


}
