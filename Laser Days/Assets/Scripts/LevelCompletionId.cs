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
public class LevelCompletionId : UniqueId
{

    protected PuzzleCompletion platform;

    public override void Awake()
    {
        platform = GetComponent<PuzzleCompletion>();

        if (Application.isPlaying)
        {
            // Debug.Log("platform ID: " + uniqueId);
            LevelCompletionData data = LoadObjData();
            ObjSetup(data);
        }
    }

    public override void SaveObj()
    {
        // Debug.Log("saving platform to " + uniqueId);

        BinaryFormatter formatter = new BinaryFormatter();
        Debug.Log(uniqueId);
        string path = Application.persistentDataPath + "/" + uniqueId;
        FileStream stream = new FileStream(path, FileMode.Create);

        LevelCompletionData data = new LevelCompletionData(platform);
        formatter.Serialize(stream, data);
        stream.Close();
    }

    public virtual LevelCompletionData LoadObjData()
    {
        string path = Application.persistentDataPath + "/" + uniqueId;

        if (!Toolbox.Instance.allIds.Contains(this))
            Toolbox.Instance.allIds.Add(this);

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            LevelCompletionData data = formatter.Deserialize(stream) as LevelCompletionData;
            stream.Close();
            return data;
        }

        else
        {
            Debug.LogError("Platform file not found in " + path);
            return null;
        }
    }


    public void ObjSetup(LevelCompletionData objectData)
    {
        if (objectData != null && objectData.completed)
        {
            GetComponent<PuzzleCompletion>().innerRing.gameObject.SetActive(false);
            GetComponent<Renderer>().enabled = false;
        }
    }
}
