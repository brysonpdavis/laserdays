using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivationZone : MonoBehaviour {
    [SerializeField]
    private GameObject[] objectsToActivate;
    private List<IActivatable> interfacesToActivate;

	void Start () {
        interfacesToActivate = new List<IActivatable>();

		foreach (GameObject obj in objectsToActivate)
        {
            foreach (IActivatable component in obj.GetComponents<IActivatable>())
            {
                interfacesToActivate.Add(component);
            }
        }
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            foreach(IActivatable component in interfacesToActivate)
            {
                component.Activate();
            }
        }
    }
}
