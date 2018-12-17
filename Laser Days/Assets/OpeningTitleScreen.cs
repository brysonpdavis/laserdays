using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpeningTitleScreen : MonoBehaviour {

    CanvasGroup canvas;
    public float duration = 2f;


    private void Start()
    {
        canvas = GetComponent<CanvasGroup>();
        StartCoroutine(FadeOnAwake());
    }

    private IEnumerator FadeOnAwake()
    {
        yield return new WaitForSeconds(1f);

        float elapsedTime = 0;
        float ratio = elapsedTime / duration;

        while (ratio <1f)
        {
            elapsedTime += Time.deltaTime;
            ratio = elapsedTime / duration;

            float current = Mathf.Lerp(1f, 0f, ratio);
            canvas.alpha = current;
            yield return null;

        }
        


    }

}
