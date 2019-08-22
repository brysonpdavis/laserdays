using System.Collections;
using System.Collections.Generic;
using Rewired.UI.ControlMapper;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;
using UnityStandardAssets.ImageEffects;
//using UnityEditor.ShaderGraph;

public class LevelLoadingMenu : MonoBehaviour {

    public static bool gameIsPaused = true;
    public GameObject pauseMenuUI;
    public GameObject buttonContainer;
    public GameObject sensitivitySlider;
    public GameObject saveButton;

    public GameObject inputMenuButton;
    public GameObject inputMenuUI;
    public string playerScene;

    public string reset;
    [SerializeField] public static GameObject easyButtons;
    [SerializeField] public static GameObject mediumButtons;
    [SerializeField] public static GameObject hardButtons;


    public GameObject soundtrackSlider;
    public GameObject soundEffectSlider;

    GameObject textNarration;
    private bool narrationWasOn;

    private EdgeDetection edge;

    [SerializeField] public static Color completedColor = new Color32(41, 188, 34, 255);
    [SerializeField] public static Color visitedColor = new Color32(117, 97, 152, 255);

    bool transitionIsDone = true;
    public Image background;
    private Color backgroundColor;
    float startAlpha;
    public float fadeDuration = .5f;

    private bool lastSceneCompleted;
    private GameObject lastSceneButton;

    public static bool sceneIsLoading = false;

	// Use this for initialization
	void Awake () {
        
        backgroundColor = background.color;
        buttonContainer = pauseMenuUI.transform.GetChild(0).gameObject;

        easyButtons = buttonContainer.transform.GetChild(0).gameObject;
        mediumButtons = buttonContainer.transform.GetChild(1).gameObject;
        hardButtons = buttonContainer.transform.GetChild(2).gameObject;

        textNarration = GameObject.Find("TextNarration");

        narrationWasOn = false;
    }

    private void Start()
    {
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(playerScene));
        GetComponent<CanvasScaler>().enabled = true;

