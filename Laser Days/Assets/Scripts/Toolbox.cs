using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Cinemachine;

public class Toolbox : Singleton<Toolbox>
{
    protected Toolbox() { } // guarantee this will be always a singleton only - can't use the constructor!

    GameObject realWorldParentObject;
    GameObject laserWorldParentObject;
    public GameObject player;
    public Camera playerCam;
    public GameObject mainCanvas;
    RaycastManager raycastManager;
    MFPP.Modules.PickUpModule pickUp;
    flipScript flipScript;
    [SerializeField]
    public float globalFlipSpeed = 1.1f;
    [SerializeField]
    public float globalRealLaserFlipSpeed = 1.1f;
    public bool loadFromSave = false;
    public bool loadSelection = false;


    IconContainer iconContainer;
    public Color UIColorA;
    public Color UIColorB;
    public Color UIColorC;

    public Shader laserCore;
    public Shader realCore;
    public GameObject pauseMenu;
    public Slider soundEffectsSlider;
    public float soundEffectsVolume = 1f;
    public IList<Material> sharedMaterials;
    public MFPP.SoundContainerAsset testercubeSounds;
    public IList<UniqueId> allIds = new List<UniqueId>();
    public AudioSource SoundEffectsAudio;
    private AudioSource SoundtrackAudio;

    public GameObject narrationContainer;
    private Animator narrationAnimator;
    public bool isNarrationAnimating;
    private Text narrationText;
    public GameObject narrationBackground;
    public GameObject narrationContinue;
    private bool narrationActive;
    private int narrationIndex;
    private TextNarration currentNarration;
    private char[] narrationWords;
    private bool wording = false;
    private int narrationWordsIndex = 0;
    private string narrationCurrentWords = "";
    private int frame_counter = 0;
    private int frames_til_draw;

    public GameObject regionController;
    public bool deleteUnusedAssets = true;

    //default fog settings
    public float fogDensityDefault;
    public float cameraFogStartDefault;


    void Awake()
    {
        //SetCustomValuesOnInstance();
        // UpdateToolbox();
        
        sharedMaterials = new List<Material>();
        DontDestroyOnLoad(this.gameObject);

        if (deleteUnusedAssets)
        {
            InvokeRepeating("DeleteUnused", 10f, 10f);
        } 
    }
    private void Update()
    {
        if (wording)
        {
            NarrationUpdate();
        }
    }

    private void DeleteUnused()
    {
        //Debug.LogError("Deleting");
        Resources.UnloadUnusedAssets();
    }
    
    void VolumeChangeCheck()
    {
        soundEffectsVolume = soundEffectsSlider.value;
        player.GetComponent<MFPP.Player>().Footstep.GlobalVolume = soundEffectsVolume;
    }

    public void SetVolume(AudioSource audio)
    {
        VolumeChangeCheck();
        audio.volume = soundEffectsVolume;
    }

    public void FullReset()
    {
        player.GetComponent<PlayerSave>().ClearObjData();
        loadSelection = true;
        //foreach (UniqueId id in allIds)
        //{
        //    id.ClearObjData();
        //}

        string path = Application.persistentDataPath;
        if (Directory.Exists(path)) { Directory.Delete(path, true); }
        Directory.CreateDirectory(path);
    }

    public void Resume(bool exitPause)
    {
        mainCanvas.GetComponent<LevelLoadingMenu>().Resume(exitPause);
    }


    public void LoadFromSave()
    {
        loadSelection = true;
        loadFromSave = true;
        foreach (UniqueId id in allIds)
        {
            if (id.enabled && id != null)
            {
                id.Setup();
            }
        }

        player.GetComponent<PlayerSave>().PlayerSetup();
    }

    public void FullSave()
    {
        //save all individual objects
        Debug.Log(allIds.Count);

        player.GetComponent<PlayerSave>().SavePlayerFile();

        foreach (UniqueId id in allIds)
        {
            if (id.enabled && id != null)
            {
                Debug.Log(id.gameObject.name);
                //id.Save();
                id.SaveObj();
            }
        }
    }

