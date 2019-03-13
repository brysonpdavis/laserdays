using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Utility;

public class Booster : MonoBehaviour {

    public float boostAmount = 20;
    public float playerBoostAmount = 10;
    public bool affectsPlayer = false;
    public bool affectsObjects = true;
    GameObject player;
    private MFPP.Modules.PickUpModule pickUp;
    private SoundBox box;


    public int count = 1;
    public GameObject ring;
    private GameObject currentRing;





    private void Start()
    {
        player = Toolbox.Instance.GetPlayer();
        box = SoundBox.Instance;
        pickUp = player.GetComponent<MFPP.Modules.PickUpModule>();
    }


    private void OnTriggerEnter(Collider other)
    {
        
        if (pickUp.heldObject && pickUp.heldObject.Equals(other.gameObject))
            pickUp.PutDown();


        if (affectsPlayer && other.GetComponent<MFPP.Player>() && !Toolbox.Instance.EqualToHeld(transform.parent.gameObject))
        {
            other.GetComponent<MFPP.Player>().FinalMovement = new Vector3(0f, 0f, 0f);
            StartCoroutine(OnBoost(other.gameObject));
            Burst();

        }

        else if (affectsObjects && other.GetComponent<Rigidbody>()){
            //Debug.Log("hitting obj" + other.GetComponent<Rigidbody>().velocity);
            Vector3 startVelocity = other.GetComponent<Rigidbody>().velocity;
            startVelocity.y = 0f;
            other.GetComponent<Rigidbody>().velocity = startVelocity;
            other.GetComponent<Rigidbody>().AddForce(transform.up * boostAmount, ForceMode.Impulse);
            Burst();

        }

        if (GetComponent<Renderer>()){
            //TODO: change this to some other variable
            //GetComponent<Renderer>().material.SetInt("_onHold", 1);
        }

        if (other.gameObject.GetComponent<AudioSource>() && (this.gameObject.layer + 5) == player.layer)
        {
            AudioSource source = other.gameObject.GetComponent<AudioSource>();
            Toolbox.Instance.SetVolume(source);
            source.clip = box.PlayBoost();
            source.Play();
        }


}
    private void Burst()
    {
        if (gameObject.layer + 5 == Toolbox.Instance.GetPlayer().layer)
        {


            for (int i = 0; i < count; i++)
            {
                currentRing = Instantiate(ring);
                currentRing.transform.position = transform.parent.transform.position;
                currentRing.GetComponent<BoostPulse>().Pulse(i * 0.1f);
            }
        }


    }
    private void OnTriggerExit(Collider other)
    {
        StartCoroutine(RenderActivationOff());
    }



    private IEnumerator RenderActivationOff()
    {
        if (GetComponent<Renderer>())
        {
            yield return new WaitForSeconds(.25f);
            //TODO: change this to some other variable
            //GetComponent<Renderer>().material.SetInt("_onHold", 0);
        }

        yield return null;
    }

    private IEnumerator OnBoost(GameObject obj)
    {
        player.GetComponent<MFPP.Player>().IsBouncing = true;
        float finalBoost = playerBoostAmount;

        if (player.GetComponent<MFPP.Player>().IsJumping)
        {
            //makes sure it doesn't magnify jump and boost if the player does both at the same time
            //Debug.Log("yo");
            float jumpAmt = player.GetComponent<MFPP.Player>().Movement.Jump.Power;
            finalBoost = playerBoostAmount -jumpAmt;
        }

        yield return new WaitForEndOfFrame();

        Vector3 boost = transform.up;
        boost *= finalBoost;
        obj.GetComponent<MFPP.Player>().AddImpulse(boost);
        //Debug.Log("boosting");

        //Burst();


        finalBoost = playerBoostAmount; //resets the player boost for next run
        yield return null;

        player.GetComponent<MFPP.Player>().IsBouncing = false;

    }



}
