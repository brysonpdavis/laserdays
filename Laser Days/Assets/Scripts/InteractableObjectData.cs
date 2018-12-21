using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class InteractableObjectData {

    public int layer;
    public float[] position = new float[3];
    public string name;
    //public float[] rotation; //will do this later

    public InteractableObjectData(InteractableObject obj)
    {
        layer = obj.gameObject.layer;
        name = obj.gameObject.name;

        position[0] = obj.gameObject.transform.position.x;
        position[1] = obj.gameObject.transform.position.y;
        position[2] = obj.gameObject.transform.position.z;
    }

}
