using System;
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
        content = txtNarration.text.Split(new string[] {"****\n"}, StringSplitOptions.None);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
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
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Toolbox.Instance.ClearNarration();
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
