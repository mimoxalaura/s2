using BackpackSurvivors.Game.Levels;
using BackpackSurvivors.UI.Tooltip.Triggers;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.UI.Adventure;

public class AdventureLevelLocationUI : MonoBehaviour
{
	[SerializeField]
	private Material _bossMaterial;

	[SerializeField]
	private DefaultTooltipTrigger _defaultTooltipTrigger;

	[SerializeField]
	private Image _locationImage;

	internal void Init(Vector2 item, LevelSO level)
	{
		base.transform.localPosition = item;
		_locationImage.sprite = level.LocationUIIcon;
		_defaultTooltipTrigger.SetDefaultContent(level.LevelName, level.Description, active: true);
		if (level.BossLevel)
		{
			GetComponent<Image>().material = _bossMaterial;
		}
	}
}
