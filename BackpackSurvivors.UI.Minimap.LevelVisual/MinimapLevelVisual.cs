using BackpackSurvivors.Game.Levels;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.UI.Minimap.LevelVisual;

internal class MinimapLevelVisual : MonoBehaviour
{
	[SerializeField]
	private Image _levelIcon;

	[SerializeField]
	private Image _glowImage;

	[SerializeField]
	private Material _levelCompletedMaterial;

	[SerializeField]
	private Material _levelIncompletedMaterial;

	[SerializeField]
	private Material _levelActiveMaterial;

	public void Init(LevelSO level, bool completed)
	{
		_levelIcon.sprite = level.LocationUIIcon;
		_levelIcon.material = (completed ? _levelCompletedMaterial : _levelIncompletedMaterial);
		_glowImage.gameObject.SetActive(value: false);
	}

	public void SetActiveLevel()
	{
		_levelIcon.material = _levelActiveMaterial;
		_glowImage.gameObject.SetActive(value: true);
	}
}
