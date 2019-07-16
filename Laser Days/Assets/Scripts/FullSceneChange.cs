using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;



public class FullSceneChange : MonoBehaviour {

    float fadeDuration;
    public string resetScene;
    [SerializeField] Image background;
    AudioSource audio;


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
        StartCoroutine(Restart());
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
            playerAudio.volume = volumeFade;

            yield return null;
        }
    }


    IEnumerator Restart()
    {
        StartCoroutine(FadeOut());
        yield return new WaitForSeconds(fadeDuration);

        Destroy(Toolbox.Instance.player);

        AsyncOperation _async = new AsyncOperation();
        _async = SceneManager.LoadSceneAsync(resetScene, LoadSceneMode.Single);
        _async.allowSceneActivation = true;
        while (!_async.isDone)
        {
            yield return null;
        }
        Debug.Log("done resetting");
        Toolbox.Instance.UpdateToolbox();
    }
}
