using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Security.Cryptography;
using Random = UnityEngine.Random;

public class SpawnableMutation : MonoBehaviour {

    public enum LifeCycle {Growth, Life, Death, Kill, Waiting}
    public LifeCycle lifePhase;

    public Vector2 minMaxWaitTime = new Vector2(0f, 1f);
    public Vector2 minMaxGrowLength = new Vector2(0f, 1f);
    public Vector2 minMaxLifeLength = new Vector2(0f, 1f);
    public Vector2 minMaxDeathLength = new Vector2(0f, 1f);
    public float startScale;
    public Vector2 minMaxScale = new Vector2(0f, 1f);

    public Color tintA;
    public Color tintB;

    private Renderer mRenderer;
    private MaterialPropertyBlock propBlock;
    private Collider coll;

    private Color tint;
    private float waitLength, growthLength, lifeLength, killLength, targetScale;
    private float growthProgess, lifeProgress, deathProgress, killProgress, waitingProgress;

    [HideInInspector] public float deathLength;

    // Use this for initialization
    void Start () {
        mRenderer = GetComponent<Renderer>();
        propBlock = new MaterialPropertyBlock();
        coll = GetComponent<SphereCollider>();
        coll.enabled = false;

        InitRandom();
        scale(0f);

        mRenderer.GetPropertyBlock(propBlock);
        propBlock.SetFloat("_BeginToBase", 0f);
        propBlock.SetFloat("_BaseToDeath", 0f);
        propBlock.SetColor("_TintColor", tint);
        mRenderer.SetPropertyBlock(propBlock);
	}
	
    void InitRandom ()
    {
        waitLength = Random.Range(minMaxWaitTime.x, minMaxWaitTime.y);
        growthLength = Random.Range(minMaxGrowLength.x, minMaxGrowLength.y);
        lifeLength = Random.Range(minMaxLifeLength.x, minMaxLifeLength.y);
        deathLength = Random.Range(minMaxDeathLength.x, minMaxDeathLength.y);

        targetScale = Random.Range(minMaxScale.x, minMaxScale.y);
        tint = Color.Lerp(tintA, tintB, Random.Range(0f, 1f));
    }

    void scale(float s)
    {
        transform.localScale = new Vector3(s, s, s);
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
                    scale(startScale);
                }

                break;
            
            case LifeCycle.Growth :
                if(growthProgess < growthLength)
                {
                    float ratio = growthProgess / growthLength;
                    scale(Mathf.Lerp(startScale, targetScale, TweeningFunctions.EaseOut(ratio)));

                    //using transition state b to change color over grown phase
                    //using one minus to go from satring in laser color to landing in real
                    mRenderer.GetPropertyBlock(propBlock);
                    propBlock.SetFloat("_BeginToBase", TweeningFunctions.EaseOut(ratio));
                    mRenderer.SetPropertyBlock(propBlock);

                    growthProgess += Time.deltaTime;

                } else 
                {
                    //move to next phase
                    lifePhase = LifeCycle.Life;
                    coll.enabled = true;
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
                    float scale = Mathf.Lerp(targetScale, 0f, TweeningFunctions.EaseOut(ratio));
              
                    transform.localScale = new Vector3(scale, scale, scale);

                    mRenderer.GetPropertyBlock(propBlock);
                    propBlock.SetFloat("_BaseToDeath", TweeningFunctions.EaseOut(ratio));
                    mRenderer.SetPropertyBlock(propBlock);


                    deathProgress += Time.deltaTime;      
                } else 
                {
                    Destroy(this.gameObject);
                }
                    
                break;
        } 
	}

    private void OnEnable()
    {
        GardenDrones.AddMutationToDrones(this);
    }

    private void OnDisable()
    {
        GardenDrones.RemoveMutationFromDrones(this);
    }
}
