using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkedPair : StationaryWall {

    public GameObject partner;
    Material partnerMaterial;
    MaterialPropertyBlock _propBlock;
    Renderer partnerRenderer;
    LinkedPair partnerLinkedPair;
    public bool isRunning;
    Renderer previewRenderer;
    Renderer partnerPreviewRenderer;

    int cancelTransition = 0;

    public override void Start()
    {
        base.Start();
        partnerRenderer = partner.GetComponent<Renderer>();
        partnerMaterial = partnerRenderer.material;
        _propBlock = new MaterialPropertyBlock();

        partnerLinkedPair = partner.GetComponent<LinkedPair>();
        previewRenderer = transform.Find("Activated Renderer").GetComponent<Renderer>();
        partnerPreviewRenderer = partner.transform.Find("Activated Renderer").GetComponent<Renderer>();
    }

    public override void OnSelect()
    {
        base.OnSelect();
        partnerPreviewRenderer.enabled = true;
        previewRenderer.enabled = true;
    }

    public override void OffSelect()
    {
        base.OffSelect();
        partnerPreviewRenderer.enabled = false;
        previewRenderer.enabled = false;
    }
    

    public void SwitchPartner(bool playerInLaser)
    {

        partnerPreviewRenderer.enabled = false;
        previewRenderer.enabled = false;


        partnerLinkedPair.timesFlipped += 1;

        if (playerInLaser) //player is going to laser with this object, so partner should be set to real
        {
            ShaderUtility.ShaderToReal(partnerMaterial);

            partner.GetComponent<Transition>().SetStart(1f);

            StartCoroutine(flipTransitionRoutine(1, 0f, 1f, false));

            partner.layer = 11;
        }

        else //player is going to real, so the pair should go to laser
        {
            ShaderUtility.ShaderToLaser(partnerMaterial);

            partner.GetComponent<Transition>().SetStart(0f);

            StartCoroutine(flipTransitionRoutine(0f, 1f, 1f, true));


            partner.layer = 10;

        }
    }

    
    private IEnumerator flipTransitionRoutine(float startpoint, float endpoint, float duration, bool goingToLaser)
    {
        partner.GetComponent<Transition>().enabled = false; 
        Debug.Log("duration " + duration);
        isRunning = true;
        //.isRunning = true;
        float elapsedTime = 0;
        float ratio = elapsedTime / duration;
        int count = 0;
        while (ratio < 1f)
        {
            ratio = elapsedTime / duration;
            float value = Mathf.Lerp(startpoint, endpoint, TweeningFunctions.BackAndForth(ratio));
            float transitionBValue;

            if (!goingToLaser)
            {
                transitionBValue = Mathf.Lerp(1f, 0f, ratio);
            }
            else transitionBValue = Mathf.Lerp(0f, 1f, ratio);

            partnerRenderer.GetPropertyBlock(_propBlock);
            _propBlock.SetFloat("_TransitionState", value);
            partnerRenderer.SetPropertyBlock(_propBlock);


            partnerMaterial.SetFloat("_TransitionStateB", transitionBValue);


            if (Toolbox.Instance.GetFlip().flippedThisFrame)
            {
                count += 1;
                if (count > 1)
                {
                        recentlySelected = false;
                        isRunning = false;
                        SetMaterialFloatProp("_onHold", 0f);
                        SetMaterialFloatProp("_Shimmer", 0f);
                    yield break;
                }
            }
                

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        //recentlySelected = false;

        isRunning = false;
       // partnerLinkedPair.isRunning = false;
        cancelTransition = 0;

        partner.GetComponent<Transition>().enabled = true;
    }


}
