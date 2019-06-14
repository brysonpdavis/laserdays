using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetScene : MonoBehaviour {

	public string spawnName;
	public string sceneName;

    public void Activate()
    {
        Toolbox.Instance.LoadScene(sceneName, spawnName);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("collide");
            Activate();
        } 
    }

}
