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
public class UniqueIdPlatform : UniqueId {

    protected PlatformMover platform;

    public override void Awake()
    {
        platform = GetComponent<PlatformMover>();

        if (Application.isPlaying)
        {
           // Debug.Log("platform ID: " + uniqueId);
            PlatformData data = LoadObjData();
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

        PlatformData data = new PlatformData(platform);
        formatter.Serialize(stream, data);
        stream.Close();
    }

    public virtual PlatformData LoadObjData()
    {
        string path = Application.persistentDataPath + "/" + uniqueId;

        if (!Toolbox.Instance.allIds.Contains(this))
            Toolbox.Instance.allIds.Add(this);

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            PlatformData data = formatter.Deserialize(stream) as PlatformData;
            stream.Close();
            return data;
        }

        else
        {
            Debug.LogError("Platform file not found in " + path);
            return null;
        }
    }


    public void ObjSetup(PlatformData objectData)
    {
        if (objectData != null)
        {
            GetComponent<PlatformMover>().Start();

            Vector3 position = new Vector3(objectData.position[0], objectData.position[1], objectData.position[2]);
            gameObject.transform.position = position;
            gameObject.layer = objectData.layer;
            //Debug.Log(gameObject.name);
            //Debug.Log("setting up at " + position);

        }
    }
}
