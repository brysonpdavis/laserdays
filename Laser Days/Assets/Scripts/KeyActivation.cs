using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyActivation : MonoBehaviour
{

    [Header("Key Settings")]
    [SerializeField] public string key;
    [SerializeField] public GameObject[] doors;

    // set to true if any key will unlock the door, false if only a specific key will unlock it
    public bool anyKey;

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.GetComponent<ItemProperties>().isKey)
        {
            if (anyKey)
            {
                Unlock();
            }
            else
            {
                if (key == col.gameObject.GetComponent<ItemProperties>().key)
                {
                    Unlock();
                }
            }
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.gameObject.GetComponent<ItemProperties>().isKey)
        {
            if (anyKey)
            {
                Lock();
            }
            else
            {
                if (key == col.gameObject.GetComponent<ItemProperties>().key)
                {
                    Lock();
                }
            }
        }
    }

    void Lock () {
        foreach (GameObject door in doors) {
            door.SetActive(true);
        }
    }

    void Unlock () {
        foreach (GameObject door in doors) {
            door.SetActive(false);
        }
    }
}