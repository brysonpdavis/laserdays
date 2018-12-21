using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class PlayerSaveData
{

    public int layer;
    public bool heldObj;
    public string heldObjName;
    public float[] position = new float[3];
    public string[] completedLevels;
    //public float[] rotation; //will do this later

    public PlayerSaveData(GameObject player, bool held, string heldName, string[] levels)
    {
        layer = player.layer;
        heldObjName = heldName;
        completedLevels = levels;

        heldObj = held;
        position[0] = player.transform.position.x;
        position[1] = player.transform.position.y;
        position[2] = player.transform.position.z;
    }
}