        edge = Camera.main.GetComponent<EdgeDetection>();

    }

    // Update is called once per frame
    void Update () {
        if (ControlManager.Instance.GetButtonDown("Pause") && !sceneIsLoading)
        {
            if (gameIsPaused && transitionIsDone)
                Resume(true);
            else
                Pause();
        }
	}

    private void Pause()
    {
        buttonContainer.SetActive(true);
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        gameIsPaused = true;
        pauseMenuUI.SetActive(true);
        soundEffectSlider.SetActive(true);
        soundtrackSlider.SetActive(true);
        sensitivitySlider.SetActive(true);
        saveButton.SetActive(true);
        inputMenuButton.SetActive(true);
        narrationWasOn = textNarration.activeSelf;
        textNarration.SetActive(false);
        
        FirstSelectedFix.OpenMenu();

        //edge.PauseMenu = 1f;

        StopAllCoroutines();
        StartCoroutine(PauseMenuIn());

        Time.timeScale = 0f;
    }

    public void Resume(bool exitPause)
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        gameIsPaused = false;
        pauseMenuUI.SetActive(false);

        if (narrationWasOn) textNarration.SetActive(true);

        //edge.PauseMenu = 0f;
       

        if (exitPause)
        {
            //ControlMapper.Close(true);
            StopAllCoroutines();
            StartCoroutine(PauseMenuOut());
        }

        Time.timeScale = 1f;
    }

    public void Home()
    {
        Time.timeScale = 1f;
        TurnOffMenuItems();
        StartCoroutine(GoHome());
    }

    public void OpenInputMenu()
    {
        inputMenuUI.SetActive(!inputMenuUI.activeInHierarchy);
    }


    public void LoadScene()
    {
        string name = EventSystem.current.currentSelectedGameObject.name;
        GameObject myButton = EventSystem.current.currentSelectedGameObject;
        string spawnPoint = EventSystem.current.currentSelectedGameObject.GetComponentInChildren<Text>().text;
        Toolbox.Instance.GetPlayer().GetComponent<MFPP.Player>().enabled = false;
        Time.timeScale = 1f;

        StartCoroutine(loadNextScene(name, spawnPoint, myButton, true));


        //set new scene button to orange
        bool newSceneCompleted = false;
        ColorBlock cb = myButton.GetComponent<Button>().colors;
        if (cb.normalColor == completedColor)
            newSceneCompleted = true;

        cb.normalColor = visitedColor;
        myButton.GetComponent<Button>().colors = cb;

        Color newBackground = lastSceneButton.GetComponent<Image>().color;
        newBackground.a = .7f;
        lastSceneButton.GetComponent<Image>().color = newBackground;

        if (lastSceneButton)
            ClearPreviousButton();

        //new scene is now previous scene
        lastSceneButton = myButton;
        lastSceneCompleted = newSceneCompleted;
    }

    public void SetMouseSensitivity()     {         float value = EventSystem.current.currentSelectedGameObject.GetComponent<Slider>().value;         Toolbox.Instance.GetPlayer().GetComponent<MFPP.Player>().Controls.MouseSensitivity = value;     } 

    public void ClearPreviousButton()
    {
        //reset previous button
        ColorBlock cb = lastSceneButton.GetComponent<Button>().colors;
        if (!(cb.normalColor == completedColor))
        {
            cb.normalColor = Color.white;

            Color newBackground = lastSceneButton.GetComponent<Image>().color;
            newBackground.a = .7f;
            lastSceneButton.GetComponent<Image>().color = newBackground;
        }

        lastSceneButton.GetComponent<Button>().colors = cb;


    }

    private void TurnOffMenuItems()
    {
        buttonContainer.SetActive(false);
        soundEffectSlider.SetActive(false);
        soundtrackSlider.SetActive(false);
        sensitivitySlider.SetActive(false);
        saveButton.SetActive(false);
        inputMenuButton.SetActive(false);
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

            var r = ratio * ratio;
            var b = 1f - ratio;
            b *= b;
            b = 1f - b;
            var c = Mathf.Lerp(r, b, ratio);

            Color newColor = Color.Lerp(fader, Color.black, ratio);

           //edge.PauseMenu = ratio;

            background.color = newColor;
            yield return null;
        }
    }

    IEnumerator FadeIn(bool outlinesFade)
    {
        float thisDuration = fadeDuration;
        if (!outlinesFade)
            thisDuration *= 2f;

        float elapsedTime = 0;
        float ratio = elapsedTime / thisDuration;
        Color fader = background.color;
       
        //fade from black to balck with white outlines
        while (ratio < 1f)
        {
            elapsedTime += Time.unscaledDeltaTime;
            ratio = elapsedTime / thisDuration;

            ratio *= 2;
            ratio = Mathf.Clamp(ratio, 0f, 1f);

            Color newColor = Color.Lerp(Color.black, Color.clear, ratio);

     

            background.color = newColor;
            yield return null;
        }

        elapsedTime = 0f;
        ratio = elapsedTime / thisDuration;

        if (outlinesFade)//fade from white outlines to scene
        {
            while (ratio < 1f)
            {
                elapsedTime += Time.unscaledDeltaTime;
                ratio = elapsedTime / thisDuration;


                edge.PauseMenu = 1f - ratio;

                yield return null;
            }
        }

        transitionIsDone = true;

        Resume(false);
    }

    IEnumerator PauseMenuIn()
    {
        float elapsedTime = 0;
        float ratio = elapsedTime / fadeDuration;
        float start = edge.PauseMenu;
        Debug.Log("START: " + edge.PauseMenu);
        while (ratio < 1f)
        {
            elapsedTime += Time.unscaledDeltaTime;
            ratio = elapsedTime / fadeDuration;

            float current = Mathf.Lerp(start, 1f, ratio);
           // Debug.Log(ratio);

            edge.PauseMenu = current;

            yield return null;
        }

        
    }

    IEnumerator PauseMenuOut()
    {
        gameIsPaused = true;
        float elapsedTime = 0;
        float ratio = elapsedTime / fadeDuration;

        while (ratio < 1f)
        {
            elapsedTime += Time.unscaledDeltaTime * 2;
            ratio = elapsedTime / fadeDuration;

            edge.PauseMenu = 1f - ratio;

            yield return null;
        }
        gameIsPaused = false;
       
    }



    IEnumerator GoHome()
    {
        StartCoroutine(FadeOut());
        yield return new WaitForSeconds(fadeDuration);
        
        GameObject p = Toolbox.Instance.GetPlayer();
        Destroy(p);
        
        Debug.LogError("Destroying Player");


        AsyncOperation _async = new AsyncOperation();
        _async = SceneManager.LoadSceneAsync("Home Menu", LoadSceneMode.Single);
        _async.allowSceneActivation = true;
        while (!_async.isDone)
        {
            yield return null;
        }

        //Toolbox.Instance.UpdateToolbox();
    }

    public IEnumerator loadNextScene(string name, string spawnPoint, GameObject myButton, bool outlinesFade)
    {
        Debug.Log("LOADING SCENE");
        sceneIsLoading = true;
        Cursor.visible = false;

        TurnOffMenuItems();

        StartCoroutine(FadeOut());
        yield return new WaitForSeconds(fadeDuration);

        //reset all of the scene's save data before re-loading in the scene
        GameObject spawn = GameObject.Find(spawnPoint);
        spawn.GetComponentInParent<SceneUniqueIds>().SceneResetSave();


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
           // SceneManager.SetActiveScene(nextScene);
        }

        //give new scene spawners reference to correct button
        GameObject[] sceneObjects = nextScene.GetRootGameObjects();
        
        Debug.Log(spawnPoint);
        if (myButton)
            GameObject.Find(spawnPoint).GetComponent<Spawner>().myButton = myButton;

        //resetting player
        Toolbox.Instance.GetRaycastManager().selectedObjs.Clear();
        if (Toolbox.Instance.GetPickUp().heldObject)
        {
            Toolbox.Instance.GetPickUp().PutDown();
            Toolbox.Instance.GetPickUp().heldObject = null;
        }

        MFPP.Player player = Toolbox.Instance.GetPlayer().GetComponent<MFPP.Player>();
        spawn = GameObject.Find(spawnPoint);

        Toolbox.Instance.GetPlayer().GetComponent<MFPP.Player>().enabled = true;
        Toolbox.Instance.GetPlayer().GetComponent<MFPP.Player>().Movement.Speed = 3;
        Vector3 teleport = GameObject.Find(spawnPoint).transform.position;
        player.TeleportTo(teleport, true);
        player.GetComponent<CharacterController>().velocity.Set(0f, 0f, 0f);


        //Look at correct angle
        Camera.main.transform.LookAt(spawn.transform.Find("LookAt"));
        float lookY;
        
        if (Camera.main.transform.eulerAngles.x > 180)
            lookY = 360 - Camera.main.transform.eulerAngles.x;
        else 
            lookY =  0 - Camera.main.transform.eulerAngles.x;



        Vector2 look = new Vector2(Camera.main.transform.eulerAngles.y, lookY);

        Debug.Log("Camera rotation " + look);

        player.TargetLookAngles = look;
        
        MajorRegions.AllRegionsDistanceCheck();
        RegionOptimization.AllRegionsDistanceCheck();
        
        Toolbox.Instance.SetSettings(spawn.GetComponent<Spawner>().WorldSettings);

        //Resume(true);

        StartCoroutine(FadeIn(outlinesFade));
        Resume(false);

        Toolbox.Instance.UpdateTransforms();
        sceneIsLoading = false;
        
    }
}
