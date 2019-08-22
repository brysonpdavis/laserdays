using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MutoSpeak : SelectableObject, INarrationActor
{

    AudioSource audio;
    [SerializeField] AudioClip[] speechClips;
    private int _currentClip = 0;
    private MFPP.Player player;
    private float _interactCooldownTime = 4f;
    private float _currentInteractWaitTime = 0f;
    private bool _canInteract = true;
    public GameObject egg;
    bool speaking = false;

    [SerializeField] private bool flipOnResume = true;
    [SerializeField] private TextAsset text;
    [SerializeField] private RobotNarrationSettings narrationSettings;


    void Update()
    {
        if (!_canInteract)
            _currentInteractWaitTime += Time.deltaTime;

        if (_currentInteractWaitTime > _interactCooldownTime)
        {
            _canInteract = true;
            _currentInteractWaitTime = 0f;
        }

        if (speaking)
            GlowRoutine();
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
        audio = GetComponent<AudioSource>();
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
        speaking = true;
        PlayAudio();
    }

    public void OnNarrationDeactivate()
    {
        Toolbox.Instance.EnablePlayerMovementAndFlip(flipOnResume);
        _currentClip = 0;
        _canInteract = false;
        audio.Stop();
        speaking = false;
    }

    public void NextNarration()
    {
        _currentClip++;
        if (_currentClip > speechClips.Length - 1)
            _currentClip = 0;

        PlayAudio();
    }

    private void PlayAudio()
    {
        audio.clip = speechClips[_currentClip];
        audio.Play();
    }

    private void GlowRoutine()
    {
        
    }


}
