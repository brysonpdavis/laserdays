using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Measure : MonoBehaviour {

    public Transform player;
    public Vector3 distance;
    public Vector3 distanceFreeze;
    public float totalDistance;
    public float totalDistanceFreeze;

    void Update () {

        if (player != null)
        {

            distance = player.position - this.transform.position;
            totalDistance = Vector3.Distance(player.position, this.transform.position);

            if (Input.GetMouseButtonDown(0))
            {
                distanceFreeze = player.position - this.transform.position;
                totalDistanceFreeze = Vector3.Distance(player.position, this.transform.position);
            }
        }
    }
}