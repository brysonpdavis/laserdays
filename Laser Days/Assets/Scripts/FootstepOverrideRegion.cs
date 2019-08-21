using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepOverrideRegion : MonoBehaviour {

    public GameObject FootstepOverrideObject;
    private MFPP.FootstepOverride footstepOverride;
    public string overrideName;
    string _prevName;

    private void Start()
    {
        footstepOverride = FootstepOverrideObject.GetComponent<MFPP.FootstepOverride>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))// && (footstepOverride != null))
        {
            Debug.Log("ENTERED!");
            _prevName = footstepOverride.FootstepOverrideName;
            footstepOverride.FootstepOverrideName = overrideName;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))// && (footstepOverride != null))
        {
            footstepOverride.FootstepOverrideName = _prevName;
        }
    }


}
