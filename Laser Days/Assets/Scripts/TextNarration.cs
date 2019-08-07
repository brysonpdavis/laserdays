using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextNarration : MonoBehaviour
{
    enum PopUpType { Static, Animated };

    [SerializeField]
    private PopUpType type = PopUpType.Static;

    GameObject container;
    private Text canvasText;
    public bool lore = false;
    public bool singleActivation;
    bool activated = false;
    private GameObject background;
	public TextAsset txtNarration;
    private string[] content;
    private Coroutine _coroutine;
    [SerializeField] private bool persistent = true;
    [SerializeField] private bool delayedHint;
    [SerializeField] private float hintDelay;

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
            activated = true;

            if (delayedHint)
            {
                StartCoroutine(HintCoroutine());
            }
            else
            {
                StartNarration();
            }
        }
    }

    private IEnumerator HintCoroutine()
    {
        yield return new WaitForSeconds(hintDelay);
        
        StartNarration();
    }

    private void StartNarration()
    {
        if(type == PopUpType.Static)
        {
            Toolbox.Instance.NewNarration(this);
        } 
        else if (type == PopUpType.Animated)
        {
            Toolbox.Instance.NewNarrationAnimated(this);
        }

        if (lore)
            Toolbox.Instance.PlaySoundEffect(SoundBox.Instance.narrationSound);
        else
            Toolbox.Instance.PlaySoundEffect(SoundBox.Instance.narrationSound);

    }

    public void CancelHint()
    {
        if (_coroutine != null)
            StopCoroutine(_coroutine);
    }
    
    public void Deactivate()
    {
        Toolbox.Instance.ClearNarration();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!persistent && other.CompareTag("Player")) 
            Deactivate();
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
