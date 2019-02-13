using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMover : MonoBehaviour {

    public PlatformGuard platformGuard;
    public GameObject platformContainer;
    public PlatformIndicator Indicator;
    public GameObject mainGuard;
    public PlatformObjectMover[] objectMovers;
    [HideInInspector] public  Vector3 start;
    public Transform end;
    bool initialized = false;

    public float durationMultiplier = 1f;
    private RaycastManager raycastManager;
    private MFPP.Modules.PickUpModule pickUp;
    private LineRenderer LR;
    private AudioSource audio;
    private bool playerLayer;
    public bool platformIsMoving = false;

    public void Start()
    {
        if (!initialized)
        {
            start = this.transform.position;

            Color RC = platformContainer.GetComponent<PlatformController>().RestingColor;
            Color AC = platformContainer.GetComponent<PlatformController>().ActiveColor;
            Color SC = platformContainer.GetComponent<PlatformController>().ShimmerColor;
            Texture2D ST = platformContainer.GetComponent<PlatformController>().ScrollText;

            this.Indicator.SetColors(RC, AC, SC, ST);

            raycastManager = Toolbox.Instance.GetPlayer().GetComponent<RaycastManager>();
            pickUp = Toolbox.Instance.GetPlayer().GetComponent<MFPP.Modules.PickUpModule>();

            LR = gameObject.transform.parent.gameObject.GetComponentInChildren<LineRenderer>();
            LR.positionCount = 2;
            Vector3 begin = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y - 0.1f, gameObject.transform.position.z);
            LR.SetPosition(0, begin);
            Vector3 finish = new Vector3(end.position.x, end.position.y - 0.1f, end.position.z);
            LR.SetPosition(1, finish);
            LR.material.SetColor("_RestingColor", RC);
            LR.material.SetColor("_ActiveColor", AC);

            //setup the audio
            if (!GetComponent<AudioSource>())
                this.gameObject.AddComponent<AudioSource>();
            audio = GetComponent<AudioSource>();
            audio.spatialBlend = 1f;
            audio.playOnAwake = false;

            initialized = true;
        }

    }

    private IEnumerator MovePlatformCoroutine(Vector3 startPos, Vector3 endPos, float duration)
    {
        checkObjectsPlace();
        platformIsMoving = true;
        Debug.Log("Platformismoving");

        yield return new WaitForSeconds(.5f);
        PlayAudio(SoundBox.Instance.platformStart);
        yield return new WaitForSeconds(SoundBox.Instance.platformStart.length);



        audio.loop = true;
        PlayAudio(SoundBox.Instance.platformRunning);

        float elapsedTime = 0;
        float ratio = elapsedTime / (duration * durationMultiplier);

        PlatformObjectsUnselectable();

        Vector3 carriedObjectPosition = Vector3.zero;

        if (mainGuard.GetComponent<PlatformGuard>().target)
        {
            carriedObjectPosition = mainGuard.GetComponent<PlatformGuard>().target.transform.position;
            carriedObjectPosition += endPos;

            Debug.Log(mainGuard.GetComponent<PlatformGuard>().target.transform.position.y);
            Debug.Log(carriedObjectPosition.y);
        }

        //yield return new WaitForSeconds(.5f);
        //play starting sound


        while (ratio < 1f)
        {

            if (!playerLayer)
            {
                audio.mute = true;
            }
            else
                audio.mute = false;



            elapsedTime += Time.deltaTime;
            ratio = elapsedTime / (duration * durationMultiplier);
            transform.position = Vector3.Lerp(startPos, endPos, ratio);


            yield return null;
        }
        transform.position = endPos;
        PlatformObjectSelectable();
        mainGuard.SetActive(true);
        yield return null;


        audio.mute = true;
        audio.loop = false;
        yield return new WaitForSeconds(.2f);
        audio.Stop();
        audio.mute = false;
        PlayAudio(SoundBox.Instance.platformEnd);

        if (mainGuard.GetComponent<PlatformGuard>().target)
        {
            mainGuard.GetComponent<PlatformGuard>().target.transform.position = carriedObjectPosition += endPos;
        }

        platformIsMoving = false;
        Debug.Log("platformStoppedMoving");


    }

    private void OnDisable()
    {
        StopAllCoroutines();
        platformIsMoving = false;
        Debug.Log("platformStoppedMoving");
        if (audio && audio.isPlaying)
            audio.Stop();
    }

    private void Update()
    {
        if (Toolbox.Instance.GetPlayer().layer == (gameObject.layer + 5))
            playerLayer = true;
        else
            playerLayer = false;
            
    }

    public void MovePlatform(Vector3 startPos, Vector3 endPos, float duration)
    {
        if (!(this.transform.position == startPos) && !(this.transform.position == endPos))
        {
            StopAllCoroutines();
        }

        float actualDuration = (duration * (Vector3.Distance(this.transform.position, endPos)/Vector3.Distance(startPos, endPos)));

        StartCoroutine(MovePlatformCoroutine(this.transform.position, endPos, actualDuration));

        }

    public void PlatformObjectsUnselectable()
    {
        if (platformGuard.isActiveAndEnabled)
        {
            foreach (GameObject obj in platformGuard.stuckObjects)
            {
                InteractableObject item = obj.GetComponent<InteractableObject>();
                if (!obj.CompareTag("Player"))
                {
                    obj.tag = ("NoTouch");

                    //makes sure all morphs get unselected when the platform is in motion!
                    if (item.objectType == InteractableObject.ObjectType.Morph && item.selected)
                    {
                        raycastManager.RemoveFromList(obj, false, false);
                        raycastManager.selectedObjs.Remove(obj);
                        item.selected = false;
                    }

                }

                if (pickUp.heldObject && pickUp.heldObject.Equals(obj))
                {
                    Debug.Log("Yo");
                    pickUp.PutDown();
                }


            }
        }

    }

    public void PlatformObjectSelectable()
    {

        foreach (GameObject obj in platformGuard.stuckObjects)
        {
            if (obj.GetComponent<InteractableObject>())
            {
                obj.tag = "Clickable";
            }
        }
    }

    public void PlatformStuckSelectable()
    {

        foreach (GameObject obj in platformGuard.stuckObjects)
        {
            if (obj.GetComponent<InteractableObject>() && !(obj.GetComponent<InteractableObject>().objectType == InteractableObject.ObjectType.Morph))
            {
                obj.tag = "Clickable";
            }
        }
    }


    void PlayAudio(AudioClip clip)
    {
        if (playerLayer)
        {
            audio.clip = clip;
            audio.volume = Toolbox.Instance.soundEffectsVolume;
            audio.Play();
        }
    }

    void checkObjectsPlace(){

        foreach (PlatformObjectMover mover in objectMovers){

            if (mover.incorrect)
            {
                mover.centerObject();
            }

        }

    }

    public void IndicatorOn()
    {
        Indicator.On();
        LR.material.SetFloat("_isActive", 1);
    


    }

    public void IndicatorOff()
    {
        Indicator.Off();
        LR.material.SetFloat("_isActive", 0);

    }

    }
