using BackpackSurvivors.Game.Levels;
using BackpackSurvivors.UI.Tooltip.Triggers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.UI.Adventure;

public class AdventureLevelVisual : MonoBehaviour
{
	[SerializeField]
	private Image _levelIcon;

	[SerializeField]
	private TextMeshProUGUI _levelText;

	[SerializeField]
	private Material _levelCompletedMaterial;

	[SerializeField]
	private Material _levelIncompletedMaterial;

	[SerializeField]
	private DefaultTooltipTrigger _defaultTooltipTrigger;

	public void Init(LevelSO level, bool completed)
	{
		_levelText.SetText(level.LevelName);
		_levelIcon.sprite = level.LocationUIIcon;
		_levelIcon.material = (completed ? _levelCompletedMaterial : _levelIncompletedMaterial);
		_defaultTooltipTrigger.SetDefaultContent(level.LevelName, level.Description, active: true);
	}
}
