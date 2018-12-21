using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class PlatformData
{

    public int layer;
    public float[] position = new float[3];
    //public float[] rotation; //will do this later

    public PlatformData(PlatformMover obj)
    {
        position[0] = obj.gameObject.transform.position.x;
        position[1] = obj.gameObject.transform.position.y;
        position[2] = obj.gameObject.transform.position.z;
    }

}
