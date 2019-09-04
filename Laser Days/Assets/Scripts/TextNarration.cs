﻿using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TextNarration : MonoBehaviour, IActivatable, IDeactivatable
{
    enum PopUpType { Static, Animated };

    [SerializeField]
    private PopUpType type = PopUpType.Static;

    GameObject container;
    private Text canvasText;
    public bool lore = false;
    public bool singleActivation;
    bool activated = false;
    private GameObject background;
    private Coroutine _coroutine;
    [SerializeField] private bool persistent = true;
    [SerializeField] private bool delayedHint;
    [SerializeField] private float hintDelay;
    [SerializeField] private NarrationSettings narrationSettings;
    [SerializeField] private TextAsset text;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Activate();
        }
    }

    public void Activate()
    {
        if (!singleActivation || (singleActivation && !activated))
        {
            activated = true;

            if (delayedHint)
            {
                StartCoroutine(HintCoroutine());
            }
            else
            {
                StartNarration();
            }
        }
    }

    private IEnumerator HintCoroutine()
    {
        yield return new WaitForSeconds(hintDelay);
        
        StartNarration();
    }

    private void StartNarration()
    {
        NarrationController.TriggerNarration(narrationSettings, null, text);
        
        if (lore)
            Toolbox.Instance.PlaySoundEffect(SoundBox.Instance.narrationSound);
        else
            Toolbox.Instance.PlaySoundEffect(SoundBox.Instance.narrationSound);

    }

    public void CancelHint()
    {
        if (_coroutine != null)
            StopCoroutine(_coroutine);
    }
    
    public void Deactivate()
    {
        NarrationController.CancelNarration();
    }

    private void OnDisable()
    {
        Deactivate();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!persistent && other.CompareTag("Player")) 
            Deactivate();
    }
}
