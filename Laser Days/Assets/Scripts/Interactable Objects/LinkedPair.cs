﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkedPair : StationaryWall {

    public GameObject partner;
    Material partnerMaterial;
    MaterialPropertyBlock _propBlock;
    Renderer partnerRenderer;
    LinkedPair partnerLinkedPair;
    bool isRunning;

    int cancelTransition = 0;


    public override void Start()
    {
        base.Start();
        partnerRenderer = partner.GetComponent<Renderer>();
        partnerMaterial = partnerRenderer.material;
        _propBlock = new MaterialPropertyBlock();
        partnerLinkedPair = partner.GetComponent<LinkedPair>();
    }

    public void SwitchPartner(bool playerInLaser)
    {

        partnerLinkedPair.timesFlipped += 1;

        if (playerInLaser) //player is going to laser with this object, so partner should be set to real
        {
            ShaderUtility.ShaderToReal(partnerMaterial);

            partner.GetComponent<Transition>().SetStart(1f);
            //StopAllCoroutines();

            StartCoroutine(flipTransitionRoutine(1, 0f, 1f));


            partner.layer = 11;
        }

        else //player is going to real, so the pair should go to laser
        {
            ShaderUtility.ShaderToLaser(partnerMaterial);

            partner.GetComponent<Transition>().SetStart(0f);
            //StopAllCoroutines();



            partner.layer = 10;

        }
    }


    public void CancelTransition()
    {

        if (isRunning)
            cancelTransition += 1;

        if (cancelTransition>1)
        {
            StopAllCoroutines();
            //material.SetFloat("_onHold", 0f);
            //material.SetFloat("_Shimmer", 0f);
        }

    }

    public override void SetType()
    {
        objectType = ObjectType.LinkedPair;
    }

    private IEnumerator flipTransitionRoutine(float startpoint, float endpoint, float duration)
    {
        isRunning = true;
        float elapsedTime = 0;
        float ratio = elapsedTime / duration;
        //int property = Shader.PropertyToID("_D7A8CF01");

        while (ratio < 1f)
        {
            ratio = elapsedTime / duration;
            float value = Mathf.Lerp(startpoint, endpoint, TweeningFunctions.BackAndForth(ratio));


            partnerRenderer.GetPropertyBlock(_propBlock);

            _propBlock.SetFloat("_TransitionState", value);
            partnerRenderer.SetPropertyBlock(_propBlock);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        isRunning = false;
        cancelTransition = 0;
    }


}