    // (optional) allow runtime registration of global objects
    static public T RegisterComponent<T>() where T : Component
    {
        return Instance.GetOrAddComponent<T>();
    }

    public Transform GetRealWorldParent()
    {
        return realWorldParentObject.transform;
    }

    public Transform GetLaserWorldParent()
    {
        return laserWorldParentObject.transform;
    }

    public GameObject GetPlayer()
    {
        return player;
    }

    public void SetPlayer(GameObject p)
    {
        player = p;
    }

    public IconContainer GetIconContainer()
    {
        return iconContainer;
    }


    public RaycastManager GetRaycastManager()
    {
        return raycastManager;
    }

    public MFPP.Modules.PickUpModule GetPickUp()
    {
        return pickUp;
    }

    public bool EqualToHeld(GameObject obj)
    {
        if (pickUp.heldObject && pickUp.heldObject.Equals(obj))
        { return true; }

        else
            return false;
    }

    public flipScript GetFlip()
    {
        return flipScript;
    }

    public bool PlayerInLaser()
    {
        return player.gameObject.layer == 15;
    }
    public bool PlayerInReal()
    {
        return player.gameObject.layer == 16;
    }

    public Shader CoreLaser()
    {
        return laserCore;
    }

    public Shader CoreReal()
    {
        return realCore;
    }


    public float EdgeGlowThickness()
    {
        return .05f;
    }

    public GameObject GetPauseMenu()
    {
        return pauseMenu;
    }

    public AudioSource GetSoundTrackAudio()
    {
        return SoundtrackAudio;
    }

    public void SetNarration(string text)
    {
        narrationActive = true;
        // narrationText.text = text;
        narrationBackground.SetActive(true);
        NarrateWords(text);
    }

    public void ClearNarration()
    {
        narrationActive = false;
        narrationText.text = null;
		narrationCurrentWords = "";
        if (narrationBackground)
            narrationBackground.SetActive(false);
        if (narrationContinue)
            narrationContinue.SetActive(false);
        narrationIndex = 0;
		wording = false;
    }

    public bool GetNarrationActive()
    {
        return narrationActive;
    }

    public void NextNarration()
    {
        narrationContinue.SetActive(false);
        
        if (currentNarration.GetContentLength() > narrationIndex + 1)
        {
            narrationIndex += 1;
            SetNarration(currentNarration.GetContent(narrationIndex));
        }
        else
        {
            narrationIndex = 0;
            ClearNarration();
        }
    }

    public void NewNarration(TextNarration cur)
    {
        ClearNarration();
        currentNarration = cur;
        SetNarration(cur.GetContent(0));
    }

    public void NewNarrationAnimated(TextNarration cur)
    {
        narrationAnimator.Play("Popup_Dialog");
        ClearNarration();
        narrationBackground.SetActive(true);
        currentNarration = cur;
    }


    public void SetNarrationAfterAnimation()
    {
        SetNarration(currentNarration.GetContent(0));
    }

    private void NarrateWords(string text)
    {
        wording = true;
        narrationWordsIndex = 0;
        frame_counter = 0;
        narrationWords = text.ToCharArray(); //text.Split(new string[] {" "}, StringSplitOptions.None).ToArray(); // for words instead of letters
        narrationCurrentWords = "";
    }

    private void NarrationUpdate()
    {
        if (frame_counter == frames_til_draw)
        {
            if (narrationWordsIndex < narrationWords.Length)
            {
                narrationCurrentWords += narrationWords[narrationWordsIndex]; // + " ";
                narrationText.text = narrationCurrentWords;
                narrationWordsIndex += 1;
                frames_til_draw = 1; // UnityEngine.Random.Range(2, 6); // for randomized frame drawing
                frame_counter = 0;
            }
            else
            {
                wording = false;
                narrationContinue.SetActive(true);
            }
        }
        frame_counter += 1;
    }

