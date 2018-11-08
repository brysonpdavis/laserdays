using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shake : MonoBehaviour {

    private Vector3 orignalPos;

    private void Start()
    {
        orignalPos = transform.localPosition;
    }

    public void stopYOU()
    {
        StopAllCoroutines();
    } 

    public IEnumerator Shake (float dur, float mag){

        Vector3 originalpos = orignalPos;

        float elapsed = 0;
        float oz = originalpos.z - mag;

        transform.localPosition = new Vector3(originalpos.x, originalpos.y, oz);

        while (elapsed < dur)
        {
            float x = Random.Range(-1, 1) * mag;
            float y = Random.Range(-1, 1) * mag;

            float v;

           

                v = Mathf.Lerp(0, mag, elapsed / dur);
                transform.localPosition = new Vector3(originalpos.x, originalpos.y, oz + v);
           

            elapsed += Time.deltaTime;

            yield return null;

        }
            transform.localPosition = originalpos;
    }
}
