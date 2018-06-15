using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyActivation : MonoBehaviour {   

    [Header("Key Settings")]
    [SerializeField]public string key ;
    [SerializeField]public GameObject door;
    public bool anyKey;
  
    // Update is called once per frame
    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.GetComponent<ItemProperties>().isKey)
        {
            if (anyKey)
            {
                door.SetActive(false);
            }
            else
            {
                if(key == col.gameObject.GetComponent<ItemProperties>().itemName)
                { door.SetActive(false); }
            }
        }
        
    }
}
