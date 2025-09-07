using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.Game.Backpack;

[ExecuteAlways]
[RequireComponent(typeof(Image))]
public class PassUnscaledTimeToShader : MonoBehaviour
{
	private Image rend;

	private string originalMatName;

	private static readonly int UnscaledTime = Shader.PropertyToID("_UnscaledTime");

	private void Awake()
	{
		rend = GetComponent<Image>();
		originalMatName = rend.material.name;
	}

	private void Update()
	{
		if (rend.material.HasProperty(UnscaledTime))
		{
			rend.material.SetFloat(UnscaledTime, Time.unscaledTime);
		}
		else
		{
			Object.Destroy(this);
		}
		rend.material = Object.Instantiate(rend.material);
		rend.material.name = originalMatName;
	}
}
