using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Booster : MonoBehaviour {

    public int boostAmount = 20;
    public int playerBoostAmount = 10;
    public bool affectsPlayer = false;
    public bool affectsObjects = true;
    GameObject player;
    private MFPP.Modules.PickUpModule pickUp;
    private SoundBox box;


    private void Start()
    {
        player = Toolbox.Instance.GetPlayer();
        box = player.GetComponent<SoundBox>();
        pickUp = player.GetComponent<MFPP.Modules.PickUpModule>();
    }


    private void OnTriggerEnter(Collider other)
    {
        
        if (pickUp.heldObject && pickUp.heldObject.Equals(other.gameObject))
        {
                pickUp.PutDown();
        }


        if (affectsPlayer && other.GetComponent<MFPP.Player>())
        {
            //Debug.Log("hitting player");
            //Debug.Log(other.GetComponent<MFPP.Player>().FinalMovement);
            other.GetComponent<MFPP.Player>().FinalMovement = new Vector3(0f, 0f, 0f);

            StartCoroutine(OnBoost(other.gameObject));

        }

        else if (affectsObjects && other.GetComponent<Rigidbody>()){
            other.GetComponent<Rigidbody>().AddForce(transform.up * boostAmount, ForceMode.Impulse);

        }

        if (GetComponent<Renderer>()){
            GetComponent<Renderer>().material.SetInt("_onHold", 1);
        }

        if (other.gameObject.GetComponent<AudioSource>() && (this.gameObject.layer + 5) == player.layer)
        {
            AudioSource source = other.gameObject.GetComponent<AudioSource>();
            source.clip = box.PlayBoost();
            source.Play();
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
            GetComponent<Renderer>().material.SetInt("_onHold", 0);
        }

        yield return null;
    }

    private IEnumerator OnBoost(GameObject obj)
    {
        
        yield return new WaitForEndOfFrame();
        //Debug.Log(transform.up * playerBoostAmount);
        //Debug.Log(transform.up);
        Vector3 boost = transform.up;
        boost *= playerBoostAmount;
        obj.GetComponent<MFPP.Player>().AddImpulse(boost);


        yield return null;
    }



}
