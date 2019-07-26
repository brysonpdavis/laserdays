using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextNarration : MonoBehaviour
{

    GameObject container;
    private Text canvasText;
    public bool lore = false;
    public bool singleActivation;
    bool activated = false;
    private GameObject background;
	public TextAsset txtNarration;
    private string[] content;

    void Awake()
    {
        content = txtNarration.text.Split(new string[] {"****\n", "****\r\n"}, StringSplitOptions.None);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Activate();
        }
    }

    public void Activate()
    {



        if (!singleActivation || (singleActivation && !activated))
        {
            Toolbox.Instance.NewNarration(this);
            activated = true;

            if (lore)
                Toolbox.Instance.PlaySoundEffect(SoundBox.Instance.narrationSound);
            else
                Toolbox.Instance.PlaySoundEffect(SoundBox.Instance.narrationSound);
        }

    }

    public void Deactivate()
    {
        Toolbox.Instance.ClearNarration();
 
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Deactivate();
        }
    }

    public string GetContent(int n)
    {
        if (n < content.Length)
        {
            return content[n];
        }
        else
        {
            return "Oops! No string here.";
        }
    }

	public int GetContentLength()
	{
		return content.Length;
	}

}
