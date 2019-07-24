using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MFPP;

public class InSceneTeleporter : MonoBehaviour {

	public Vector3 sceneOffset;
	public bool reverse;

	public Light sun;

	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameObject playerObject = Toolbox.Instance.GetPlayer();
            Player playerScript = playerObject.GetComponent<Player>();

            playerScript.TeleportTo(playerObject.transform.position + sceneOffset, false);

        }
    }

}
