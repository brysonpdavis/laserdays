using UnityEngine;

public class Toolbox : Singleton<Toolbox> {
	protected Toolbox () {} // guarantee this will be always a singleton only - can't use the constructor!
 
	GameObject realWorldParentObject;
    GameObject laserWorldParentObject;
    GameObject player;

 
	void Awake () {
		// Your initialization code here
        realWorldParentObject = GameObject.FindWithTag("Real");
        laserWorldParentObject = GameObject.FindWithTag("Laser");
        player  = GameObject.FindWithTag("Player");

        DontDestroyOnLoad(this.gameObject);
	}
 
	// (optional) allow runtime registration of global objects
	static public T RegisterComponent<T> () where T: Component {
		return Instance.GetOrAddComponent<T>();
	}

    public Transform GetRealWorldParent()
    {
        return realWorldParentObject.transform;
    }

    public Transform GetLaserWorldParent()
    {
        return laserWorldParentObject.transform;
    }

    public GameObject GetPlayer()
    {
        return player;
    }
}