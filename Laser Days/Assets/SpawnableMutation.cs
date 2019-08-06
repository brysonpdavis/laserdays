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
    private float waitLength, lifeLength, killLength, targetScale;
    public float growthLength;
    private float growthProgess, lifeProgress, deathProgress, killProgress, waitingProgress;
    PlantMovement movement;

    [HideInInspector] public float deathLength;

    // Use this for initialization
    void Awake() {
        mRenderer = GetComponent<Renderer>();
        coll = GetComponent<SphereCollider>();
        movement = GetComponent<PlantMovement>();
        coll.enabled = true;
    }

    void PrepareAudioLife(AudioSource audio)
    {
        float pitchMultiplier = targetScale / (minMaxScale[1] - minMaxScale[0]);
        float actualPitch = (6f / pitchMultiplier) - 4f; 
        audio.pitch = actualPitch;
        Toolbox.Instance.SetVolume(audio);
        audio.Play();
    }

    void PrepareAudioDeath(AudioSource audio)
    {
        audio.clip = SoundBox.Instance.completionZone;
        Toolbox.Instance.SetVolume(audio);
        audio.pitch = 0;
        audio.Play();

    }
	
    void InitRandom ()
    {
        waitLength = Random.Range(minMaxWaitTime.x, minMaxWaitTime.y);
        growthLength = Random.Range(minMaxGrowLength.x, minMaxGrowLength.y);
        lifeLength = Random.Range(minMaxLifeLength.x, minMaxLifeLength.y);
        deathLength = Random.Range(minMaxDeathLength.x, minMaxDeathLength.y);

        targetScale = Random.Range(minMaxScale.x, minMaxScale.y);
        tint = Color.Lerp(tintA, tintB, Random.Range(0f, 1f));
        
        propBlock = new MaterialPropertyBlock();
        
        mRenderer.GetPropertyBlock(propBlock);
        propBlock.SetFloat("_BeginToBase", 0f);
        propBlock.SetFloat("_BaseToDeath", 0f);
        propBlock.SetColor("_TintColor", tint);
        mRenderer.SetPropertyBlock(propBlock);
        
        scale(0f);

        growthProgess = 0;
        lifeProgress = 0;
        deathProgress = 0;
        killProgress = 0; 
        waitingProgress = 0;
        
        lifePhase = LifeCycle.Waiting;
    }

    void scale(float s)
    {
        transform.localScale = new Vector3(s, s, s);
    }

    public void ColorShift(string property, float value)
    {
        mRenderer.GetPropertyBlock(propBlock);
        propBlock.SetFloat(property, value);
        mRenderer.SetPropertyBlock(propBlock);

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
                    PrepareAudioLife(GetComponent<AudioSource>());
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
                    movement.Activate();
                }
                break;

            case LifeCycle.Life :
/*
                if(lifeProgress < lifeLength)
                {
                    lifeProgress += Time.deltaTime;
                } else 
                {
                    //move to next phase
                    lifePhase = LifeCycle.Death;
                }
*/
                break;

            case LifeCycle.Death :
                PrepareAudioDeath(GetComponent<AudioSource>());
                movement.Deactivate();
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
                    this.gameObject.SetActive(false);
                }
                    
                break;
        } 
	}

    private void OnEnable()
    {
        GardenDrones.AddMutationToDrones(this);
        GardenEye.AddMutationToBots(this);
     
        coll.enabled = true;

        InitRandom();
    }

    private void OnDisable()
    {
        GardenDrones.RemoveMutationFromDrones(this);
        GardenEye.RemoveMutationFromBots(this);
    }
}
