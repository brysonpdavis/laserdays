using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldSpaceUI : MonoBehaviour
{
	private GameObject player;

	private Camera mainCamera;

	private float playerDistance = 5;

	public float swtichDistance;

	private Text textComponent;

	private string originalText;

	private int originalFontSize;

	void Start () {
		player = Toolbox.Instance.GetPlayer();
		mainCamera = Camera.main;
		textComponent = GetComponentInChildren<Text>();
		originalText = textComponent.text;
		originalFontSize = textComponent.fontSize;
	}
	
	void Update ()
	{
		LookAtCamera();
		ChangeTextByDistance();
	}

	void SetText()
	{
		textComponent.text = ". . .";
		textComponent.fontSize = (int) (originalFontSize * 1.5);
	}

	void UnsetText()
	{
		textComponent.text = originalText;
		textComponent.fontSize = originalFontSize;
	}

	void LookAtCamera()
	{
		transform.LookAt(mainCamera.transform);
	}

	void ChangeTextByDistance()
	{
		playerDistance = Vector3.Distance(player.transform.position, transform.position);
		
		if (playerDistance > swtichDistance)
			SetText();
		else
			UnsetText();
	}
}
