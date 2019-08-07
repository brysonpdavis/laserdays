using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NarrationAnimationTrigger : MonoBehaviour {

   
	public void TriggerNarration()
    {
		Toolbox.Instance.SetNarrationAfterAnimation();
        Toolbox.Instance.isNarrationAnimating = false;
	}

	public void SetAnimatingTrue()
    {
		Toolbox.Instance.isNarrationAnimating = true;
	}



}
