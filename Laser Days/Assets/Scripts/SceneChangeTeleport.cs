using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SceneChangeTeleport : MonoBehaviour {

    float fadeDuration;
    [SerializeField] Image background;
    AudioSource audio;
    public AudioSource highAudio;
    public AudioSource lowAudio;
    public MFPP.FlipClipAsset nextFlipClip;
    public Transform teleportPosition;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            LoadScene();
    }

    private void Start()
    {
        background = GameObject.Find("Fade").GetComponent<Image>();
        audio = GetComponent<AudioSource>();
        fadeDuration = audio.clip.length;

    }

    private void LoadScene()
    {
        audio.Play();
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        AudioSource playerAudio = Toolbox.Instance.GetSoundTrackAudio();

        float audioStart = playerAudio.volume;

        float elapsedTime = 0;
        float ratio = elapsedTime / fadeDuration;
        Color fader = background.color;
        while (ratio < 1f)
        {
            elapsedTime += Time.deltaTime;
            ratio = elapsedTime / fadeDuration;
            Color newColor = Color.Lerp(fader, Color.black, ratio);
            background.color = newColor;

            // audio fadeout
            float volumeFade = Mathf.Lerp(audioStart, 0f, ratio);
            highAudio.volume = volumeFade;
            lowAudio.volume = volumeFade;

            yield return null;
        }

        GameObject player = Toolbox.Instance.GetPlayer();
        SoundTrackManager soundTrack = player.GetComponentInChildren<SoundTrackManager>();

        //teleport player
        player.GetComponent<MFPP.Player>().TeleportTo(teleportPosition.position, true);

        //unmute soundtrack
        soundTrack.mute = false;

        //switch flip sound
        soundTrack.flipClip = nextFlipClip;

        //fade in

        elapsedTime = 0;
        ratio = 0;

        fader = background.color;
        while (ratio < 1f)
        {
            elapsedTime += Time.deltaTime;
            ratio = elapsedTime / fadeDuration;
            Color newColor = Color.Lerp(fader, Color.clear, ratio);
            background.color = newColor;

            yield return null;
        }


    }


}