    public void UpdateToolbox()
    {
        if (!player)
            player  = GameObject.FindWithTag("Player");



        if (!mainCanvas)
            mainCanvas = GameObject.FindWithTag("Main Canvas");

        playerCam = player.GetComponentInChildren<Camera>();
        raycastManager = player.GetComponent<RaycastManager>();
        pickUp = player.GetComponent<MFPP.Modules.PickUpModule>();
        flipScript = player.GetComponent<flipScript>();
        UpdateTransforms();
        iconContainer = GameObject.FindWithTag("IconContainer").GetComponent<IconContainer>();
        pauseMenu = GameObject.Find("PauseMenu");

        cameraFogStartDefault = Camera.main.GetComponent<UnityStandardAssets.ImageEffects.GlobalFog>().startDistance;
        fogDensityDefault = RenderSettings.fogDensity;

        if (Toolbox.Instance == this)
        {
            narrationContainer = GameObject.Find("TextNarration");
            narrationText = narrationContainer.GetComponentInChildren<Text>();
            narrationAnimator = narrationContainer.GetComponent<Animator>();
            narrationBackground = narrationContainer.transform.Find("Background").gameObject;
            narrationContinue = narrationContainer.transform.Find("Continue").gameObject;
            SoundEffectsAudio = GameObject.Find("SoundEffectsAudio").GetComponent<AudioSource>();

            ClearNarration();

        }

        Debug.Log("adding player info!");
        Toolbox.Instance.player = player;
        Toolbox.Instance.playerCam = playerCam;
        Toolbox.Instance.pickUp = pickUp;
        Toolbox.Instance.raycastManager = raycastManager;
        Toolbox.Instance.flipScript = flipScript;
            UpdateTransforms();
        Toolbox.Instance.iconContainer = iconContainer;
        Toolbox.Instance.pauseMenu = pauseMenu;
        if (!Toolbox.Instance.narrationContainer)
        {
            Toolbox.Instance.narrationContainer = GameObject.Find("TextNarration");
            Toolbox.Instance.narrationAnimator = narrationContainer.GetComponent<Animator>();
        }

        if (!Toolbox.Instance.narrationText)
            Toolbox.Instance.narrationText = Toolbox.Instance.narrationContainer.GetComponentInChildren<Text>();


        Toolbox.Instance.SoundEffectsAudio = GameObject.Find("SoundEffectsAudio").GetComponent<AudioSource>();
        Toolbox.Instance.SoundtrackAudio = GameObject.Find("LaserChords").GetComponent<AudioSource>();


        Toolbox.Instance.soundEffectsSlider = pauseMenu.transform.GetChild(2).GetComponent<Slider>();
        Toolbox.Instance.soundEffectsSlider.onValueChanged.AddListener(delegate { VolumeChangeCheck(); });

        if (!Toolbox.Instance.narrationBackground)
        {
            Toolbox.Instance.narrationBackground = Toolbox.Instance.narrationContainer.transform.Find("Background").gameObject;
        }

        if (!Toolbox.Instance.narrationContinue)
        {
            Toolbox.Instance.narrationContinue = Toolbox.Instance.narrationContainer.transform.Find("Continue").gameObject;
        }

    }

    public void UpdateTransforms()
    {
        realWorldParentObject = GameObject.FindWithTag("Real");
        laserWorldParentObject = GameObject.FindWithTag("Laser");
    }

    public void FlipSharedMaterials(bool direction)
    {
        float end = 0f;
        StopAllCoroutines();
        if (direction)
            end = 0f;
        else
            end = 1f;
        foreach (Material mat in sharedMaterials)
        {
            float start = mat.GetFloat("_TransitionState");
            StartCoroutine(FlipTransitionRoutineShared(mat, start, end));
        }

        //Debug.Log(sharedMaterials.Count);

    }

    private IEnumerator FlipTransitionRoutineShared(Material material, float startpoint, float endpoint)
    {
        float elapsedTime = 0;
        float ratio = elapsedTime / globalFlipSpeed;
        //int property = Shader.PropertyToID("_D7A8CF01");

        while (ratio < 1f)
        {
            elapsedTime += Time.deltaTime;
            ratio = elapsedTime / globalFlipSpeed;
            float value = Mathf.Lerp(startpoint, endpoint, ratio);

            material.SetFloat("_TransitionState", value);
            //RendererExtensions.UpdateGIMaterials(mRenderer);

            yield return null;
        }
    }

