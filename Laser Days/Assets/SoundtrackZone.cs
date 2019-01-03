using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundtrackZone : MonoBehaviour {

    public MFPP.FlipClipAsset mySoundtrack;

    GameObject player;
    SoundTrackManager soundtrack;

	// Use this for initialization
	void Start () {
        player = Toolbox.Instance.GetPlayer();
        soundtrack = player.GetComponentInChildren<SoundTrackManager>();
	}


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.Equals(player))
        {
            soundtrack.flipClip = mySoundtrack;
        }
    }
}
