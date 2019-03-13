using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

public class MainMenu : MonoBehaviour {

    public float fadeDuration = .5f;
    public string resetScene;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    //IEnumerator FadeOut()
    //{
    //    float elapsedTime = 0;
    //    float ratio = elapsedTime / fadeDuration;
    //    Color fader = background.color;
    //    while (ratio < 1f)
    //    {
    //        elapsedTime += Time.deltaTime;
    //        ratio = elapsedTime / fadeDuration;
    //        Color newColor = Color.Lerp(fader, Color.black, ratio);
    //        background.color = newColor;
    //        yield return null;
    //    }
    //}

    public void RestartScene()
    {
        StartCoroutine(Restart());
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    IEnumerator Restart()
    {
        //StartCoroutine(FadeOut());
        //yield return new WaitForSeconds(fadeDuration);

        AsyncOperation _async = new AsyncOperation();
        _async = SceneManager.LoadSceneAsync(resetScene, LoadSceneMode.Single);
        _async = SceneManager.LoadSceneAsync("Default_Main_Player January", LoadSceneMode.Additive);
        _async.allowSceneActivation = true;
        while (!_async.isDone)
        {
            yield return null;
        }
        Debug.Log("done resetting");
        Toolbox.Instance.UpdateToolbox();
    }
}
