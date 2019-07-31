using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.ImageEffects;


public class ResetScene : SelectableObject {

	public string spawnName;
	public string sceneName;

    public AudioSource audio;
    public float fadeDuration = .3f;
    bool entered = false;
    private EdgeDetection edge;
    private float timer = 0f;
    const float nSecond = 1f;


    public override void OnSelect()
    {
        base.OnSelect();
        Activate();
    }

    public override void OffSelect()
    {
        base.OffSelect();
    }

    public override void OnHover()
    {
        //base.OnHover();
        _iconContainer.SetReset();
        entered = true;
        if (!audio.isPlaying)
            Play();
    }

    public override void OffHover()
    {
        base.OffHover();
        entered = false;
        timer = 0f;
        edge.PauseMenu = 0;
    }

    private void Update()
    {
        if (entered)
        {
            //Increment timer
            timer += Time.deltaTime;

            float value = Mathf.Clamp((timer / nSecond), 0f, .95f);
            Debug.Log(value);
            edge.PauseMenu = value;
        }
    }

    public override void Start()
    {
        base.Start();
        audio = GetComponent<AudioSource>();
        audio.volume = Toolbox.Instance.soundEffectsVolume;
        edge = Camera.main.GetComponent<EdgeDetection>();
    }

    public void Activate()
    {
        Toolbox.Instance.LoadScene(sceneName, spawnName);
    }

    public void Play()
    {
        audio.volume = Toolbox.Instance.soundEffectsVolume;
        StopAllCoroutines();
        audio.Play();
    }

    public void Deactivate()
    {
        //audio.Stop();
        StartCoroutine(VolumeFade(audio, 0f, fadeDuration));
    }

    void SceneResetOff()
    {
        edge.PauseMenu = 0;
        timer = 0;
        Deactivate();
    }


    IEnumerator VolumeFade(AudioSource _AudioSource, float _EndVolume, float _FadeLength)
    {

        float _StartVolume = _AudioSource.volume;
        float _StartTime = Time.time;

        while (Time.time < _StartTime + _FadeLength)
        {

            _AudioSource.volume = _StartVolume + ((_EndVolume - _StartVolume) * ((Time.time - _StartTime) / _FadeLength));
            yield return null;

        }

        if ( _EndVolume == 0f) { _AudioSource.Stop(); }

    }
}
