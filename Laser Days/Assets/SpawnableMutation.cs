using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnableMutation : MonoBehaviour {

    public enum LifeCycle {Growth, Life, Death, Kill, Waiting}
    public LifeCycle lifePhase;

    public float waitLength = 0.1f;
    public float growthLength = 1f;
    public float lifeLength = 5f;
    public float deathLength = 1f;
    public float killLength = 0.5f;

    public float startScale = 0f;
    public float endScale = 1f;

    private Renderer mRenderer;
    private MaterialPropertyBlock propBlock;

    private float growthProgess, lifeProgress, deathProgress, killProgress, waitingProgress;

	// Use this for initialization
	void Start () {
        mRenderer = GetComponent<Renderer>();
        propBlock = new MaterialPropertyBlock();
        mRenderer.GetPropertyBlock(propBlock);
        propBlock.SetFloat("_TransitionStateB", 1f);
        mRenderer.SetPropertyBlock(propBlock);

        lifePhase = LifeCycle.Growth;
        transform.localScale = new Vector3(startScale, startScale, startScale);
	}
	
	// Update is called once per frame
	void Update () {
    
        switch (lifePhase) 
        {
            case LifeCycle.Waiting :
                if(waitingProgress < waitLength)
                {
                    waitingProgress += Time.deltaTime;
                }
                else 
                {
                    lifePhase = LifeCycle.Growth;
                }

                break;
            
            case LifeCycle.Growth :
                if(growthProgess < growthLength)
                {
                    float ratio = growthProgess / growthLength;
                    float scale = Mathf.Lerp(startScale, endScale, TweeningFunctions.EaseOut(ratio));
                    transform.localScale = new Vector3(scale, scale, scale);

                    //using transition state b to change color over grown phase
                    //using one minus to go from satring in laser color to landing in real
                    mRenderer.GetPropertyBlock(propBlock);
                    propBlock.SetFloat("_TransitionStateB", 1f - TweeningFunctions.EaseOut(ratio));
                    mRenderer.SetPropertyBlock(propBlock);

                    growthProgess += Time.deltaTime;
                } else 
                {
                    //move to next phase
                    lifePhase = LifeCycle.Life;
                }
                break;

            case LifeCycle.Life :
                if(lifeProgress < lifeLength)
                {
                    lifeProgress += Time.deltaTime;
                } else 
                {
                    //move to next phase
                    lifePhase = LifeCycle.Death;
                }
                break;

            case LifeCycle.Death :
                if(deathProgress < deathLength)
                {
                    float ratio = deathProgress / deathLength;
                    float scale = Mathf.Lerp(endScale, 0f, TweeningFunctions.EaseOut(ratio));
              
                    transform.localScale = new Vector3(scale, scale, scale);

                    deathProgress += Time.deltaTime;      
                } else 
                {
                    Destroy(this.gameObject);
                }
                    
                break;
        } 
	}
}
