using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class Toolbox : Singleton<Toolbox>
{
    protected Toolbox() { } // guarantee this will be always a singleton only - can't use the constructor!

    GameObject realWorldParentObject;
    GameObject laserWorldParentObject;
    GameObject player;
    RaycastManager raycastManager;
    MFPP.Modules.PickUpModule pickUp;
    flipScript flipScript;
    public float globalFlipSpeed = .4f;
    public float globalRealLaserFlipSpeed = .2f;
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
    public float soundEffectsVolume;
    public IList<Material> sharedMaterials;
    public MFPP.SoundContainerAsset testercubeSounds;
    public IList<UniqueId> allIds = new List<UniqueId>();
    Text narrationText;
    GameObject narrationBackground;
     
    

    void Awake()
    {

        SetCustomValuesOnInstance();

        // Your initialization code here
        UpdateToolbox();
        sharedMaterials = new List<Material>();
        DontDestroyOnLoad(this.gameObject);

        soundEffectsSlider = pauseMenu.transform.GetChild(2).GetComponent<Slider>();
        soundEffectsSlider.onValueChanged.AddListener(delegate { VolumeChangeCheck(); });
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

    public Text GetNarrationText()
    {
        return narrationText;
    }


    public GameObject GetNarrationBackground()
    {
        return narrationBackground;
    }

    public void UpdateToolbox()
    {
        player  = GameObject.FindWithTag("Player");
        raycastManager = player.GetComponent<RaycastManager>();
        pickUp = player.GetComponent<MFPP.Modules.PickUpModule>();
        flipScript = player.GetComponent<flipScript>();
        UpdateTransforms();
        iconContainer = GameObject.FindWithTag("IconContainer").GetComponent<IconContainer>();
        pauseMenu = GameObject.Find("PauseMenu");


        //text narration
        narrationText = GameObject.Find("TextNarration").GetComponent<Text>();
        narrationBackground = narrationText.gameObject.GetComponentInChildren<Image>().gameObject;


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

    void SetCustomValuesOnInstance()
    {
        if (Toolbox.Instance)
        {
            //setting correct values if toolbox has already instantiated itself
            Toolbox.Instance.globalFlipSpeed = globalFlipSpeed;
            Toolbox.Instance.globalRealLaserFlipSpeed = globalRealLaserFlipSpeed;
            Toolbox.Instance.realCore = realCore;
            Toolbox.Instance.laserCore = laserCore;
            Toolbox.Instance.pauseMenu = pauseMenu;
            //Toolbox.Instance.soundEffectsSlider = soundEffectsSlider;
            Toolbox.Instance.testercubeSounds = testercubeSounds;
            Toolbox.Instance.allIds = allIds;
        }
    }

}