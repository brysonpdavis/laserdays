using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Core : MonoBehaviour {

    Material renderMat; 

	// Use this for initialization
	void Awake () {
        renderMat = GetComponent<Renderer>().material;
	}
    private void Start()
    {
        renderMat.SetColor("_ShimmerColor", GetComponentInParent<FlippableObject>().shimmerColor);//new Color(.4f, .4f, .4f))//GetComponentInParent<Renderer>().material.GetColor("_ShimmerColor"));
    }
    public void Flip(bool dir)
    {
        //quick setup for if we're changing the core shader before the core itself has done the awake
        if (!renderMat)
            renderMat = GetComponent<Renderer>().material;

        if (dir)
        {
            renderMat.shader = Toolbox.Instance.GetPlayer().GetComponent<RaycastManager>().coreLaser;
            GetComponent<Transition>().SetStart(1f);
        }
        else
        {
            renderMat.shader = Toolbox.Instance.GetPlayer().GetComponent<RaycastManager>().coreReal;
            GetComponent<Transition>().SetStart(0f);
        }
    }
    public void SetColor(Color color)
    {
        renderMat.SetColor("_ShimmerColor", color);
    }

}
