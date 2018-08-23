using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMover : MonoBehaviour {

    public PlatformGuard platformGuard;
    public GameObject mainGuard;
    public PlatformObjectMover[] objectMovers;
    public  Vector3 start;
    public Transform end;                          

    private void Start()
    {

        start = this.transform.position;
        // platformGuard = GetComponentInChildren<PlatformGuard>();
    }

    private IEnumerator MovePlatformCoroutine(Vector3 startPos, Vector3 endPos, float duration)
    {
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

    void PlatformObjectsUnselectable()
    {
        if (platformGuard.isActiveAndEnabled)
        {
            foreach (GameObject obj in platformGuard.stuckObjects)
            {
                obj.tag = ("NoTouch");
            }
        }

    }

    void PlatformObjectSelectable()
    {

        foreach (GameObject obj in platformGuard.stuckObjects)
        {
            Debug.Log("SELECT");
            if (obj.GetComponent<ItemProperties>())
            {
                if (obj.GetComponent<ItemProperties>().unflippable) { obj.tag = ("Sokoban"); }

                else { obj.tag = ("MorphOn"); }

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

    }
