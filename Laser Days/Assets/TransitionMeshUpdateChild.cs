using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionMeshUpdateChild : MonoBehaviour {

    public int flipCountActivate;
    public int currentFlips = 0;
    [SerializeField] bool visibleInlaser;

    public GameObject player;

    private void Start()
    {
        if (currentFlips < flipCountActivate)
        {
            GetComponent<Transition>().enabled = false;
            GetComponent<Renderer>().enabled = false;
        }

        player = Toolbox.Instance.GetPlayer();

        Material material = GetComponent<Renderer>().material;
        visibleInlaser = ShaderUtility.ShaderIsLaser(material);

    }

    public void Check()
    {
        currentFlips += 1;
        if (currentFlips == flipCountActivate)
            Activate();
    }

	public void Activate()
    {
        GetComponent<Transition>().enabled = true;
        GetComponent<Renderer>().enabled = true;

        if (visibleInlaser && player.layer == 16)
        {
            //float current = GetComponent<Renderer>().material.GetFloat("_TransitionState");
            GetComponent<Transition>().SetStart(0f);
            //set it to off
            //Debug.Log("should set to off: i'm in laser player is real");
        }

        else if (!visibleInlaser && player.layer == 15)
        {
            GetComponent<Transition>().SetStart(1f);

            //set it to off
            //Debug.Log("should set to off: i'm in real player is laser");
        }
    }
}
