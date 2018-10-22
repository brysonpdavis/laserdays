using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextNarration : MonoBehaviour {

    public string text;
    private Text canvasText;


	// Use this for initialization
	void Start () {
        canvasText = GameObject.Find("TextNarration").GetComponent<Text>();
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canvasText.text = text;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canvasText.text = null;
        }
    }

}
