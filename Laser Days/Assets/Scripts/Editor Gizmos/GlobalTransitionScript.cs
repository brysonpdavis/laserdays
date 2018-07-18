using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalTransitionScript : MonoBehaviour {
    private Component[] components;

	// Use this for initialization
	void Start () {


	}
	
	// Update is called once per frame
	void Update () {
		
	}


    public void GlobalReal()
    {
        components = GetComponentsInChildren<Transition>();
            foreach (Transition albo in components)
            {
            Material material = albo.GetComponent<Renderer>().material;
            material.SetFloat("_TransitionState", 0);
        
            }   
     
    }


    public void GlobalLaser()
    {
        components = GetComponentsInChildren<Transition>();
        foreach (Transition albo in components)
        {
            Material material = albo.GetComponent<Renderer>().material;
            material.SetFloat("_TransitionState", 1);
        }

    }

    public void GlobalAll(){

        components = GetComponentsInChildren<Transition>();
        foreach (Transition albo in components)
        {
            Material material = albo.GetComponent<Renderer>().material;

            //set all real world objects to visible
            if (albo.gameObject.layer == 11) {
                material.SetFloat("_TransitionState", 0);
            }

            //set all laser world objects to visible
            else if (albo.gameObject.layer == 10)
            {
                material.SetFloat("_TransitionState", 1);
            }

            //set all transition objects to halfway
            else if (albo.gameObject.layer == 17)
            {
                material.SetFloat("_TransitionState", .5f);
            }

        }


    }


}
