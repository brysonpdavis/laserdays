using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeBeam : MonoBehaviour {

    AudioSource audio;

    private void Start()
    {
        audio = GetComponentInChildren<AudioSource>();
    }

    public void SetAudioVolume()
    {
        Toolbox.Instance.SetVolume(audio);
    }

    public void SetLength (float len)
    {
        Vector3 beam = new Vector3(transform.localScale.x, transform.localScale.y, len);
        transform.localScale = beam;
    }

    public void SetWidth(float width)
    {
        Vector3 beam = new Vector3(width, width, transform.localScale.z);
        transform.localScale = beam;
    }
}
