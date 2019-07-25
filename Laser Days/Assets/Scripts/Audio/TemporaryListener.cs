using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using UnityEngine;

public class TemporaryListener : MonoBehaviour {

	public void StartWaitingForPlayer()
	{
		StartCoroutine(DestroyOnPlayer());
	}

	private IEnumerator DestroyOnPlayer()
	{
		while (true)
		{
			if (Toolbox.Instance.GetPlayer())
			{
				Destroy(gameObject);
				break;
			}			
			
			yield return null;
		}
	}	
}
