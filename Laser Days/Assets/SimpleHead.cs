using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleHead : MonoBehaviour {

	private Transform player;   

	void Start () {

		player = Toolbox.Instance.GetPlayer().transform;
  
	}
	
	// Update is called once per frame
	void Update () {

        transform.LookAt(player.position);
        transform.eulerAngles = new Vector3(0f, transform.eulerAngles.y, 0f);
 
    }
}
