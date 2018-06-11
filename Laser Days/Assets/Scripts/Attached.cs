using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Attached : MonoBehaviour
{
    public GameObject TrackObject;
    public Vector3 Offset;

    void Update()
    {
        gameObject.transform.position = Camera.main.WorldToScreenPoint(TrackObject.transform.position) + Offset;
    }
}