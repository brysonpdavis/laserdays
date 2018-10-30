using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TimedText : MonoBehaviour {

    public string text;
    public float waitMinutes = 2;
    public float durationSeconds = 10f;
    private Text canvasText;


    // Use this for initialization
    void Start()
    {
        canvasText = GameObject.Find("TextNarration").GetComponent<Text>();
    }



    private void OnTriggerEnter(Collider other)
    {
        StartCoroutine(WaitForText());

    }

    private void OnTriggerExit(Collider other)
    {
        canvasText.text = null;
        StopAllCoroutines();  
    }

    private IEnumerator WaitForText()
    {
        yield return new WaitForSeconds(waitMinutes*60f);
        canvasText.text = text;
        yield return new WaitForSeconds(durationSeconds);
        canvasText.text = null;

    }



}
