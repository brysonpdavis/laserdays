using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class UIElementBob : MonoBehaviour {

    public float bobMag;
    public float timeScale;
    private RectTransform rect;
    private float ogPos;


    private void Start()
    {
        rect = GetComponent<RectTransform>();
        ogPos = rect.transform.position.y;
    }
    private void FixedUpdate()
    {
        Vector3 t = rect.transform.position;
        rect.transform.position = new Vector3(t.x, ogPos + Mathf.Sin(Time.time * timeScale) * bobMag, t.z);
    }

}
