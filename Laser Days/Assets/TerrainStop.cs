using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainStop : MonoBehaviour {

    public float speedMult;
    MFPP.Player play;
    public bool isGood;
    private float lim;

    // Use this for initialization
    void Start () {
         play = GetComponent<MFPP.Player>();
         lim = play.SlopeLimit;
    }
	
	// Update is called once per frame
	void FixedUpdate () {

        RaycastHit hit;

        if(Physics.Raycast(transform.position + new Vector3(0f,0.2f,0f), Vector3.down, out hit, 10f, LayerMaskController.Everything))
        {
            if(hit.collider.CompareTag("HeavyTerrain"))
            {
                play.AddForce(play.DesiredWorldMovement * -1 * speedMult);
                isGood = true;
            } else
            {
                isGood = false;
            }
        }
		
	}




}
