using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneUniqueIds : MonoBehaviour {

    public UniqueId[] allIds;

    private void Awake()
    {
        allIds = GetComponentsInChildren<UniqueId>();

        foreach (UniqueId id in allIds)
        {
            id.ObjSetup(id.LoadObjData());
           // Debug.Log("adding " + id.gameObject.name + "to newly open scene");

        } 
    }

    public void SceneResetSave()
    {
        foreach (UniqueId id in allIds)
        {
            //remove the file
            id.ClearObjData();

            //remove the id from the master list
            if (Toolbox.Instance.allIds.Contains(id))
            {
                Toolbox.Instance.allIds.Remove(id);
               // Debug.Log("removing " + id.gameObject.name + "from old scene");
            }
               
        }
    }

}