using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Core : MonoBehaviour {

    Material renderMat; 

	// Use this for initialization
	void Awake () {
        renderMat = GetComponent<Renderer>().material;
	}

    public void Flip(bool dir)
    {
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

}
