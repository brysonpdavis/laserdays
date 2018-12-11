using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeOutwards : MonoBehaviour
{

    Vector3 movement;

    // Use this for initialization
    void Start()
    {

        float totalposition = transform.position.x + transform.position.y + transform.position.z;

        float scaledX = transform.position.x / totalposition;
        float scaledY = transform.position.x / totalposition;
        float scaledZ = transform.position.x / totalposition;

        movement = new Vector3(scaledX, scaledY, scaledZ);

    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(-movement);
    }
}