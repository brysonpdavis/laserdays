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
    private MFPP.Player player;
    private float _interactCooldownTime = 4f;
    private float _currentInteractWaitTime = 0f;
    private bool _canInteract = true;
    //private float _speechCooldown = 4f;
    //private float _currentSpeechCooldownTime = 0f;
    //private bool _canSpeak;



	// Update is called once per frame
	void Update () {
		if (!_canInteract)
            _currentInteractWaitTime += Time.deltaTime;
        
        if (_currentInteractWaitTime > _interactCooldownTime)
        {
            _canInteract = true;
            _currentInteractWaitTime = 0f;
        }

        //if (!_canSpeak)
        //    _currentSpeechCooldownTime += Time.deltaTime;
        
        //if (_currentSpeechCooldownTime > _speechCooldown)
        //{
        //    _canSpeak = true;
        //    _currentSpeechCooldownTime = 0f;
        //}
            
            


    }

    void DoNarration()
    {
        //_canSpeak = false;
        //_currentSpeechCooldownTime = 0f;

        if (_narrationIndex == 0)
        {

            base.OnSelect();
            player.Movement.AllowMovement = false;
            player.Movement.AllowMouseMove = false;

            _textNarration.Activate();
            _contentLength = _textNarration.GetContentLength();

            //on first select play first clip
            audio.clip = speechClips[0];
            audio.Play();
            GetComponentInChildren<GardenEye>().PlayerInteraction();
            Toolbox.Instance.GetFlip().canFlip = false;
        }

        _narrationIndex++;
        
        if (_narrationIndex > _contentLength)
        {
            _narrationIndex = 0;
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

    public override void OnSelect()
    {
        active = true;

        if (_canInteract)
            DoNarration();
    }

    public override void Start()
    {
        base.Start();
        _narrationIndex = 0;
        player = Toolbox.Instance.GetPlayer().GetComponent<MFPP.Player>();
        _textNarration = GetComponentInChildren<TextNarration>();

    }


    public override void RobotDeactivate()
    {
        _canInteract = false;
        _textNarration.Deactivate();
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
        if (_canInteract)
            _iconContainer.SetCharacterInteract();
        else
            _iconContainer.SetDefault();
    }

    public override void CloseIconHover()
    {
        if (_canInteract)
            _iconContainer.SetCharacterInteract();
        else
            _iconContainer.SetDefault();
    }
}
