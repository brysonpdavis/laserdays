using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraShaderReplacement : MonoBehaviour
{
	[SerializeField]
	private Shader ReplacementShader;

	public void Start ()
	{
		Debug.Log("Activate replacement shader");
		GetComponent<Camera>().SetReplacementShader(ReplacementShader, "RenderType");
	}
}