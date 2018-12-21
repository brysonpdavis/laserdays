using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.UI;

public class PlayerSave : MonoBehaviour{
    
    static string playerSaveLocation = "playerdata";
    public GameObject playerContainer;
    static GameObject player;
    MFPP.Modules.PickUpModule pickUp;
    PlayerSaveData data;
    public GameObject levelButtonContainer;
    IList<string> completedLevels = new List<String>();

    void Awake()
    {
        player = gameObject;
        pickUp = GetComponent <MFPP.Modules.PickUpModule>();

        if (Application.isPlaying)
        {
            data = LoadObjData();
            ObjSetup(data);
        }
    }

    private void Start()
    {
        if (data != null)
        {


            if (data.heldObj)
            {
                GameObject prevHeld = GameObject.Find("prevHeldObj");
                prevHeld.name = data.heldObjName;
                Debug.Log("object was called " + data.heldObjName);

                pickUp.PickUp(prevHeld.GetComponent<Rigidbody>());
            }

            foreach (string s in data.completedLevels)
            {
                Debug.Log("looking for " + s);
                Button button = GameObject.Find(s).GetComponent<Button>();
                ColorBlock cb = button.colors;
                cb.normalColor = Color.green;


                Color newBackground = button.GetComponent<Image>().color;
                newBackground.a = .7f;
                button.GetComponent<Image>().color = newBackground;


                button.colors = cb;
            }
        }
    }

    public void SavePlayerFile()
    {
        //Debug.Log(gameObject.name);
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/" + playerSaveLocation;
        FileStream stream = new FileStream(path, FileMode.Create);

        //saving heldobject
            bool heldObj = false;
            string heldObjName = null;
            if (pickUp.heldObject)
            {
                heldObj = true;
                heldObjName = pickUp.heldObject.name;
                pickUp.heldObject.name = "prevHeldObj";
            }

        //saving completed levels for menu
        Button[] buttonList = levelButtonContainer.GetComponentsInChildren<Button>();

        foreach (Button b in buttonList)
        {
            ColorBlock cb = b.colors;
            if (cb.normalColor == Color.green)
            {
                completedLevels.Add(b.gameObject.name);
                Debug.Log("adding completed level " + b.gameObject.name);
            }
        }

        String[] completedLevelsArray = new string[completedLevels.Count];
        completedLevels.CopyTo(completedLevelsArray, 0);

        Debug.Log("holding an object called " + heldObjName);
        PlayerSaveData data = new PlayerSaveData(player, heldObj, heldObjName, completedLevelsArray);
        formatter.Serialize(stream, data);
        stream.Close();
    }

    static PlayerSaveData LoadObjData()
    {
        string path = Application.persistentDataPath + "/" + playerSaveLocation;

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            PlayerSaveData data = formatter.Deserialize(stream) as PlayerSaveData;
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
        string path = Application.persistentDataPath + "/" + playerSaveLocation;

        if (File.Exists(path))
            File.Delete(path);

        else
        {
            Debug.LogError("Can't find a save to delete" + path);
        }
    }


    void ObjSetup(PlayerSaveData playerData)
    {
        if (playerData != null)
        {
            Vector3 position = new Vector3(playerData.position[0], playerData.position[1], playerData.position[2]);
            gameObject.transform.position = position;
            gameObject.layer = playerData.layer;

        }
    }
}
