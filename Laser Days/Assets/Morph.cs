using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Morph : MonoBehaviour {

    public bool parent;
    public bool turnOff = false;
    public float morphDuration = 0.4f;
    RaycastManager rm;
    public GameObject associatedMorph;
    public Transition[] internalNonMorphs;


	void Start () {
        rm = Toolbox.Instance.GetPlayer().GetComponent<RaycastManager>();
	}
	
    public void OnPickup (){

        //get ready for potential flip, with both objects
        //change other object's shader as well

        associatedMorph.SetActive(true);

        rm.selectedObjs.Add(associatedMorph);
        rm.AddToList(associatedMorph);


    }

    public void OnPutDown () {
        rm.selectedObjs.Remove(associatedMorph);
        rm.RemoveFromList(associatedMorph, false);
        associatedMorph.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        //change shader back to whatever morph shader it's suppoed to be in based on player's layer

        //COULD DO TRANSITION HERE! [OTHER PART FOR SELECTED OBJ FLIPPED IS IN TRANSITION]

            associatedMorph.SetActive(false);
        
        if (rm.gameObject.layer == 15){
            //change shader and value to laser on drop
            associatedMorph.GetComponent<Renderer>().material.shader = rm.morphLaserWorldShader;
            associatedMorph.GetComponent<Transition>().SetStart(1f);

        }

        else {
            //change object's shader and value to real on drop
            associatedMorph.GetComponent<Renderer>().material.shader = rm.morphRealWorldShader;
            associatedMorph.GetComponent<Transition>().SetStart(0f);

        }


    }

   public void OnFlip (int direction, bool held) {
        // call visual transition transition controller given direction. ONLY on self! (this method will also get called on counterpart!)
        //float of 1 means we're going to laser, float 0 means we're going to real

            Transition transition = GetComponent<Transition>();

            // VISUAL TRANSITION
            if (direction == 1)
            {
                this.GetComponent<Renderer>().material.shader = rm.morphLaserWorldShader;
                transition.SetStart(1f);
                if (this.gameObject.CompareTag("MorphOn"))
                {
                    transition.Morph(0, morphDuration);
                }

                else
                {
                    transition.Morph(1, morphDuration);
                }
            }

            else
            {
                this.GetComponent<Renderer>().material.shader = rm.morphRealWorldShader;
                transition.SetStart(0f);

                if (this.gameObject.CompareTag("MorphOn"))
                {
                    transition.Morph(0, morphDuration);
                }

                else
                {
                    transition.Morph(1, morphDuration);
                }

            }



            if (this.gameObject.CompareTag("MorphOn"))
            {
                //transition properly
                if (!held)
                {
                    //transition with turning game object off at the end
                    turnOff = true;
                }


                //tramsition normally

                GetComponent<Rigidbody>().useGravity = false;
                GetComponent<Rigidbody>().isKinematic = true;
                GetComponent<BoxCollider>().isTrigger = true;
                this.gameObject.tag = "MorphOff";

            }

            else
            {
                //transition normally
                // transition.Morph(direction, morphDuration);

                //turn things on!
                GetComponent<Rigidbody>().useGravity = true;
                GetComponent<Rigidbody>().isKinematic = false;
                GetComponent<BoxCollider>().isTrigger = false;
                this.gameObject.tag = "MorphOn";
            }

        //transition everybody that doesn't morph that's contained within the transition
        TransitionInternals(direction);

    }

    public void OnSelection () {
        //change shader
        rm.AddToList(associatedMorph);
    }


    private void TransitionInternals(int direction)
    {
        if (internalNonMorphs.Length > 0)
        {
            foreach (Transition albo in internalNonMorphs)
            {
                if (direction == 1)
                {
                    albo.Flip(1f, morphDuration);
                }

                else
                {
                    albo.Flip(0f, morphDuration);
                }
            }

        }
    }
    }
