using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

public class MainMenu : MonoBehaviour {

    public float fadeDuration = .5f;
    public string resetScene;
    public string playerScene;
    public Image[] backgrounds;
    public GameObject container;

    [SerializeField] private GameObject listener;


	// Use this for initialization
	void Start () {

        OpeningSongSingleton.Instance.mute = false;
        OpeningSongSingleton.Instance.Play();
        
        if (ControlManager.GetControllerState() == ControlManager.ControllerState.JoystickPS4)
            EventSystem.current.SetSelectedGameObject(null);

        DontDestroyOnLoad(listener);
        listener.SetActive(false);
    }
    
    IEnumerator FadeOut()
    {
        float elapsedTime = 0;
        float ratio = elapsedTime / fadeDuration;
        Color[] fader = new Color[backgrounds.Length];

        for (int i = 0; i < backgrounds.Length; i ++)
        {
            fader[i] = backgrounds[i].color;
        }

        while (ratio < 1f)
        {
            elapsedTime += Time.deltaTime;
            ratio = elapsedTime / fadeDuration;
            for (int i = 0; i < backgrounds.Length; i++)
            {
                Color newColor = Color.Lerp(fader[i], Color.clear, ratio);
                backgrounds[i].color = newColor;
            }

            yield return null;
        }
        
        listener.SetActive(true);
        listener.GetComponent<TemporaryListener>().StartWaitingForPlayer();
    }

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
        
        container.SetActive(false);

        StartCoroutine(FadeOut());
        yield return new WaitForSeconds(fadeDuration);
        
        Destroy(Toolbox.Instance.player);

        AsyncOperation _async = new AsyncOperation();
        _async = SceneManager.LoadSceneAsync(resetScene, LoadSceneMode.Single);
        _async = SceneManager.LoadSceneAsync(playerScene, LoadSceneMode.Additive);
        _async.allowSceneActivation = true;
        while (!_async.isDone)
        {
            yield return null;
        }
        Debug.Log("done resetting");
        Toolbox.Instance.UpdateToolbox();
    }
}
