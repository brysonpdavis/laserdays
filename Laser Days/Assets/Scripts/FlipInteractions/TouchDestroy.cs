﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchDestroy : FlipInteraction{

    public bool touched = false;
    bool activated = false;
    Material material;
    Renderer mRenderer;
    private MaterialPropertyBlock _propBlock;
    float duration;
    float elapsedTime;
    AudioSource audio;

    ParticleTransitionBurst[] particleBursts;


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Toolbox.Instance.SetVolume(audio);
            audio.Play();
            Toolbox.Instance.PlaySoundEffect(SoundBox.Instance.selection);
            if (!touched)
            { 
                material.SetFloat("_onHold", 1f);
            }

            touched = true;
        }
    }

    private void Awake()
    {
        _propBlock = new MaterialPropertyBlock();
        mRenderer = GetComponent<Renderer>();
        duration = Toolbox.Instance.globalFlipSpeed;
        material = mRenderer.material;
        audio = GetComponent<AudioSource>();

        mRenderer.GetPropertyBlock(_propBlock);

        particleBursts = GetComponentsInChildren<ParticleTransitionBurst>();

    }

    // Update is called once per frame
    void Update () {
        if (touched && !activated)
        {
            elapsedTime += Time.deltaTime;
            material.SetFloat("_TransitionStateB", ((Mathf.Sin(elapsedTime * 2f) + 1f) / 12f));
        }
            

	}

    public override void Interact()
    {
        if (touched && !activated)
        {
            activated = true;

            audio.clip = SoundBox.Instance.flipFail;
            audio.Play();
            audio.loop = false;

            GetComponent<MeshCollider>().isTrigger = true;
            //GetComponent<Collider>().enabled = false;


            StartCoroutine(InteractionRoutine());

            foreach (ParticleTransitionBurst burst in particleBursts)
            {
                burst.TransitionBurst();
            }

            //Toolbox.Instance.
            //Destroy(gameObject);
        }
    }


    private IEnumerator InteractionRoutine()
    {
        float elapsedTime = 0;
        float ratio = elapsedTime / duration;
        float start = material.GetFloat("_TransitionStateB");

        while (ratio < 1f)
        {
            ratio = elapsedTime / duration;
            float value = Mathf.Lerp(start, 1f, TweeningFunctions.EaseInOut(ratio));

            //mRenderer.GetPropertyBlock(_propBlock);

            //_propBlock.SetFloat("_TransitionStateB", value);
            //mRenderer.SetPropertyBlock(_propBlock);

            material.SetFloat("_TransitionStateB", value);

            elapsedTime += Time.fixedDeltaTime;
            yield return new  WaitForFixedUpdate();
        }

        mRenderer.enabled = false;

                 
    }


}
