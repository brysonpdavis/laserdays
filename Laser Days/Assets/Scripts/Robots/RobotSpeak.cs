﻿﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotSpeak : RobotInteraction, INarrationActor {

    public AudioSource audio;
    [SerializeField] AudioClip[] speechClips;
    private int _currentClip = 0;
    private MFPP.Player player;
    private float _interactCooldownTime = 4f;
    private float _currentInteractWaitTime = 0f;
    private bool _canInteract = true;

    [SerializeField] private bool flipOnResume = true;
    [SerializeField] private TextAsset text;
    [SerializeField] private RobotNarrationSettings narrationSettings;
    private List<IActivatable> _interfacesToActivate;
    [SerializeField] private GameObject[] objectsToActivate;
    private List<IDeactivatable> _interfacesToDeactivate;
    [SerializeField] private GameObject[] objectsToDeactivate;
    
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
        NarrationController.TriggerNarration(narrationSettings, this, text);
        
        base.OnSelect();
    }

    public override void OnSelect()
    {
        if (_canInteract)
            DoNarration();
    }

    public override void Start()
    {
        base.Start();
        player = Toolbox.Instance.GetPlayer().GetComponent<MFPP.Player>();

        _interfacesToActivate = new List<IActivatable>();

        _interfacesToDeactivate = new List<IDeactivatable>();

        foreach (GameObject obj in objectsToActivate)
        {
            foreach (var component in obj.GetComponents<IActivatable>())
                _interfacesToActivate.Add(component);
        }

        foreach (var obj in objectsToDeactivate)
        {
            foreach (var component in obj.GetComponents<IDeactivatable>())
                _interfacesToDeactivate.Add(component);
        }
    }


    public override void RobotDeactivate()
    {
        _canInteract = false;
        audio.Stop();
        GetComponentInChildren<GardenEye>().PlayerDeactivate();
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

    public void OnNarrationActivate()
    {
        Toolbox.Instance.DisablePlayerMovementAndFlip();
        _currentClip = 0;
        PlayRobotAudio();
        
        foreach (IDeactivatable thing in _interfacesToDeactivate)
        {
            thing.Deactivate();
        }
    }

    public void OnNarrationDeactivate()
    {
        Toolbox.Instance.EnablePlayerMovementAndFlip(flipOnResume);
        RobotDeactivate();
        _currentClip = 0;
        
        foreach (IActivatable thing in _interfacesToActivate)
        {
            thing.Activate();
        }
    }

    public void NextNarration()
    {
        _currentClip++;
        if (_currentClip > speechClips.Length - 1)
            _currentClip = 0;

        PlayRobotAudio();
    }

    private void PlayRobotAudio()
    {
        audio.clip = speechClips[_currentClip];
        audio.Play();
    }
}
