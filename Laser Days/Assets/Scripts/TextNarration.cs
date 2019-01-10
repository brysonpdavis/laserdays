using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextNarration : MonoBehaviour {

    public string text;
    private Text canvasText;
    public bool singleActivation;
    bool activated = false;
    private GameObject background;


	// Use this for initialization
	void Start () {
        //canvasText = GameObject.Find("TextNarration").GetComponent<Text>();
        //background = canvasText.gameObject.GetComponentInChildren<Image>();
        //background.gameObject.SetActive(false);

        canvasText = Toolbox.Instance.GetNarrationText();
        background = Toolbox.Instance.GetNarrationBackground();

        if (background.activeInHierarchy)
            background.SetActive(false);

	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {

            if (!singleActivation || (singleActivation&& !activated))
            {
                canvasText.text = text;
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
            //background.enabled = false;
            background.SetActive(false);
        }
    }

}
