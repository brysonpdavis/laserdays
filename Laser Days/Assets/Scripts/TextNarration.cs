using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextNarration : MonoBehaviour {

    public string text;
    GameObject container;
    private Text canvasText;
    public bool singleActivation;
    bool activated = false;
    private GameObject background;


	// Use this for initialization
	void Start () {
        container = GameObject.Find("TextNarration");
        canvasText = container.GetComponentInChildren<Text>();
        background = container.gameObject.GetComponentInChildren<Image>().gameObject;

	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canvasText.text = text;

            if (!singleActivation || (singleActivation && !activated))
            {
                canvasText.text = text;
                //background.enabled = true;
                background.SetActive(true);
                activated = true;
            }

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canvasText.text = null;
            background.SetActive(false); // = false;

        }
    }

}
