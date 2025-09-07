using BackpackSurvivors.ScriptableObjects.Relics;
using BackpackSurvivors.UI.Tooltip.Triggers;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.UI.Stats;

public class RelicItemUI : MonoBehaviour
{
	[SerializeField]
	private Image _image;

	[SerializeField]
	private RelicTooltipTrigger _tooltip;

	private RelicSO _relic;

	public void Init(RelicSO relic)
	{
		_relic = relic;
		_image.sprite = relic.Icon;
		_tooltip.SetRelic(relic, active: true);
	}
}
