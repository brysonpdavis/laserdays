using System.Linq;
using UnityEngine;

public class ReplacementColor : MonoBehaviour
{
	private const string ColorName = "ReplacementColor";
	
	public Color maskColor;

	public void Awake ()
	{
		foreach (var renderer in GetComponentsInChildren<Renderer>()) {
			var materials = renderer.materials;
			foreach (var mat in materials) {
				mat.SetColor(ColorName, Random.ColorHSV());
			}
		}
	}
}