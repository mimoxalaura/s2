using UnityEngine;

namespace SpriteShadersUltimate;

[AddComponentMenu("Sprite Shaders Ultimate/Utility/Unscaled Time")]
public class UnscaledTimeSSU : MonoBehaviour
{
	public bool dontDestroyOnLoad;

	private void Awake()
	{
		if (dontDestroyOnLoad)
		{
			Object.DontDestroyOnLoad(base.gameObject);
		}
	}

	private void Update()
	{
		Shader.SetGlobalFloat("UnscaledTime", Time.unscaledTime);
	}
}
