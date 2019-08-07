using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotSpeak : RobotInteraction {

    public AudioSource audio;
    [SerializeField] AudioClip[] speechClips;
    bool active;
    private int _contentLength;
    private TextNarration _textNarration;
    private int _narrationIndex;
    private int _currentClip = 0;

	// Update is called once per frame
	void Update () {
		
        if (active)
        {
            if (ControlManager.Instance.GetButtonDown("Submit"))
            {
                _narrationIndex++;
                if (_narrationIndex + 1 > _contentLength )
                {
                    RobotDeactivate();
                    _currentClip = 0;
                }

                else
                {
                    //if you hit enter when it's not the end of the narration index, play the next clip
                    //loop around if we have more lines in narration than clips
                    _currentClip++;
                    if (_currentClip > speechClips.Length - 1)
                        _currentClip = 0;

                    audio.clip = speechClips[_currentClip];
                    audio.Play();
                }


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

        //on first select play first clip
        audio.clip = speechClips[0];
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
