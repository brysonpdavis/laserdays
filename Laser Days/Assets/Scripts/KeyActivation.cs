using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyActivation : MonoBehaviour
{

    // use this script to create keys for doors. place this script on a trigger collider
    // where you want the key to be placed to open the door(s). drag any GameObjects into
    // the field "doors" to tell the editor which doors to unlock when the key is placed
    // into position.

    [Header("Key Settings")]
    [SerializeField] public string key;
    [SerializeField] public GameObject[] doors;

    // set to true if any key will unlock the door, false if only a specific key will unlock it
    public bool anyKey;

    void OnTriggerEnter(Collider col)
    {
        if (col.GetComponent<InteractableObject>() != null)
        {
            if (col.GetComponent<InteractableObject>().isKey)
            {
                if (anyKey)
                {
                    Unlock();
                }
                else
                {
                    if (key == col.GetComponent<InteractableObject>().key)
                    {
                        Unlock();
                    }
                }
            }
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.GetComponent<InteractableObject>() != null)
        {
            if (col.gameObject.GetComponent<InteractableObject>().isKey)
            {
                if (anyKey)
                {
                    Lock();
                }
                else
                {
                    if (key == col.GetComponent<InteractableObject>().key)
                    {
                        Lock();
                    }
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