    public void PlaySoundEffect(AudioClip audio)
    {
        SoundEffectsAudio.clip = audio;
        SoundEffectsAudio.volume = soundEffectsSlider.value;
        SoundEffectsAudio.Play();
    }

    void SetCustomValuesOnInstance()
    {
        if (Toolbox.Instance)
        {
            //Toolbox.player = player;
            //setting correct values if toolbox has already instantiated itself
            Toolbox.Instance.globalFlipSpeed = globalFlipSpeed;
            Toolbox.Instance.globalRealLaserFlipSpeed = globalRealLaserFlipSpeed;
            Toolbox.Instance.realCore = realCore;
            Toolbox.Instance.laserCore = laserCore;
            Toolbox.Instance.pauseMenu = pauseMenu;
            Toolbox.Instance.testercubeSounds = testercubeSounds;
            Toolbox.Instance.allIds = allIds;
            //Toolbox.Instance.SoundEffectsAudio = SoundEffectsAudio;

            //Debug.Log("working here");
            //Toolbox.Instance.narrationText = narrationText;
            //Toolbox.Instance.narrationContainer= narrationContainer;
            //Toolbox.Instance.narrationBackground = narrationBackground;

            ////ClearNarration();
            //
        }
    }

    public void LoadScene(string sceneName, string spawnPoint)
    {
        Instance.GetPlayer().GetComponent<MFPP.Player>().enabled = false;
        Instance.ClearNarration();
        Time.timeScale = 1f;

        GameObject button = GameObject.Find(spawnPoint).GetComponent<Spawner>().myButton;
        StartCoroutine(mainCanvas.GetComponent<LevelLoadingMenu>().loadNextScene(sceneName, spawnPoint, button, false));
    }

    public void SetSettings(WorldSettings settingsObject)
    {
        SkyboxTransition skyboxSettings = player.GetComponent<SkyboxTransition>();

        if (skyboxSettings)
        {
            if (settingsObject.GetSkyBoxMaterial())
            {
                RenderSettings.skybox = settingsObject.GetSkyBoxMaterial();
                skyboxSettings.material = settingsObject.GetSkyBoxMaterial();
            }
            
            skyboxSettings.laserFog = settingsObject.GetLaserFog();
            skyboxSettings.laserAmbient = settingsObject.GetLaserAmbient();
            skyboxSettings.realFog = settingsObject.GetRealFog();
            skyboxSettings.realAbmient = settingsObject.GetRealAmbient();
            
            if (settingsObject.GetRealGlobalParticle())
                skyboxSettings.realGlobalParticle = settingsObject.GetRealGlobalParticle();
            
            if (settingsObject.GetLaserGlobalParticle())
                skyboxSettings.laserGlobalParticle = settingsObject.GetLaserGlobalParticle();
        }
        
        if (flipScript) 
            flipScript.canFlip = settingsObject.GetPlayerCanSwitch();

        SoundTrackManager soundtrack = player.GetComponentInChildren<SoundTrackManager>();
        
        
        
        if (soundtrack)
        {
            soundtrack.flipClip = settingsObject.GetFlipClip();
            soundtrack.mute = !settingsObject.GetSoundtrackEnabled();
        }

        if (settingsObject.MuteOpeningSoundtrack())
        {
            if (soundtrack)
                soundtrack.dynamicVolume = 1f;
            if (TriggeredAudio.Instance)
                TriggeredAudio.Instance.mute = true;
            if (OpeningSongSingleton.Instance)
                OpeningSongSingleton.Instance.mute = true;
        }

        //else
        //{
        //    if (TriggeredAudio.Instance)
        //        TriggeredAudio.Instance.mute = false;
        //    if (OpeningSongSingleton.Instance)
        //        OpeningSongSingleton.Instance.mute = false;
        //}

    }
}