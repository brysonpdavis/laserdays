using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorIndicator : MonoBehaviour
{

    private Material RenderMat;
    public float ScrollSpeed = 0.4f;

    void Awake()
    {
        RenderMat = GetComponent<Renderer>().material;
        RenderMat.SetInt("_Animated", 0);
    }


    private void Update()
    {
        var temp = RenderMat.GetFloat("_Elapsed");
        temp += (Time.deltaTime * ScrollSpeed);
        RenderMat.SetFloat("_Elapsed", temp);
    }


    public void SetColors(Color a, Color b, Color c, Texture2D d)
    {
        RenderMat.SetColor("_RestingColor", a);
        RenderMat.SetColor("_ActiveColor", b);
        RenderMat.SetColor("_ShimmerColor", c);
        RenderMat.SetTexture("_TriggerMap", d);
    }

    public void On()
    {
        RenderMat.SetFloat("_isActive", 1);
    }

    public void Off()
    {
        RenderMat.SetFloat("_isActive", 0);
    }
}
