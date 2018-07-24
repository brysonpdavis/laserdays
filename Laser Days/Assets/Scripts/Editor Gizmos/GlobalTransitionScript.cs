using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalTransitionScript : MonoBehaviour {
    public Component[] components;


    public void GlobalReal()
    {
        components = GetComponentsInChildren<Transition>();
            foreach (Transition albo in components)
            {
            if (albo.GetComponent<Renderer>()){
                Material material = albo.GetComponent<Renderer>().sharedMaterial;
                material.SetFloat("_TransitionState", 0);
            }
            }   
     
    }


    public void GlobalLaser()
    {
        components = GetComponentsInChildren<Transition>();
        foreach (Transition albo in components)
        {
            if (albo.GetComponent<Renderer>())
            {
                
                Material material = albo.GetComponent<Renderer>().sharedMaterial;
                material.SetFloat("_TransitionState", 1);
            }
        }
    }

    public void GlobalAll()
    {

        components = GetComponentsInChildren<Transition>();
        foreach (Transition albo in components)
        {

            if (albo.GetComponent<Renderer>())
            {
                Material material = albo.GetComponent<Renderer>().sharedMaterial;
                RendererExtensions.UpdateGIMaterials(albo.GetComponent<Renderer>());

                //set all real world objects to visible
                if (albo.gameObject.layer == 11)
                {
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

                //SETTING LADDERS


                //realworld ladders
                if (albo.gameObject.layer == 18)
                {
                    material.SetFloat("_TransitionState", 0);
                }

                //laserworld ladders
                if (albo.gameObject.layer == 19)
                {
                    material.SetFloat("_TransitionState", 1);
                }


            }


        }

    }
}
