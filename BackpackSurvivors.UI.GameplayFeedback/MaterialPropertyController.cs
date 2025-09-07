using System.Collections.Generic;
using UnityEngine;

namespace BackpackSurvivors.UI.GameplayFeedback;

public class MaterialPropertyController : MonoBehaviour
{
	[TextArea(2, 5)]
	public string HowToUse = "For the effect to appear, Add the material(s) in this list as a Full Screen Pass Renderer Feature to the active Universal Renderer Data asset. For more details check out the Documentation.";

	[Tooltip("Drag and drop your materials here in the Inspector.")]
	public List<Material> materials;

	private string propertyName = "_enabled";

	[SerializeField]
	private bool _isEnabled;

	private void OnValidate()
	{
		UpdateShaderProperties();
	}

	private void Update()
	{
		if (!Application.isPlaying)
		{
			UpdateShaderProperties();
		}
	}

	public void SetEnabled(bool enabled)
	{
		_isEnabled = enabled;
		UpdateShaderProperties();
	}

	private void OnEnable()
	{
		UpdateShaderProperties();
	}

	private void OnDisable()
	{
		UpdateShaderProperties();
	}

	private void UpdateShaderProperties()
	{
		if (materials == null || materials.Count <= 0)
		{
			return;
		}
		int value = (_isEnabled ? 1 : 0);
		foreach (Material material in materials)
		{
			if (material != null)
			{
				material.SetInt(propertyName, value);
			}
		}
	}
}
