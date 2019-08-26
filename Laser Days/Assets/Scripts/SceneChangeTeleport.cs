using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SceneChangeTeleport : MonoBehaviour {

    public float fadeDuration;
    AudioSource audio;

    //[SerializeField] private AudioSource[] audioSources;
    [SerializeField] private string sceneName;
    [SerializeField] private string spawnName;
    //[SerializeField] OpeningSongSingleton openingSong;
    //[SerializeField] TriggeredAudio bassAudio;
    //[SerializeField] MFPP.FlipClipAsset mainFlip;
    //[SerializeField] AmbientSound ambient;
    //[SerializeField] AudioSource ambientReal;
    //[SerializeField] AudioSource ambientLaser;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            LoadScene();
    }

    private void Start()
    {
        audio = GetComponent<AudioSource>();
        //fadeDuration = audio.clip.length;
        //openingSong = OpeningSongSingleton.Instance;
        //bassAudio = TriggeredAudio.Instance;
    }

    private void LoadScene()
    {
        //audio.Play();
        Toolbox.Instance.LoadScene(sceneName, spawnName);
        //StartCoroutine(FadeOutVolume());
    }


}
