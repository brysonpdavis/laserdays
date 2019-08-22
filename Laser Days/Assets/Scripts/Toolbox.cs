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

    public void EnablePlayerMovement()
    {
        var playerScript = player.GetComponent<MFPP.Player>();

        playerScript.Movement.AllowMovement = true;
        playerScript.Movement.AllowMouseMove = true;
        flipScript.canFlip = true;
    }

    public void DisablePlayerMovement()
    {
        var playerScript = player.GetComponent<MFPP.Player>();

        playerScript.Movement.AllowMovement = false;
        playerScript.Movement.AllowMouseMove = false;
        flipScript.canFlip = false;
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
            SoundEffectsAudio = GameObject.Find("SoundEffectsAudio").GetComponent<AudioSource>();
            
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
        Toolbox.Instance.SoundEffectsAudio = GameObject.Find("SoundEffectsAudio").GetComponent<AudioSource>();
        Toolbox.Instance.SoundtrackAudio = GameObject.Find("LaserChords").GetComponent<AudioSource>();


        Toolbox.Instance.soundEffectsSlider = pauseMenu.transform.GetChild(2).GetComponent<Slider>();
        Toolbox.Instance.soundEffectsSlider.onValueChanged.AddListener(delegate { VolumeChangeCheck(); });
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
        }
    }

    public void LoadScene(string sceneName, string spawnPoint)
    {
        Instance.GetPlayer().GetComponent<MFPP.Player>().enabled = false;
        NarrationController.CancelNarration();
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

                if (PlayerInReal())
                    skyboxSettings.material.SetFloat("_TransitionState", 0f);
                else
                    skyboxSettings.material.SetFloat("_TransitionState", 1f);
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