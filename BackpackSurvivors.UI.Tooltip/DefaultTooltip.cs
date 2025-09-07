using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.UI.Tooltip;

public class DefaultTooltip : BaseTooltip
{
	[Header("Visuals")]
	[SerializeField]
	private Image _backgroundImage;

	public void SetContent(string header, string content, bool active)
	{
		SetText(content, header);
	}
}
