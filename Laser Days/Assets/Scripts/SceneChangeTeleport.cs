using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SceneChangeTeleport : MonoBehaviour {

    float fadeDuration;
    AudioSource audio;

    //[SerializeField] private AudioSource[] audioSources;
    [SerializeField] private string sceneName;
    [SerializeField] private string spawnName;
    [SerializeField] OpeningSongSingleton openingSong;
    [SerializeField] TriggeredAudio bassAudio;
    [SerializeField] MFPP.FlipClipAsset mainFlip;
    [SerializeField] AmbientSound ambient;
    [SerializeField] AudioSource ambientReal;
    [SerializeField] AudioSource ambientLaser;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            LoadScene();
    }

    private void Start()
    {
        audio = GetComponent<AudioSource>();
        fadeDuration = audio.clip.length;
        //openingSong = OpeningSongSingleton.Instance;
        //bassAudio = TriggeredAudio.Instance;
    }

    private void LoadScene()
    {
        audio.Play();
        Toolbox.Instance.LoadScene(sceneName, spawnName);
        StartCoroutine(FadeOutVolume());
    }

    IEnumerator FadeOutVolume()
    {
        //turn the soundtrack on with default flip clip- make sure it's audio is set to zero
        SoundTrackManager soundTrack = Toolbox.Instance.GetPlayer().GetComponentInChildren<SoundTrackManager>();
        soundTrack.dynamicVolume = 0f;
        soundTrack.flipClip = mainFlip;
        soundTrack.SetVolume();

        //turn off opening and bass audio completely - volume should be at zero from ambient zone
        //doing this so that they don't turn back on when teleporting to a new location
        openingSong.GetComponent<AudioSource>().enabled = false;
        bassAudio.GetComponent<AudioSource>().enabled = false;

        //turning off the ambient script so that we can set its real and laser volume manually for this fade
        ambient.percentageOfSoundtrack = 0f;
        ambient.enabled = false;


        float elapsedTime = 0;
        float ratio = elapsedTime / fadeDuration;

        float ambRealStart = ambientReal.volume;
        float ambLaserStart = ambientLaser.volume;
        while (ratio < 1f)
        {
            elapsedTime += Time.deltaTime;
            ratio = elapsedTime / fadeDuration;

            // audio fade out/in
            float fadeOutReal = Mathf.Lerp(ambRealStart, 0f, ratio);
            float fadeOutLaser = Mathf.Lerp(ambLaserStart, 0f, ratio);

            ambientReal.volume = fadeOutReal;
            ambientLaser.volume = fadeOutLaser;

            float fadeIn = Mathf.Lerp(0f, 1f, TweeningFunctions.EaseIn(ratio));
            soundTrack.dynamicVolume = fadeIn;

            yield return null;
        }


    }


}
