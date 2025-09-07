using BackpackSurvivors.ScriptableObjects.Relics;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.UI.Tooltip;

public class RelicTooltip : BaseTooltip
{
	[Header("Visuals")]
	[SerializeField]
	private Image _backgroundImage;

	public void SetRelic(RelicSO relicSO, bool active)
	{
		SetText(relicSO.Description, relicSO.Name);
	}

	internal void RefreshUI()
	{
		LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)base.transform);
		Canvas.ForceUpdateCanvases();
	}
}
