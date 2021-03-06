﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeThatSees : MonoBehaviour {

    public bool isActive = false;
    [SerializeField] private Transform player;
    //LineRenderer lineRenderer;
    flipScript flip;
    public bool onWall;
    public bool blockingFlip;
    private float startingAngleY;
    private enum EyeDirection{Forward, Backward, Right, Left};
    private EyeDirection myDirection;
    private ParticleSystem particleSystem;
    [HideInInspector] public Vector3 currentPlayerPoint;
    AudioSource audio;

    public int particleCount = 15;
    public float radialSpeed = 0.0f;
    public float lifeMultiplier = 1.0f;

    [HideInInspector]
    public SimpleEye eyeParent;

    private LayerMask currentLayerMask;

    private Transition transition;

    void Start () {
        player = Toolbox.Instance.GetPlayer().transform;
        flip = player.gameObject.GetComponent<flipScript>();
        flipScript.OnFailedFlip += FailedFlip;
        eyeParent = GetComponent<SimpleEye>();

        InitializeWallCheck();
        Debug.Log(transform.rotation.eulerAngles.y);
        particleSystem = GetComponentInChildren<ParticleSystem>();
        audio = GetComponent<AudioSource>();
        audio.maxDistance = GetComponent<SphereCollider>().radius;

        transition = GetComponentInChildren<Transition>();
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isActive = true;
            Debug.Log("entered");
            flip.eyeThatSeesList.Add(this);

            Toolbox.Instance.SetVolume(audio);
            if (audio)
                audio.mute = false;

            eyeParent.BeamActivate();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isActive = false;
            Debug.Log("exited");
            flip.eyeThatSeesList.Remove(this);
            eyeParent.BeamReset();
            if (audio)
                audio.mute = true;

            eyeParent.BeamDeactivate();
        }
    }

    private void OnDisable()
    {
        if (flip.eyeThatSeesList.Contains(this))
            flip.eyeThatSeesList.Remove(this);
    }

    private void FailedFlip()
    {
        if (isActive)
        {
            CameraShake.Shake(0.15f, .05f);
            ParticleEffect();
            AudioSource audio = SoundBox.Instance.thisSource;
            //audio.clip = SoundBox.Instance.flipFail;
            //Toolbox.Instance.SetVolume(audio);
            //audio.Play();

            MFPP.Audio.Play(SoundBox.Instance.flipFail, Toolbox.Instance.soundEffectsVolume, 1f);
        }

    }

    void ParticleEffect()
    {
        if (particleSystem)
        {
            var mainModule = particleSystem.main;

            var emissionModule = particleSystem.emission;
            var internalTime = particleSystem.time;
            var thisBurst = new ParticleSystem.Burst(Time.fixedDeltaTime, (short)particleCount, (short)particleCount, 1, 0f);
            emissionModule.burstCount = 1;
            emissionModule.SetBurst(0, thisBurst);
            //emissionModule.rateOverTime = 0.0f;

            particleSystem.Play();
        }
    }

    private void Update()
    {
        if (isActive)
        {

            if (CheckForPlayer())
            {
                blockingFlip = true;
                eyeParent.BeamActivate();
                //Debug.Log("woohoo!");
            }

            else
            {
                eyeParent.BeamDeactivate();
                blockingFlip = false;
            }
                
            

        }
        else
        {
            blockingFlip = false;
            eyeParent.BeamDeactivate();
        }
    }

    private bool CheckForPlayer()
    {
        RaycastHit hit;

        Vector3 eyeLevel = new Vector3 (player.position.x, player.position.y + 1.5f, player.position.z);
        Vector3 footLevel = new Vector3(player.position.x, player.position.y + 0.15f, player.position.z);

        currentLayerMask = LayerMaskController.GetLayerMaskForRaycast(player.gameObject.layer);

        bool checkEyeLevel = false;
        bool checkFootLevel = false;

        if (Physics.Linecast(transform.position, eyeLevel, out hit, currentLayerMask))
        {

            eyeParent.hitPoint = hit.point;
            Debug.DrawLine(transform.position, hit.point, Color.red, .1f);


            //so the simpleEye can run WallCheck;
            currentPlayerPoint = hit.point;

            if ((hit.collider.CompareTag("Player") || hit.collider.CompareTag("Clickable")) && (WallCheck(hit.point)))
                checkEyeLevel = true;

                
            else
                checkEyeLevel = false;
        }

        //if (Physics.Linecast(transform.position, footLevel, out hit, currentLayerMask))
        //{

        //    eyeParent.hitPoint = hit.point;
        //    Debug.DrawLine(transform.position, hit.point, Color.red, .1f);


        //    //so the simpleEye can run WallCheck;
        //    currentPlayerPoint = hit.point;

        //    if (hit.collider.CompareTag("Player") && (WallCheck(hit.point)))
        //        checkFootLevel = true;


        //    else
        //        checkFootLevel = false;
        //}

       // Debug.Log("eyelevel " + checkEyeLevel + " footlevel " + checkFootLevel);


        if (checkEyeLevel)// || checkFootLevel)
        {
            eyeParent.hittingPlayer = true;
            return true;
        }

        else
        {
            eyeParent.hittingPlayer = false;
            return false;  
        }
    }

    public bool WallCheck(Vector3 hit)
    {
        if (!onWall)
            return true;

        else 
        {
            Vector3 relativePos = (transform.position - hit);


            switch (myDirection)
            {
                case EyeDirection.Forward:
                    {
                        if (relativePos.z <= 0)
                            return true;

                        else return false;
                    }

                case EyeDirection.Backward:
                    {
                        if (relativePos.z >= 0)
                            return true;
                        else return false;
                                            }

                case EyeDirection.Right:
                    {
                        if (relativePos.x >= 0)
                            return true;
                        else return false;

                    }

                case EyeDirection.Left:
                    {
                        if (relativePos.x <= 0)
                            return true;
                        else return false;
                    }

                default:
                    return false;
            }

        }

    }

    private void InitializeWallCheck()
    {
        Debug.Log(transform.rotation.eulerAngles.y);
        //facing forward: looking for 0
        if (transform.rotation.eulerAngles.y >= 0f && transform.rotation.eulerAngles.y <= 10f)
            myDirection = EyeDirection.Forward;
        //facing left: looking for 90
        if (transform.rotation.eulerAngles.y >= 80f && transform.rotation.eulerAngles.y <= 100f)
            myDirection = EyeDirection.Left;
        
        //facing backward: looking for 180
        if (transform.rotation.eulerAngles.y >= 170f && transform.rotation.eulerAngles.y <= 190f)
            myDirection = EyeDirection.Backward;

        //facing right: looking for 270
        if (transform.rotation.eulerAngles.y >= 260f && transform.rotation.eulerAngles.y <= 280f)
            myDirection = EyeDirection.Right;

    }


}
