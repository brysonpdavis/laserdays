using UnityEngine;

public class Toolbox : Singleton<Toolbox> {
	protected Toolbox () {} // guarantee this will be always a singleton only - can't use the constructor!
 
	GameObject realWorldParentObject;
    GameObject laserWorldParentObject;
    GameObject player;
    public Color UIColorA;
    public Color UIColorB;
    public Color UIColorC;

 
	void Awake () {
		// Your initialization code here
        UpdateToolbox();
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
    public void UpdateToolbox()
    {
        player  = GameObject.FindWithTag("Player");
        realWorldParentObject = GameObject.FindWithTag("Real");
        laserWorldParentObject = GameObject.FindWithTag("Laser");

    }
}