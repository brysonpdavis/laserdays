using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotSpeak : RobotInteraction {

    public AudioSource audio;
    bool active;
    private int _contentLength;
    private TextNarration _textNarration;
    private int _narrationIndex;

	// Update is called once per frame
	void Update () {
		
        if (active)
        {
            if (ControlManager.Instance.GetButtonDown("Submit"))
            {
                _narrationIndex++;
                if (_narrationIndex + 1 > _contentLength )
                    RobotDeactivate();
            }
        }
    }

    public override void OnSelect()
    {
        active = true;
        _narrationIndex = 0;

        base.OnSelect();

        MFPP.Player player = Toolbox.Instance.GetPlayer().GetComponent<MFPP.Player>();
        player.Movement.AllowMovement = false;
        player.Movement.AllowMouseMove = false;

        _textNarration = GetComponentInChildren<TextNarration>();
        _textNarration.Activate();
        _contentLength = _textNarration.GetContentLength();
        
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

    public override void DistantIconHover()
    {
        _iconContainer.SetCharacterInteract();
    }

    public override void CloseIconHover()
    {
        _iconContainer.SetCharacterInteract();
    }
}
