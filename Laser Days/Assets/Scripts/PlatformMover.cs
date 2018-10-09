using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMover : MonoBehaviour {

    public PlatformGuard platformGuard;

    public GameObject platformContainer;
    public PlatformIndicator Indicator;

    public GameObject mainGuard;
    public PlatformObjectMover[] objectMovers;
    public  Vector3 start;
    public Transform end;

    private RaycastManager raycastManager;
    private MFPP.Modules.PickUpModule pickUp;

    private void Start()
    {

        start = this.transform.position;

        this.Indicator.SetColors(platformContainer.GetComponent<PlatformController>().PassiveColor,
                            platformContainer.GetComponent<PlatformController>().ActiveColor);

        raycastManager = Toolbox.Instance.GetPlayer().GetComponent<RaycastManager>();
        pickUp = Toolbox.Instance.GetPlayer().GetComponent<MFPP.Modules.PickUpModule>();

        LineRenderer LR = gameObject.GetComponentInChildren<LineRenderer>();
        LR.positionCount = 2;
        Vector3 begin = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y - 0.1f, gameObject.transform.position.z);
        LR.SetPosition(0, begin);
        Vector3 finish = new Vector3(end.position.x, end.position.y - 0.1f, end.position.z);
        LR.SetPosition(1, finish);
    }

    private IEnumerator MovePlatformCoroutine(Vector3 startPos, Vector3 endPos, float duration)
    {
        //Debug.Log("moving again" + this.name);
        float elapsedTime = 0;
        float ratio = elapsedTime / duration;
        checkObjectsPlace();
        PlatformObjectsUnselectable();

        yield return new WaitForSeconds(.2f);


        while (ratio < 1f)
        {
            elapsedTime += Time.deltaTime;
            ratio = elapsedTime / duration;
            transform.position = Vector3.Lerp(startPos, endPos, ratio);
            yield return null;
        }
        transform.position = endPos;
        PlatformObjectSelectable();
        mainGuard.SetActive(true);
        yield return null;



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

    }

    public void IndicatorOff()
    {
        Indicator.Off();

    }

    }
