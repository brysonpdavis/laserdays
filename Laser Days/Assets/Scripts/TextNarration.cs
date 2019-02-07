using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class TextNarration : MonoBehaviour
{

    public string text;
    GameObject container;
    private Text canvasText;
    public bool lore = false;
    public bool singleActivation;
    bool activated = false;
    private GameObject background;


    // Use this for initialization
    void Start()
    {
        //container = GameObject.Find("TextNarration");
        //canvasText = container.GetComponentInChildren<Text>();
        //background = container.gameObject.GetComponentInChildren<Image>().gameObject;

        if (GetComponent<UnityEngine.UI.Text>())
           text = GetComponent<UnityEngine.UI.Text>().text;

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //canvasText.text = text;

            if (!singleActivation || (singleActivation && !activated))
            {
                Toolbox.Instance.SetNarration(text);
                activated = true;

                if (lore)
                    Toolbox.Instance.PlaySoundEffect(SoundBox.Instance.narrationSound);
                else
                    Toolbox.Instance.PlaySoundEffect(SoundBox.Instance.narrationSound);
            }

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Toolbox.Instance.ClearNarration();
        }
    }

}
