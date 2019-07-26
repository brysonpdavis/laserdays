using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotSpeak : RobotInteraction {

    public AudioSource audio;
    bool active;
    // Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
		
        if (active)
        {
            if (ControlManager.Instance.GetButtonDown("Submit"))
            {
                RobotDeactivate(); 
            }

            }

	}

    public override void RobotActivate()
    {
        active = true;

        base.RobotActivate();

        MFPP.Player player = Toolbox.Instance.GetPlayer().GetComponent<MFPP.Player>();
        player.Movement.AllowMovement = false;
        player.Movement.AllowMouseMove = false;

        GetComponentInChildren<TextNarration>().Activate();
        audio.Play();
        GetComponentInChildren<GardenEye>().PlayerInteraction();
        Toolbox.Instance.GetFlip().canFlip = false;
    }

    public override void RobotDeactivate()
    {

        MFPP.Player player = Toolbox.Instance.GetPlayer().GetComponent<MFPP.Player>();
        player.Movement.AllowMovement = true;
        player.Movement.AllowMouseMove = true;
        audio.Stop();

        GetComponentInChildren<GardenEye>().PlayerDeactivate();


        active = false;
        Toolbox.Instance.GetFlip().canFlip = true;
    }
}
