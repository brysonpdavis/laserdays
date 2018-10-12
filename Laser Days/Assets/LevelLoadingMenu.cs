using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

public class LevelLoadingMenu : MonoBehaviour {

    public static bool gameIsPaused = true;
    public GameObject pauseMenuUI;
    public GameObject buttonContainer;
    public GameObject soundtrackSlider;
    public GameObject soundEffectSlider;
    bool transitionIsDone = true;
    public Image background;
    private Color backgroundColor;
    float startAlpha;
    public float fadeDuration = .5f;

	// Use this for initialization
	void Start () {

        background = pauseMenuUI.GetComponent<Image>();
        backgroundColor = background.color;
        buttonContainer = pauseMenuUI.transform.GetChild(0).gameObject;
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (gameIsPaused && transitionIsDone)
                Resume();
            else
                Pause();
        }
	}

    private void Pause()
    {
        buttonContainer.SetActive(true);
        background.color = backgroundColor;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        gameIsPaused = true;
        pauseMenuUI.SetActive(true);
        soundEffectSlider.SetActive(true);
        soundtrackSlider.SetActive(true);
        Time.timeScale = 0f;
    }

    void Resume()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        gameIsPaused = false;
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
    }

    public void LoadScene()
    {
        string name = EventSystem.current.currentSelectedGameObject.name;
        GameObject myButton = EventSystem.current.currentSelectedGameObject;
        string spawnPoint = EventSystem.current.currentSelectedGameObject.GetComponentInChildren<Text>().text;
        Toolbox.Instance.GetPlayer().GetComponent<MFPP.Player>().enabled = false;
        Time.timeScale = 1f;
        StartCoroutine(loadNextScene(name, spawnPoint, myButton));
    }

    IEnumerator FadeOut()
    {
        float elapsedTime = 0;
        float ratio = elapsedTime / fadeDuration;
        Color fader = background.color;
        while (ratio < 1f)
        {
            elapsedTime += Time.deltaTime;
            ratio = elapsedTime / fadeDuration;
            Color newColor = Color.Lerp(fader, Color.black, ratio);
            background.color = newColor;
            yield return null;
        }
    }

    IEnumerator FadeIn()
    {
        float elapsedTime = 0;
        float ratio = elapsedTime / fadeDuration;
        Color fader = background.color;
        while (ratio < 1f)
        {
            elapsedTime += Time.deltaTime;
            ratio = elapsedTime / fadeDuration;
            Color newColor = Color.Lerp(Color.black, Color.clear, ratio);
            background.color = newColor;
            yield return null;
        }
        transitionIsDone = true;
        Resume();
    }


    IEnumerator loadNextScene(string name, string spawnPoint, GameObject myButton)
    {

        Cursor.visible = false;


        buttonContainer.SetActive(false);
        soundEffectSlider.SetActive(false);
        soundtrackSlider.SetActive(false);

        StartCoroutine(FadeOut());
        yield return new WaitForSeconds(fadeDuration);
  
        AsyncOperation _async = new AsyncOperation();
        SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName(name));

        _async = SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);
        _async.allowSceneActivation = true;

        while (!_async.isDone)
        {
            transitionIsDone = false;
            yield return null;
        }
        transitionIsDone = true;
        Scene nextScene = SceneManager.GetSceneByName(name);

        if (nextScene.IsValid())
        {
            SceneManager.SetActiveScene(nextScene);
        }

        //give new scene spawners reference to correct button
        GameObject[] sceneObjects = nextScene.GetRootGameObjects();
        GameObject newSpawner = sceneObjects[1].transform.GetChild(0).Find(spawnPoint).gameObject;
        if (newSpawner)
            newSpawner.GetComponent<Spawner>().myButton = myButton;
        else
            Debug.Log("help!");
        StartCoroutine(FadeIn());


        //resetting player
        Toolbox.Instance.GetRaycastManager().selectedObjs.Clear();
        if (Toolbox.Instance.GetPickUp().heldObject)
        {
            Toolbox.Instance.GetPickUp().PutDown();
            Toolbox.Instance.GetPickUp().heldObject = null;
        }
        Toolbox.Instance.GetPlayer().GetComponent<MFPP.Player>().enabled = true;
        Vector3 teleport = GameObject.Find(spawnPoint).transform.position;
        MFPP.Player player = Toolbox.Instance.GetPlayer().GetComponent<MFPP.Player>();
        player.TeleportTo(teleport, true);
        player.GetComponent<CharacterController>().velocity.Set(0f, 0f, 0f);

        Toolbox.Instance.UpdateTransforms();

    }
}
