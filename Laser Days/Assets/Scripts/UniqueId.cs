// Script for generating a unique but persistent string identifier belonging to this 
 // component
 //
 // We construct the identifier from two parts, the scene name and a guid.
 // 
 // The guid is guaranteed to be unique across all components loaded at 
 // any given time. In practice this means the ID is unique within this scene. We 
 // then append the name of the scene to it. This ensures that the identifier will be 
 // unique accross all scenes. (as long as your scene names are unique)
 // 
 // The identifier is serialised ensuring it will remaing the same when the level is 
 // reloaded
 //
 // This code copes with copying the game object we are part of, using prefabs and 
 // additive level loading
 //
 // Final point - After adding this Component to a prefab, you need to open all the 
 // scenes that contain instances of this prefab and resave them (to save the newly 
 // generated identifier). I recommend manually saving it rather than just closing it
 // and waiting for Unity to prompt you to save it, as this automatic mechanism 
 // doesn't always seem to know exactly what needs saving and you end up being re-asked
 // incessantly
 //
 // Written by Diarmid Campbell 2017 - feel free to use and ammend as you like
 //
 using UnityEngine;
 using System.Collections.Generic;
 using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
 
 #if UNITY_EDITOR
 using UnityEditor;
 using UnityEditor.SceneManagement;
 #endif
 
 [ExecuteInEditMode]
 public class UniqueId : MonoBehaviour {
 
     // global lookup of IDs to Components - we can esnure at edit time that no two 
     // components which are loaded at the same time have the same ID. 
    static Dictionary<string, UniqueId> allGuids = new Dictionary<string, UniqueId> ();
 
    public string uniqueId;
    protected HoldableObject Holdable;

    private void Start()
    {
        //Toolbox.Instance.allIds.Add(this);
    }

    // Only compile the code in an editor build
#if UNITY_EDITOR

    // Whenever something changes in the editor (note the [ExecuteInEditMode])
    protected void Update(){
         // Don't do anything when running the game
         if (Application.isPlaying)
             return;
         
         // Construct the name of the scene with an underscore to prefix to the Guid
         string sceneName = gameObject.scene.name + "_";
 
         // if we are not part of a scene then we are a prefab so do not attempt to set 
         // the id
         if  (sceneName == null) return;
 
         // Test if we need to make a new id
         bool hasSceneNameAtBeginning = (uniqueId != null && 
             uniqueId.Length > sceneName.Length && 
             uniqueId.Substring (0, sceneName.Length) == sceneName);
         
         bool anotherComponentAlreadyHasThisID = (uniqueId != null && 
             allGuids.ContainsKey (uniqueId) && 
             allGuids [uniqueId] != this);
 
         if (!hasSceneNameAtBeginning || anotherComponentAlreadyHasThisID){
             uniqueId =  sceneName + Guid.NewGuid ();
             EditorUtility.SetDirty (this);
             EditorSceneManager.MarkSceneDirty (gameObject.scene);
         }
         // We can be sure that the key is unique - now make sure we have 
         // it in our list
         if (!allGuids.ContainsKey (uniqueId)) {
             allGuids.Add(uniqueId, this);
         }
     }
 
     // When we get destroyed (which happens when unloading a level)
     // we must remove ourselves from the global list otherwise the
     // entry still hangs around when we reload the same level again
     // but now the THIS pointer has changed and we end up changing 
     // our ID unnecessarily
     protected void OnDestroy(){
         allGuids.Remove(uniqueId);
        //if (Toolbox.Instance.allIds.Contains(this))
                //Toolbox.Instance.allIds.Remove(this);
     }
#endif

    public void Save()
    {
        if (Application.isPlaying && gameObject.activeInHierarchy)
        {
            SaveObj();
        }
    }

    public virtual void Awake()
    {
        Holdable = GetComponent<HoldableObject>();


        if (Application.isPlaying)
        {

            //if this object wakes up before player has decided what to do, then add the object to the toolbox list.
            if (!Toolbox.Instance.loadSelection)
            {
                if (!Toolbox.Instance.allIds.Contains(this))
                    Toolbox.Instance.allIds.Add(this);
            }


            //if player has already decided to load from save and that decision has been made
            if (Toolbox.Instance.loadSelection && Toolbox.Instance.loadFromSave)
            {
                Setup();
            }
        }
    }


    public virtual void SaveObj()
    {
        Debug.Log(gameObject.name);
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/" + uniqueId;
        FileStream stream = new FileStream(path, FileMode.Create);

        InteractableObjectData data = new InteractableObjectData(Holdable) ;
        formatter.Serialize(stream, data);
        stream.Close();
    }

    public virtual InteractableObjectData LoadObjData()
    {
        string path = Application.persistentDataPath + "/" + uniqueId;

        if (!Toolbox.Instance.allIds.Contains(this))
            Toolbox.Instance.allIds.Add(this);

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            InteractableObjectData data = formatter.Deserialize(stream) as InteractableObjectData;
            stream.Close();
            return data;
        }

        else 
        {
          //  Debug.LogError("Save file not found in " + path);
            return null;
        }
    }


    public void ClearObjData()
    {
        string path = Application.persistentDataPath + "/" + uniqueId;

        if (File.Exists(path))
            File.Delete(path);

        else
        {
            Debug.LogError("Can't find a save to delete" + path);
        }
    }

    //TO BE CALLED FROM TOOLBOX
    public virtual void Setup()
    {
        InteractableObjectData data = LoadObjData();
        ObjSetup(data);
    }


    //used by this script internally
    public virtual void ObjSetup(InteractableObjectData objectData)
    {
        if (objectData != null)
        {
            Vector3 position = new Vector3(objectData.position[0], objectData.position[1], objectData.position[2]);
            gameObject.transform.position = position;
            gameObject.layer = objectData.layer;
            gameObject.name = objectData.name;

            Holdable = GetComponent<HoldableObject>();
            //Debug.Log(gameObject.name);

            if (objectData.layer == 11)
                Holdable.LoadShader(true);
            if (objectData.layer == 10)
                Holdable.LoadShader(false);

        }
    }
}