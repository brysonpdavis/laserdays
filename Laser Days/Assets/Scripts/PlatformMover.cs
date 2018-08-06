using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMover : MonoBehaviour {

    private PlatformGuard platformGuard;

    private void Start()
    {
        platformGuard = GetComponentInChildren<PlatformGuard>();
    }

    private IEnumerator MovePlatformCoroutine(Vector3 startPos, Vector3 endPos, float duration)
    {
        float elapsedTime = 0;
        float ratio = elapsedTime / duration;
        PlatformObjectsUnselectable();

        while (ratio < 1f)
        {
            elapsedTime += Time.deltaTime;
            ratio = elapsedTime / duration;
            transform.position = Vector3.Lerp(startPos, endPos, ratio);
            yield return null;
        }
        transform.position = endPos;
        PlatformObjectSelectable();
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
        foreach (GameObject obj in platformGuard.stuckObjects)
        {
            obj.tag = ("NoTouch");
        }
    }

    void PlatformObjectSelectable()
    {
        foreach (GameObject obj in platformGuard.stuckObjects)
        {
            if (obj.GetComponent<ItemProperties>().unflippable){obj.tag = ("Sokoban");}

            else {obj.tag = ("MorphOn");}

        }
    }

    }
