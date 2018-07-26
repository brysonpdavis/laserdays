using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMover : MonoBehaviour {
    


    private IEnumerator MovePlatformCoroutine(Vector3 startPos, Vector3 endPos, float duration)
    {
        float elapsedTime = 0;
        float ratio = elapsedTime / duration;
        while (ratio < 1f)
        {
            elapsedTime += Time.deltaTime;
            ratio = elapsedTime / duration;
            transform.position = Vector3.Lerp(startPos, endPos, ratio);
            yield return null;
        }
        transform.position = endPos;
        yield return null;
    }

    public void MovePlatform(Vector3 startPos, Vector3 endPos, float duration)
    {
        if (!(this.transform.position == startPos) && !(this.transform.position == endPos))
        {
            print("hello!");
            // StopCoroutine(MovePlatformCoroutine(startPos, endPos, 0));
            StopAllCoroutines();
        }

        float actualDuration = (duration * (Vector3.Distance(this.transform.position, endPos)/Vector3.Distance(startPos, endPos)));

        print(actualDuration);
        StartCoroutine(MovePlatformCoroutine(this.transform.position, endPos, actualDuration));

        }
    }
